using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using StockChat.Api.Data;
using StockChat.Api.Hubs;
using StockChat.Api.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var configuration = builder.Configuration;

IdentityModelEventSource.ShowPII = true;

builder.Services.AddMediator();

builder.Services.Configure<MongoOptions>(configuration.GetSection("MongoOptions"));
builder.Services.Configure<RabbitOptions>(configuration.GetSection("RabbitOptions"));

builder.Services.AddTransient(typeof(IRepository<>), typeof(Repository<>));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
    {
        options.Authority = configuration["IdentityAuthority"];
        options.Audience = "chatapi";
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false
        };
    });

builder.Services.AddControllers();

builder.Services.AddSignalR();

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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapHub<ChatHub>("/chat");

app.Run();