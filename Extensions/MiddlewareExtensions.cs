using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sadkah.Backend.Extensions
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseCustomStatusCodes(this IApplicationBuilder app)
        {
            app.UseStatusCodePages(async context =>
            {
                if (context.HttpContext.Response.StatusCode == 404)
                {
                    await context.HttpContext.Response.WriteAsJsonAsync(
                        ApiResponse<object>.FailResponse("The resource you are looking for was not found. Please check the URL and try again.")
                    );
                }

                if (context.HttpContext.Response.StatusCode == 429)
                {
                    await context.HttpContext.Response.WriteAsJsonAsync(
                        ApiResponse<object>.FailResponse("Too many requests. Please try again later.")
                    );
                }
            });

            return app;
        }
    }
}