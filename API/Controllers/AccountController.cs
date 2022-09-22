using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using API.Data;
using API.DTO;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace API.Controllers
{
  public class AccountController : BaseApiController
  {
    private readonly DataContext _context;
    private readonly ILogger<AccountController> _logger;
    private readonly ITokenService _tokenService;
    private readonly IUserRepository _userRepository;

    public AccountController(DataContext context,
     ILogger<AccountController> logger,
     ITokenService tokenService,
     IUserRepository userRepository)
    {
      _context = context;
      _logger = logger;
      _tokenService = tokenService;
      _userRepository = userRepository;
    }

    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register(RegisterDto request)
    {
      var username = request.Username;
      var password = request.Password;
      _logger.LogInformation($"Username={username} and password={password}");

      if (await CheckUserExists(username)) return BadRequest("User already taken");

      using var hmac = new HMACSHA512();
      var hashedPassword = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
      var user = new AppUser
      {
        UserName = username,
        PasswordHash = hashedPassword,
        PasswordSalt = hmac.Key
      };
      _context.Users.Add(user);
      await _context.SaveChangesAsync();
      return new UserDto
      {
        Username = user.UserName,
        Token = _tokenService.CreateToken(user),
        PhotoUrl = user.Photos?.FirstOrDefault(x => x.IsMain)?.Url,
      };
    }

    private async Task<bool> CheckUserExists(string username)
    {
      return await _context.Users.AnyAsync(x => x.UserName == username.ToLower());
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto login)
    {
      var user = await _userRepository.GetUserByUsernameAsync(login.UserName);
      if (user == null) return Unauthorized("Invalid username or password");
      using var hmac = new HMACSHA512(user.PasswordSalt);
      var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(login.Password));
      if (computedHash.ToString() != user.PasswordHash.ToString()) return Unauthorized("Invalid username or password");
      return new UserDto()
      {
        Username = user.UserName,
        Token = _tokenService.CreateToken(user),
        PhotoUrl = user.Photos?.FirstOrDefault(x => x.IsMain)?.Url,
      };
    }
  }
}