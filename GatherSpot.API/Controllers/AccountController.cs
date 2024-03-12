using System.Security.Claims;
using Domain;
using GatherSpot.API.DTOs;
using GatherSpot.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GatherSpot.API.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class AccountController : ControllerBase
	{
		private readonly UserManager<AppUser> _userManager;
		private readonly TokenService _tokenService;
		private readonly IConfiguration _config;
		private readonly HttpClient _httpClient;

		public AccountController(UserManager<AppUser> userManager, TokenService tokenService, IConfiguration config)
		{
			_userManager = userManager;
			_tokenService = tokenService;
			_config = config;
			_httpClient = new HttpClient()
			{
				BaseAddress = new System.Uri("https://graph.facebook.com")
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
			if (user is null) return Unauthorized();
			var result = await _userManager.CheckPasswordAsync(user, loginDto.Password);
			if(!result) return Unauthorized();
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

			return CreateUserDto(user);
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
			return CreateUserDto(user);
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
