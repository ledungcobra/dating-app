using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Extensions;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace API.Helpers
{
  public class LogUserActivity : IAsyncActionFilter
  {
    public LogUserActivity(ILogger<LogUserActivity> logger)
    {
      logger.LogInformation("Init new log user activity");
    }
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
      var resultContext = await next();
      if (!resultContext.HttpContext.User.Identity.IsAuthenticated) return;
      var userId = resultContext.HttpContext.User.GetUserId();
      var repo = (IUserRepository)resultContext.HttpContext.RequestServices.GetService<IUserRepository>();
      var user = await repo.GetUserByIdAsync(userId);
      user.LastActive = DateTime.Now;
      await repo.SaveAllAsync();
    }
  }
}