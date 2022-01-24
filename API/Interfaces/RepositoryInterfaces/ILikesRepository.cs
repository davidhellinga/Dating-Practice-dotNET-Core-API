using API.DTOs;
using API.Entities;
using API.Helpers.PaginationHelpers;
using API.Helpers.PaginationHelpers.Params;

namespace API.Interfaces.RepositoryInterfaces;

public interface ILikesRepository
{
    Task<UserLike> GetUserLike(int sourceUserId, int likedUserId);
    Task<AppUser> GetUserWithLikes(int userId);
    Task<PagedList<LikeDto>> GetUserLikes(LikesParams likesParams);
    
}