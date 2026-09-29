using System.Collections.Concurrent;

namespace ProductInventoryAPI.Middleware
{
    public class RateLimitingMiddleware
    {
        private readonly RequestDelegate _next;

        private static readonly ConcurrentDictionary<string, List<DateTime>> Requests
            = new();

        public RateLimitingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var user = context.User.Identity?.Name ?? "Anonymous";

            var now = DateTime.UtcNow;

            var requests = Requests.GetOrAdd(
                user,
                _ => new List<DateTime>()
            );

            lock (requests)
            {
                requests.RemoveAll(
                    timestamp => timestamp <= now.AddMinutes(-1)
                );

                //if (requests.Count >= 2)
                if (requests.Count >= 100)
                {
                    context.Response.StatusCode =
                        StatusCodes.Status429TooManyRequests;

                    context.Response.Headers.RetryAfter = "60";

                    return;
                }

                requests.Add(now);
            }

            await _next(context);
        }
    }
}