using Application.Profiles;
using Domain;

namespace Application.Activities
{
	public record ActivityDto
	{
		public Guid Id { get; set; }
		public string Title { get; set; }
		public DateTime Date { get; set; }
		public string Description { get; set; }
		public string Category { get; set; }
		public string City { get; set; }
		public string Venue { get; set; }
		public string HostUsername { get; set; }
		public string IsCancelled { get; set; }
		public ICollection<AttendeeDto> Attendees { get; set; }
	}
}
