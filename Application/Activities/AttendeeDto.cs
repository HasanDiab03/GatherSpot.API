namespace Application.Activities
{
	public record AttendeeDto
	{
		public string Username { get; set; }
		public string DisplayName { get; set; }
		public string Bio { get; set; }
		public string Image { get; set; }
	}
}
