using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Photos;
using Application.Repositories;
using Domain;
using Firebase.Storage;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Persistence;

namespace Infrastructure.Photos
{
	public class PhotoAccessor : IPhotoAccessor
	{
		private readonly DataContext _context;
		private readonly FirebaseStorage _storage;
		public PhotoAccessor(IConfiguration config, DataContext context)
		{
			_context = context;
			_storage = new FirebaseStorage(config["Firebase:FolderURL"]);
		}
		public async Task<Photo> AddPhoto(IFormFile file)
		{
			if (file.Length > 0)
			{
				await using var stream = file.OpenReadStream();
				var id = Guid.NewGuid();
				var fileName = $"{id}_{Path.GetFileName(file.FileName)}";
				var imageUrl = await _storage.Child("images")
					.Child(fileName)
					.PutAsync(stream)
				               ?? throw new Exception("Failed To Upload Image");
				var image = new Photo()
				{
					Url = imageUrl,
					FileName = fileName
				};
				return image;
			}
			return null;
		}

		public async Task DeletePhoto(Photo photo)
			=> await _storage.Child("images").Child(photo.FileName).DeleteAsync();
	}
}
