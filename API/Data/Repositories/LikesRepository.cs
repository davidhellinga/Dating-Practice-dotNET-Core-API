using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers.PaginationHelpers;
using API.Helpers.PaginationHelpers.Params;
using API.Interfaces;
using API.Interfaces.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data.Repositories;

public class LikesRepository : ILikesRepository
{
    private readonly DataContext _context;

    public LikesRepository(DataContext context)
    {
        _context = context;
    }

    public async Task<UserLike> GetUserLike(int sourceUserId, int likedUserId)
    {
        return await _context.Likes.FindAsync(sourceUserId, likedUserId);
    }

    public async  Task<AppUser> GetUserWithLikes(int userId)
    {
        return await _context.Users.Include(x => x.LikedUsers).FirstOrDefaultAsync(x => x.Id == userId);
    }

    public async Task<PagedList<LikeDto>> GetUserLikes(LikesParams likesParams)
    {
        var users = _context.Users.OrderBy(u => u.Username).AsQueryable();
        var likes = _context.Likes.AsQueryable();
        switch (likesParams.Predicate)
        {
            case "liked":
                likes = likes.Where(like => like.SourceUserId == likesParams.UserId);
                users = likes.Select(like => like.LikedUser);
                break;
            case "likedBy":
                likes = likes.Where(like => like.LikedUserId == likesParams.UserId);
                users = likes.Select(like => like.SourceUser);
                break;
        }

        var likedUsers = users.Select(user => new LikeDto
        {
            Username = user.Username,
            KnownAs = user.KnownAs,
            Age = user.DateOfBirth.CalculateAge(),
            PhotoUrl = user.Photos.FirstOrDefault(p => p.IsMain).Url,
            City = user.City,
            Id = user.Id
        });
        return await PagedList<LikeDto>.CreateAsync(likedUsers, likesParams.PageNumber, likesParams.PageSize);
    }
}