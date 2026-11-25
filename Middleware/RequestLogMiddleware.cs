using System.Diagnostics;

namespace ParcelTrack.Middleware
{
    public class RequestLogMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestLogMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            string requestId = Guid.NewGuid().ToString("N").Substring(0, 8);
            context.Response.Headers["X-Request-Id"] = requestId;

            Stopwatch watch = Stopwatch.StartNew();
            await _next(context);
            watch.Stop();

            Console.WriteLine(
                $"[{requestId}] {context.Request.Method} {context.Request.Path} -> {context.Response.StatusCode} in {watch.ElapsedMilliseconds} ms");
        }
    }
}
