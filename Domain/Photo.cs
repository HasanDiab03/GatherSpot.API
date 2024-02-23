namespace Domain
{
	public class Photo
	{
		public Guid Id { get; set; }
		public string Url { get; set; }
		public string FileName { get; set; }
		public bool IsMain { get; set; }
	}
}
