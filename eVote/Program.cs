using eVote.src.Repository;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// Register the DbContext
builder.Services.AddDbContext<EVoteDbContext>();

// Http client for API requests (sends request to the adress)
builder.Services.AddHttpClient("eVoteAPI", client =>
{
    //TODO fix base adress hardcoded
    client.BaseAddress = new Uri("https://localhost:7152/"); // Adjust the base address as needed
});
builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor(); // For accessing HttpContext in Razor Pages


var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();

app.Run();
