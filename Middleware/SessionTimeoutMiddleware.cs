using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace irevlogix_backend.Middleware
{
    public class SessionTimeoutMiddleware
    {
        private readonly RequestDelegate _next;

        public SessionTimeoutMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var endpoint = context.GetEndpoint();
            if (endpoint?.Metadata?.GetMetadata<AuthorizeAttribute>() == null)
            {
                await _next(context);
                return;
            }

            if (context.User.Identity?.IsAuthenticated == true)
            {
                var loginTimeClaim = context.User.FindFirst("LoginTime")?.Value;
                if (!string.IsNullOrEmpty(loginTimeClaim) && long.TryParse(loginTimeClaim, out var loginTime))
                {
                    var loginDateTime = DateTimeOffset.FromUnixTimeSeconds(loginTime).UtcDateTime;
                    var clientId = context.User.FindFirst("ClientId")?.Value;
                    
                    if (!string.IsNullOrEmpty(clientId))
                    {
                        var dbContext = context.RequestServices.GetRequiredService<Data.ApplicationDbContext>();
                        var timeoutSetting = await dbContext.ApplicationSettings
                            .Where(s => s.ClientId == clientId)
                            .FirstOrDefaultAsync();
                        
                        var timeoutMinutes = timeoutSetting?.LoginTimeoutMinutes ?? 5;
                        var sessionExpiry = loginDateTime.AddMinutes(timeoutMinutes);
                        
                        if (DateTime.UtcNow > sessionExpiry)
                        {
                            context.Response.StatusCode = 401;
                            context.Response.ContentType = "application/json";
                            await context.Response.WriteAsync("{\"message\":\"Session has expired due to inactivity\",\"sessionExpired\":true}");
                            return;
                        }
                    }
                }
            }

            await _next(context);
        }
    }
}
