using Microsoft.AspNetCore.Identity;

namespace Domain
{
	public class AppUser : IdentityUser // this identity user entity will provide us with the basic info of a user,
                                     // such as email, username, password, etc...
	{
		public string DisplayName { get; set; }
		public string Bio { get; set; }
		public ICollection<ActivityAttendee> Activities { get; set; }
		public ICollection<Photo> Photos { get; set; }
		public ICollection<UserFollowing> Followers { get; set; }
		public ICollection<UserFollowing> Followings { get; set; } 
		public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
	}
}
