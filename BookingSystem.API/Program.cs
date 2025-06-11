using BookingSystem.Application.Interfaces.Repositories;
using BookingSystem.Application.Interfaces.Services;
using BookingSystem.Application.Services;
using BookingSystem.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Wasfaty.Application.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// ÅÖÇİÉ CORS ááÓãÇÍ ÈÇáæÕæá ÇáÚÇã
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin()  // ÇáÓãÇÍ ÈÃí ãÕÏÑ
                  .AllowAnyMethod()  // ÇáÓãÇÍ ÈÃí ØÑíŞÉ (GET, POST, PUT, DELETE)
                  .AllowAnyHeader(); // ÇáÓãÇÍ ÈÃí ÑÃÓ (headers)
        });
});


// ÅÖÇİÉ ÇáÎÏãÇÊ
builder.Services.AddControllers();
// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// ÅÚÏÇÏ ÎÏãÇÊ ÇáãÕÇÏŞÉ ÈÇÓÊÎÏÇã JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        // ÅÚÏÇÏ ãÚáãÇÊ ÇáÊÍŞŞ ãä ÕÍÉ ÇáÊæßä
        options.TokenValidationParameters = new TokenValidationParameters
        {
            // ÊÍŞŞ ããÇ ÅĞÇ ßÇä ÇáãõÕÏÑ ááÊæßä ÕÍíÍğÇ
            ValidateIssuer = true,
            // ÊÍŞŞ ããÇ ÅĞÇ ßÇä ÇáÌãåæÑ ÇáãÓÊåÏİ ááÊæßä ÕÍíÍğÇ
            ValidateAudience = true,
            // ÊÍŞŞ ããÇ ÅĞÇ ßÇäÊ ÕáÇÍíÉ ÇáÊæßä ŞÏ ÇäÊåÊ
            ValidateLifetime = true,
            // ÊÍŞŞ ããÇ ÅĞÇ ßÇä ãİÊÇÍ ÇáÊæŞíÚ ÕÍíÍğÇ
            ValidateIssuerSigningKey = true,
            // ÇáÌåÉ ÇáãÕÏÑÉ ááÊæßä¡ íÊã ÇáÍÕæá ÚáíåÇ ãä ãáİ ÇáÅÚÏÇÏÇÊ
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            // ÇáÌãåæÑ ÇáãÓÊåÏİ ÇáĞí íÓÊÎÏã ÇáÊæßä¡ íÊã ÇáÍÕæá Úáíå ãä ãáİ ÇáÅÚÏÇÏÇÊ
            ValidAudience = builder.Configuration["Jwt:Audience"],
            // ÇáãİÊÇÍ ÇáÓÑí ÇáĞí íÓÊÎÏã áÊæŞíÚ ÇáÊæßä¡ íÊã ÊÍæíáå Åáì ãÕİæİÉ ÈÇíÊ
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

builder.Services.AddDbContext<BookingDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();


builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();


builder.Services.AddScoped<IStaffRepository, StaffRepository>();
builder.Services.AddScoped<IStaffService, StaffService>();

// ÅÖÇİÉ ÎÏãÇÊ Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();


// Êãßíä Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
        c.RoutePrefix = string.Empty; // áÚÑÖ ÇáæÇÌåÉ Úáì ÇáÌĞÑ
    });
    app.MapOpenApi(); // ÊÃßÏ ãä Ãäß ÊŞæã ÈÊÔÛíá MapOpenApi åäÇ

}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();  // ÊÃßÏ ãä Ãä Swagger íÚãá ÈÔßá ÕÍíÍ

    app.MapOpenApi();
}



app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

// åäÇ¡ ÊÃßÏ ãä Ãäß ÊÓÊÎÏã CORS ÃæáÇğ ŞÈá ÇáãÕÇÏŞÉ æÇáÜ Authorization
app.UseCors("AllowAll");  // ÊØÈíŞ ÓíÇÓÉ CORS

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
