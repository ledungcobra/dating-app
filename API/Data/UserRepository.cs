using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTO;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
  public class UserRepository : IUserRepository
  {
    private readonly DataContext _context;
    public readonly IMapper _mapper;

    public UserRepository(DataContext context, IMapper mapper)
    {
      _mapper = mapper;
      _context = context;
    }
    public void Update(AppUser user)
    {
      _context.Entry(user).State = EntityState.Modified;
    }

    public async Task<bool> SaveAllAsync()
    {
      return await _context.SaveChangesAsync() > 0;
    }

    public async Task<IEnumerable<AppUser>> GetUsersAsync()
    {
      return await _context.Users
          .Include(u => u.Photos)
          .ToListAsync();
    }

    public async Task<AppUser> GetUserByIdAsync(int id)
    {
      return await _context.Users.Include(x => x.Photos).FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<AppUser> GetUserByUsernameAsync(string username)
    {
      return await _context.Users
      .Include(x => x.Photos)
      .SingleOrDefaultAsync(x => x.UserName == username);
    }

    public async Task<MemberDto> GetMemberAsync(string username)
    {
      return await _context.Users
        .Where(x => x.UserName == username)
        .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
        .SingleOrDefaultAsync();
    }

    public async Task<PageList<MemberDto>> GetMembersAsync(UserParams userParams)
    {
      var query = _context.Users.AsQueryable();
      query = query.Where(u => u.UserName != userParams.CurrentUsername);
      query = query.Where(u => u.Gender == userParams.Gender);

      // Fiter by min and max age
      var minDob = DateTime.Today.AddYears(-userParams.MaxAge - 1);
      var maxDob = DateTime.Today.AddYears(-userParams.MinAge);

      // Order
      query = userParams.OrderBy switch
      {
        "created" => query.OrderByDescending(u => u.Created),
        _ => query.OrderByDescending(u => u.LastActive)
      };
      
      query = query.Where(u => u.DateOfBirth >= minDob && u.DateOfBirth <= maxDob);

      return await PageList<MemberDto>.CreateAsync(query
                .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
                .AsNoTracking(),
      userParams.PageNumber, userParams.PageSize);
    }
  }
}
