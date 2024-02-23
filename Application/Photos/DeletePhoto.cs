using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Core;
using Application.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Photos
{
    public record DeletePhotoCommand(Guid Id) : IRequest<Result<Unit>>;

    public class DeletePhotoHandler : IRequestHandler<DeletePhotoCommand, Result<Unit>>
    {
        private readonly IPhotoAccessor _photoAccessor;
        private readonly DataContext _context;
        private readonly IUserAccessor _userAccessor;

        public DeletePhotoHandler(IPhotoAccessor photoAccessor, DataContext context, IUserAccessor userAccessor)
        {
            _photoAccessor = photoAccessor;
            _context = context;
            _userAccessor = userAccessor;
        }
        public async Task<Result<Unit>> Handle(DeletePhotoCommand request, CancellationToken cancellationToken)
        {
            var user = await _context.Users
                .Include(u => u.Photos)
                .FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername(), cancellationToken);
            if (user is null)
                return null;
            var photo = user.Photos.FirstOrDefault(x => x.Id == request.Id);
            if (photo is null) return null;
            if (photo.IsMain) return Result<Unit>.Failure("Cannot delete main photo");
            await _photoAccessor.DeletePhoto(photo);
            user.Photos.Remove(photo);
            var result = await _context.SaveChangesAsync(cancellationToken) > 0;
            return result ? Result<Unit>.Success(Unit.Value) : Result<Unit>.Failure("Failed To Delete Photo");
        }
    }
}
