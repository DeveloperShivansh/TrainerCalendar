using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using TrainerCalendar.Authentications;
using TrainerCalendar.Contexts;
using TrainerCalendar.Models;

namespace TrainerCalendar.Middlewares
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class RequestProcessorMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestProcessorMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public Task Invoke(HttpContext httpContext, ApplicationDbContext dbContext, UserManager<User> userManager, IConfiguration configuration, IJwtAuthenticationManager jwtAuthenticationManager)
        {
            User? u = userManager.GetUserAsync(httpContext.User).GetAwaiter().GetResult();
            CurrentUser? currentUser = new CurrentUser();
            if (u == null) currentUser = null;
            else
            {
                currentUser.User = u;
                currentUser.Email = u.Email;
                currentUser.Role = u.Role;
                currentUser.PhoneNumber = u.PhoneNumber;
                currentUser.UserName = u.UserName;

                CurrentRequest.UserManager = userManager;
                CurrentRequest.JwtAuthenticationManager = jwtAuthenticationManager;
                CurrentRequest.DbContext = dbContext;
                CurrentRequest.Configuration = configuration;
                CurrentRequest.CurrentUser = currentUser;
            }
            return _next(httpContext);
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class RequestProcessorMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestProcessorMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestProcessorMiddleware>();
        }
    }
}
