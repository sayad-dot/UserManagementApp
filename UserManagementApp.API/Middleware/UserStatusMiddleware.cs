using System.Security.Claims;
using UserManagementApp.Core.Interfaces; // ADD THIS LINE

namespace UserManagementApp.API.Middleware
{
    public class UserStatusMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public UserStatusMiddleware(RequestDelegate next, IServiceScopeFactory serviceScopeFactory)
        {
            _next = next;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Skip check for registration, login, and email verification
            var path = context.Request.Path.Value ?? "";
            if (path.StartsWith("/api/auth/register") || 
                path.StartsWith("/api/auth/login") ||
                path.StartsWith("/api/auth/verify-email") ||
                path.StartsWith("/swagger"))
            {
                await _next(context);
                return;
            }

            // Check if user is authenticated
            var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId != null && int.TryParse(userId, out int id))
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
                
                var user = await userRepository.GetByIdAsync(id);
                if (user == null || user.Status == Core.Enums.UserStatus.Blocked)
                {
                    // FIFTH REQUIREMENT: Redirect to login if user is blocked or doesn't exist
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("User account is blocked or doesn't exist");
                    return;
                }
            }

            await _next(context);
        }
    }
}