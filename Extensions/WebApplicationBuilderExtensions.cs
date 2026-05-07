using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Sadkah.Backend.Extensions
{
    public static class WebApplicationBuilderExtensions
    {
        public static WebApplicationBuilder AddJwtAuthentication(this WebApplicationBuilder builder, IConfiguration config)
        {
            builder.Services.AddAuthentication(options => {
                options.DefaultAuthenticateScheme = 
                options.DefaultChallengeScheme = 
                options.DefaultForbidScheme = 
                options.DefaultScheme =
                options.DefaultSignInScheme =
                options.DefaultSignOutScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options => {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = config["JWT:Issuer"],
                    ValidAudience = config["JWT:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JWT:SigningKey"] ?? "temporary_secret_key_for_development_purposes_only")),
                    ClockSkew = TimeSpan.Zero
                };
                options.Events = new JwtBearerEvents
                {
                    OnChallenge = context =>
                    {
                        context.HandleResponse();

                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        context.Response.ContentType = "application/json";

                        return context.Response.WriteAsJsonAsync(ApiResponse<object>.FailResponse("You are not authorized to access this resource, or your token is invalid or has expired. Please log in first."));
                    },
                    OnForbidden = context =>
                    {
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        context.Response.ContentType = "application/json";

                        return context.Response.WriteAsJsonAsync(ApiResponse<object>.FailResponse("You don’t have permission to access this. Please contact the administrator if you think this is a mistake."));
                    }
                };
            });

            return builder;
        }
        public static WebApplicationBuilder AddIdentityServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddIdentity<User, IdentityRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 8;
            })
            .AddEntityFrameworkStores<ApplicationDBContext>();

            return builder;
        }
        public static WebApplicationBuilder AddCustomValidation(this WebApplicationBuilder builder)
        {
            builder.Services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var errors = context.ModelState
                        .Where(x => x.Value?.Errors.Count > 0)
                        .Select(x => new
                        {
                            field = x.Key,
                            messages = x.Value!.Errors.Select(e => e.ErrorMessage)
                        });

                    return new BadRequestObjectResult(
                        ApiResponse<object>.FailResponse("Validation failed.", errors)
                    );
                };
            });

            return builder;
        }
    }
}