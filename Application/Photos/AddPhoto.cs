using Application.Core;
using Application.Repositories;
using AutoMapper;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Photos
{
	public record AddPhotoCommand(IFormFile File) : IRequest<Result<Photo>>;

	public class AddPhotoHandler : IRequestHandler<AddPhotoCommand, Result<Photo>>
	{
		private readonly DataContext _context;
		private readonly IPhotoAccessor _photoAccessor;
		private readonly IUserAccessor _userAccessor;

		public AddPhotoHandler(DataContext context, IPhotoAccessor photoAccessor, IUserAccessor userAccessor)
		{
			_context = context;
			_photoAccessor = photoAccessor;
			_userAccessor = userAccessor;
		}
		public async Task<Result<Photo>> Handle(AddPhotoCommand request, CancellationToken cancellationToken)
		{
			var user = await _context.Users
				.Include(p => p.Photos)
				.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername(), cancellationToken);
			if (user is null)
				return null;
			var photo = await _photoAccessor.AddPhoto(request.File);
			if (!user.Photos.Any(p => p.IsMain))
				photo.IsMain = true;
			user.Photos.Add(photo);
			var result = await _context.SaveChangesAsync(cancellationToken) > 0;
			return result ? Result<Photo>.Success(photo) : Result<Photo>.Failure("Failed To Add Photo");
		}
	}
}
