using System.Security.Claims;
using System.Text;
using Domain;
using GatherSpot.API.DTOs;
using GatherSpot.API.Services;
using Infrastructure.Email;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;

namespace GatherSpot.API.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class AccountController : ControllerBase
	{
		private readonly UserManager<AppUser> _userManager;
		private readonly SignInManager<AppUser> _signInManager;
		private readonly TokenService _tokenService;
		private readonly IConfiguration _config;
		private readonly EmailSender _emailSender;
		private readonly HttpClient _httpClient;

		public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager,
			TokenService tokenService, IConfiguration config, EmailSender emailSender)
		{
			_userManager = userManager;
			_signInManager = signInManager;
			_tokenService = tokenService;
			_config = config;
			_emailSender = emailSender;
			_httpClient = new HttpClient()
			{
				BaseAddress = new Uri("https://graph.facebook.com")
				// this will be used to verify that the access token sent to us by the user is valid for our facebook app, 
				// also it can be used to get the user details
			};
		}
		[HttpPost("login")]
		[AllowAnonymous] // this is so that the AccountController endpoints don't require authentication
		public async Task<ActionResult<UserDto>> Login([FromBody] LoginDto loginDto)
		{
			var user = await _userManager.Users
				.Include(p => p.Photos)
				.FirstOrDefaultAsync(x => x.Email == loginDto.Email);
			if (user is null) return Unauthorized("Invalid Email");
			if (!user.EmailConfirmed)
				return Unauthorized("Email Not Confirmed");
			var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
			if(!result.Succeeded) return Unauthorized("Invalid Password");
			await SetRefreshToken(user);
			return CreateUserDto(user);
		}

		[HttpPost("register")]
		[AllowAnonymous] // this is so that the AccountController endpoints don't require authentication
		public async Task<ActionResult<UserDto>> Register([FromBody] RegisterDto registerDto)
		{
			if (await _userManager.Users.AnyAsync(user => user.UserName == registerDto.Username))
			{
				ModelState.AddModelError("username", "username already taken");
				return ValidationProblem(); // we add the validation error to the Model State,
                                // and then return a validationProblem in order to get an errors object that has the errors
			}

			if (await _userManager.Users.AnyAsync(user => user.Email == registerDto.Email))
			{
				ModelState.AddModelError("email", "Email already taken");
				return ValidationProblem();
			}

			var user = new AppUser()
			{
				DisplayName = registerDto.DisplayName,
				Email = registerDto.Email,
				UserName = registerDto.Username
			};
			var result = await _userManager.CreateAsync(user, registerDto.Password);
			if (!result.Succeeded)
				return BadRequest(result.Errors);
			var origin = Request.Headers["origin"];
			var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
			token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token)); // since the token will be sent in an html in the email, it needs to be encoded so that it doesn't get corrupted
			var verifyUrl = $"{origin}/account/verifyEmail?token={token}&email={user.Email}";
			var message = $"<p>Please Click the Below Link to Verify Your Email Address: </p><p><a href='{verifyUrl}'>Click To Verify Email</a></p>";
			await _emailSender.SendEmailAsync(user.Email, "Please Verify Email", message);
			return Ok("Registration Success - Please Verify Email");
		}

		[AllowAnonymous]
		[HttpPost("verifyEmail")]
		public async Task<IActionResult> VerifyEmail(string token, string email)
		{
			var user = await _userManager.FindByEmailAsync(email);
			if (user is null)
				return Unauthorized();
			var decodedTokenBytes = WebEncoders.Base64UrlDecode(token);
			var decodedToken = Encoding.UTF8.GetString(decodedTokenBytes);
			var result = await _userManager.ConfirmEmailAsync(user, decodedToken);
			if (!result.Succeeded)
				return BadRequest("Could not Verify Email Address");
			return Ok("Email confirmed - you can now login");
		}

		[AllowAnonymous]
		[HttpGet("resendEmailConfirmationLink")]
		public async Task<IActionResult> ResendEmailConfirmationLink(string email) 
		{
			var user = await _userManager.FindByEmailAsync(email);
			if (user is null)
				return Unauthorized();
			var origin = Request.Headers["origin"];
			var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
			token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token)); // since the token will be sent in an html in the email, it needs to be encoded so that it doesn't get corrupted
			var verifyUrl = $"{origin}/account/verifyEmail?token={token}&email={user.Email}";
			var message = $"<p>Please Click the Below Link to Verify Your Email Address: </p><p><a href='{verifyUrl}'>Click To Verify Email</a></p>";
			await _emailSender.SendEmailAsync(user.Email, "Please Verify Email", message);
			return Ok("Email Verification Link Resent");
		}

		[HttpGet]
		[Authorize]
		public async Task<ActionResult<UserDto>> GetCurrentUser()
		{
			var user = await _userManager.Users
				.Include(x => x.Photos)
				.FirstOrDefaultAsync(x => x.Email == User.FindFirstValue(ClaimTypes.Email));
			// the User here is a Claims Principal associated to the action execution,
			// since we are using JWT to authenticate, then the claims principal will be inside the token
			return CreateUserDto(user);
		}

		[AllowAnonymous]
		[HttpPost("fbLogin")]
		public async Task<ActionResult<UserDto>> FacebookLogin(string accessToken)
		{
			var fbVerifyKeys = _config["Facebook:AppId"] + "|" + _config["Facebook:AppSecret"];
			var verifyTokenResponse =
				await _httpClient.GetAsync($"debug_token?input_token={accessToken}&access_token={fbVerifyKeys}");
			if (!verifyTokenResponse.IsSuccessStatusCode)
				return Unauthorized();
			var fbUrl = $"me?access_token={accessToken}&fields=name,email,picture.width(100).height(100)";
			var fbInfo = await _httpClient.GetFromJsonAsync<FacebookDto>(fbUrl);
			var user = await _userManager.Users.Include(x => x.Photos).FirstOrDefaultAsync(x => x.Email == fbInfo.Email);
			if (user is not null)
				return CreateUserDto(user);
			user = new AppUser()
			{
				DisplayName = fbInfo.Name,
				Email = fbInfo.Email,
				UserName = fbInfo.Email,
				Photos = new List<Photo>()
				{
					new Photo()
					{
						Id = Guid.Parse("fb_" + fbInfo.Id),
						Url = fbInfo.Picture.Data.Url,
						IsMain = true
					}
				}
			};
			var result = await _userManager.CreateAsync(user);
			if (!result.Succeeded)
				return BadRequest("Problem Creating User Account");
			await SetRefreshToken(user);
			return CreateUserDto(user);
		}

		[Authorize]
		[HttpPost("refreshToken")]
		public async Task<ActionResult<UserDto>> RefreshToken()
		{
			var refreshToken = Request.Cookies["refreshToken"];
			var user = await _userManager.Users
				.Include(r => r.RefreshTokens)
				.Include(p => p.Photos)
				.FirstOrDefaultAsync(x => x.UserName == User.FindFirstValue(ClaimTypes.Name));
			if(user is null)
				return Unauthorized();
			var oldToken = user.RefreshTokens.SingleOrDefault(x => x.Token == refreshToken);
			if (oldToken is not null && !oldToken.IsActive)
				return Unauthorized();
			return CreateUserDto(user);
		}

		private async Task SetRefreshToken(AppUser user)
		{
			var refreshToken = _tokenService.GenerateRefreshToken();
			user.RefreshTokens.Add(refreshToken);
			await _userManager.UpdateAsync(user);
			var cookieOptions = new CookieOptions
			{
				HttpOnly = true,
				Expires = DateTime.UtcNow.AddDays(7),
			};
			Response.Cookies.Append("refreshToken", refreshToken.Token, cookieOptions);
		}
		private UserDto CreateUserDto(AppUser user)
			=> new()
			{
				DisplayName = user.DisplayName,
				Image = user.Photos?.FirstOrDefault(x => x.IsMain)?.Url,
				Token = _tokenService.CreateToken(user),
				Username = user.UserName,
			};
}
}
