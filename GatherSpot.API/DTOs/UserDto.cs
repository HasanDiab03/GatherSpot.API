namespace GatherSpot.API.DTOs
{
	public record UserDto
	{
		public string DisplayName { get; set; }
		public string Token { get; set; }
		public string Image { get; set; }
		public string Username { get; set; }
	}
}
