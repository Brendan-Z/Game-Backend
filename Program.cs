using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using A2.Data;
using A2.Handlers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<A2DBContext>(options => options.UseSqlite(builder.Configuration["WebAPIConnection"]));
builder.Services.AddScoped<IA2Repo, A2Repo>();

builder.Services.AddAuthentication().AddScheme<AuthenticationSchemeOptions, A2AuthHandler>("A2UserAuthentication", null);
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("UserOnly", policy => policy.RequireClaim("userName"));
});

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
