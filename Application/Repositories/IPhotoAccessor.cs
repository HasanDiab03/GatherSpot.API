using Domain;
using Microsoft.AspNetCore.Http;

namespace Application.Repositories
{
	public interface IPhotoAccessor
	{
		Task<Photo> AddPhoto(IFormFile file);
		Task DeletePhoto(Photo photo);
	}
}
