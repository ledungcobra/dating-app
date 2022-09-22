using API.Data;
using API.Helpers;
using API.Interfaces;
using API.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace API.Extensions
{
  public static class ApplicationServiceExtensions
  {
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {

      // Strongly typed configuration 
      services.Configure<CloudinarySettings>(configuration.GetSection("CloudinarySettings"));

      services.AddDbContext<DataContext>(options =>
      {
        options.UseSqlite(configuration.GetConnectionString("DefaultConnection"));
      });
      services.AddSingleton<ITokenService, TokenService>();
      services.AddAutoMapper(typeof(AutoMapperProfiles).Assembly);

      services.AddLogging(config =>
      {
        config.AddConsole();
      });

      services.AddScoped<IUserRepository, UserRepository>();
      services.AddScoped<IPhotoService, PhotoService>();
      return services;
    }
  }
}
