using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StockChat.Identity;
using StockChat.Identity.Database;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

var connectionString = configuration.GetConnectionString("DefaultConnection");

// Add services to the container.

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString, o => o.MigrationsAssembly("StockChat.Identity")));

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddRazorPages();

builder.Services
    .AddIdentityServer()
    // demo purposes only
    .AddDeveloperSigningCredential()
    
    .AddConfigurationStore(options =>
    {
        options.ConfigureDbContext = b => b.UseSqlServer(connectionString, o => o.MigrationsAssembly("StockChat.Identity"));
    })
    .AddOperationalStore(options =>
    {
        options.ConfigureDbContext = b => b.UseSqlServer(connectionString, o => o.MigrationsAssembly("StockChat.Identity"));
    })
    .AddAspNetIdentity<IdentityUser>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseIdentityServer();

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapRazorPages()
    .RequireAuthorization();
app.MapControllers();

// For demo purposes only
SeedData.EnsureSeedData(app);

app.Run();