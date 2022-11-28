using Microsoft.EntityFrameworkCore;
using GameAcademy.Data;
using Microsoft.AspNetCore.Authentication;
using GameAcademy.Handler;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services
    .AddAuthentication()
    .AddScheme<AuthenticationSchemeOptions, AuthHandler>("Authentication", null);
builder.Services.AddDbContext<GameAcademyDBContext>(
    options => options.UseSqlite(builder.Configuration["AuthDbConnection"])
);
builder.Services.AddDbContext<GameAcademyDBContext>(options => options.UseSqlite(builder.Configuration["WebAPIConnection"]));
builder.Services.AddScoped<IGameAcademyRepo, GameAcademyRepo>();
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireClaim("admin"));
    options.AddPolicy("AuthOnly", policy => {
        policy.RequireAssertion(context =>
            context.User.HasClaim(c =>
            (c.Type == "normalUser" || c.Type == "admin")));
    });
});

builder.Services.AddCors(o => o.AddPolicy("NUXT", builder =>
{
    builder.AllowAnyOrigin()
           .AllowAnyMethod()
           .AllowAnyHeader();
}));

var app = builder.Build();


app.UseRouting();

app.UseCors("NUXT");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
