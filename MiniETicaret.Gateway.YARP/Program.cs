using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MiniETicaret.Gateway.YARP.Context;
using MiniETicaret.Gateway.YARP.Dtos;
using MiniETicaret.Gateway.YARP.Models;
using MiniETicaret.Gateway.YARP.Services;
using System.Text;
using TS.Result;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSql"));
});
builder.Services.AddReverseProxy().
    LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

builder.Services.AddAuthentication().AddJwtBearer(opt =>
{
    opt.TokenValidationParameters = new()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration.GetSection("JWT:Issuer").Value,
        ValidAudience = builder.Configuration.GetSection("JWT:Audience").Value,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetSection("JWT:SecretKey").Value!)),
        ValidateLifetime = true
    };
});

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseCors(x => x.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod());

app.MapGet("/", () => "Hello World!");

app.MapPost("/auth/register", async (RegisterDto request, ApplicationDbContext context, CancellationToken token) =>
{
    bool isUsernameExists = await context.Users.AnyAsync(p => p.Username == request.Username, cancellationToken: token);

    if (isUsernameExists)
    {
        return Results.BadRequest(Result<string>.Failure("Username already exists"));
    }

    User user = new()
    {
        Username = request.Username,
        Password = request.Password,
    };

    await context.Users.AddAsync(user, token);
    await context.SaveChangesAsync(token);

    return Results.Ok(Result<string>.Succeed("User created with successfully"));

});

app.MapPost("/auth/login", async (LoginDto request, ApplicationDbContext context, CancellationToken cancellationToken) =>
{
    User? user = await context.Users.FirstOrDefaultAsync(p => p.Username == request.Username, cancellationToken: cancellationToken);

    if (user is null)
    {
        return Results.BadRequest(Result<string>.Failure($"Username :{user!.Username} not found "));
    }

    JwtProvider jwtProvider = new(builder.Configuration);

    string token = jwtProvider.CreateToken(user);
    return Results.Ok(Result<string>.Succeed(token));
});

app.UseAuthentication();
app.UseAuthorization();

app.MapReverseProxy();

using (var scoped = app.Services.CreateScope())
{
    var srv = scoped.ServiceProvider;
    var context = srv.GetRequiredService<ApplicationDbContext>();
    context.Database.Migrate();
}


app.Run();
