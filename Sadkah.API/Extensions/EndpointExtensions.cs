using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sadkah.API.Extensions
{
    public static class EndpointExtensions
    {
        public static WebApplication MapCustomEndpoints(this WebApplication app)
        {
            app.MapGet("/", () =>
            {
                return Results.Ok(ApiResponse<object>.SuccessResponse(
                    "API is running",
                    new {
                        version = "1.0",
                        documentation = "/swagger",
                    }
                ));
            });

            return app;
        }
    }
}