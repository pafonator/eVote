using eVote.src.Repository;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;
using System.IdentityModel.Tokens.Jwt;
using eVote.src.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using eVote.src.Client;




var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// Register the DbContext
builder.Services.AddDbContext<EVoteDbContext>();

// Used to handle authentication for all API requests
builder.Services.AddTransient<ClientApiHandler>();


// Http client for API requests (sends request to the adress)
var baseAddress = builder.Configuration["eVoteAPI:BaseAddress"];
builder.Services.AddHttpClient("eVoteAPI", client =>
{
    client.BaseAddress = new Uri(baseAddress); // Adjust the base address as needed
})
    .AddHttpMessageHandler<ClientApiHandler>();

builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor(); // For accessing HttpContext in Razor Pages


// Authentication
var secretKey = builder.Configuration["Jwt:Secret"];
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

builder.Services.AddSingleton(new JwtToken(secretKey));

// Service to access curent user information
builder.Services.AddScoped<CurrentUserService>();


var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();

app.Run();
