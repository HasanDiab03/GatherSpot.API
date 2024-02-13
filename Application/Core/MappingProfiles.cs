using AutoMapper;
using Domain;

namespace Application.Core
{
	public class MappingProfiles : Profile
	{
		public MappingProfiles()
		{
			CreateMap<Activity, Activity>(); // for updating, from new activity to old one
		}
	}
}
