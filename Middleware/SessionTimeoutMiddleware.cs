using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace irevlogix_backend.Middleware
{
    public class SessionTimeoutMiddleware
    {
        private readonly RequestDelegate _next;
        private static readonly Dictionary<string, DateTime> _lastActivityTimes = new();
        private static readonly object _lockObject = new();

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
                var userIdClaim = context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                var clientId = context.User.FindFirst("ClientId")?.Value;
                
                if (!string.IsNullOrEmpty(userIdClaim) && !string.IsNullOrEmpty(clientId))
                {
                    var sessionKey = $"{clientId}:{userIdClaim}";
                    var currentTime = DateTime.UtcNow;
                    
                    var dbContext = context.RequestServices.GetRequiredService<Data.ApplicationDbContext>();
                    var timeoutSetting = await dbContext.ApplicationSettings
                        .Where(s => s.ClientId == clientId && s.SettingKey == "LoginTimeoutMinutes")
                        .FirstOrDefaultAsync();
                    
                    var timeoutMinutes = timeoutSetting?.LoginTimeoutMinutes ?? 10;
                    
                    lock (_lockObject)
                    {
                        if (_lastActivityTimes.TryGetValue(sessionKey, out var lastActivity))
                        {
                            var timeSinceLastActivity = currentTime - lastActivity;
                            if (timeSinceLastActivity.TotalMinutes > timeoutMinutes)
                            {
                                _lastActivityTimes.Remove(sessionKey);
                                context.Response.StatusCode = 401;
                                context.Response.ContentType = "application/json";
                                await context.Response.WriteAsync("{\"message\":\"Session has expired due to inactivity\",\"sessionExpired\":true}");
                                return;
                            }
                        }
                        
                        _lastActivityTimes[sessionKey] = currentTime;
                    }
                }
            }

            await _next(context);
        }
    }
}
