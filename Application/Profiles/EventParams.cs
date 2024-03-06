namespace Application.Profiles
{
	public record EventParams
	{
		public bool IsPast { get; set; }
		public bool IsHost { get; set; }
	}
}
