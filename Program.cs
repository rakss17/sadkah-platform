using Sadkah.Backend.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddDbContext<ApplicationDBContext>(options => {
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddIdentityServices();
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddCustomValidation();
builder.Services.AddApplicationServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UseCustomStatusCodes();

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

app.Run();
