using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers.PaginationHelpers;
using API.Helpers.PaginationHelpers.Params;
using API.Interfaces.RepositoryInterfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data.Repositories;

public class UserRepository : IUserRepository
{
    private readonly DataContext _context;
    private readonly IMapper _mapper;

    public UserRepository(DataContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public void Update(AppUser user)
    {
        _context.Entry(user).State = EntityState.Modified;
    }

    public async Task<IEnumerable<AppUser>> GetUsersAsync()
    {
        return await _context.Users.Include(p => p.Photos).ToListAsync();
    }

    public async Task<AppUser> GetUserByIdAsync(int id)
    {
        return await _context.Users.FindAsync(id);
    }

    public async Task<AppUser> GetUserByUsernameAsync(string username)
    {
        return await _context.Users.Include(p => p.Photos).SingleOrDefaultAsync(x => x.UserName == username);
    }

    public async Task<PagedList<MemberDto>> GetMembersAsync(UserParams userParams)
    {
        var attractedTo = userParams.Sexuality.AttractedTo(userParams.Gender);

        var query = _context.Users.AsQueryable();
        //filters
        query = query.Where(u => u.UserName != userParams.CurrentUsername);
        if (attractedTo != "all") query = query.Where(u => u.Gender == attractedTo);

        var minDob = DateTime.Today.AddYears(-userParams.MaxAge - 1);
        var maxDob = DateTime.Today.AddYears(-userParams.MinAge);
        query = query.Where(u => u.DateOfBirth >= minDob && u.DateOfBirth <= maxDob);
        //sorting
        query = userParams.OrderBy switch
        {
            "created" => query.OrderByDescending(u => u.Created),
            _ => query.OrderByDescending(u => u.LastActive)
        };

        return await PagedList<MemberDto>.CreateAsync(
            query.ProjectTo<MemberDto>(_mapper.ConfigurationProvider).AsNoTracking(), userParams.PageNumber,
            userParams.PageSize);
    }

    public async Task<MemberDto> GetMemberAsync(string username)
    {
        return await _context.Users.Where(x => x.UserName == username)
            .ProjectTo<MemberDto>(_mapper.ConfigurationProvider).SingleOrDefaultAsync();
    }

    public async Task<string> GetUserGender(string username)
    {
        return await _context.Users.Where(x => x.UserName == username)
            .Select(x => x.Gender)
            .FirstOrDefaultAsync();
    }
}