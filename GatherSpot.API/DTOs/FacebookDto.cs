namespace GatherSpot.API.DTOs
{
	public record FacebookDto
	{
		public string Id { get; set; }
		public string Email { get; set; }
		public string Name { get; set; }
		public FacebookPicture Picture { get; set; }
	}

	public record FacebookPicture
	{
		public FacebookPictureData Data { get; set; }
	}

	public record FacebookPictureData
	{
		public string Url { get; set; }
	}
}
