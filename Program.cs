

using A2.data;
using A2.Data;
using A2.Handler;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services
    .AddAuthentication()
    .AddScheme<AuthenticationSchemeOptions, A2AuthHandler>("MyAuthentication", null);
// Add services to the container.
builder.Services.AddDbContext<A2DBContext>(
    options => options.UseSqlite(builder.Configuration["A2DbConnection"])
);

builder.Services.AddScoped<IA2Repo, A2Repo>();
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireClaim("admin"));
    options.AddPolicy("UserOnly", policy => policy.RequireClaim("userName"));
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();


app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
