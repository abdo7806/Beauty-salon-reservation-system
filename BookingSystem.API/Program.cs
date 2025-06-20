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

// ����� CORS ������ ������� �����
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin()  // ������ ��� ����
                  .AllowAnyMethod()  // ������ ��� ����� (GET, POST, PUT, DELETE)
                  .AllowAnyHeader(); // ������ ��� ��� (headers)
        });
});


// ����� �������
builder.Services.AddControllers();
// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// ����� ����� �������� �������� JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        // ����� ������ ������ �� ��� ������
        options.TokenValidationParameters = new TokenValidationParameters
        {
            // ���� ��� ��� ��� ������� ������ ������
            ValidateIssuer = true,
            // ���� ��� ��� ��� ������� �������� ������ ������
            ValidateAudience = true,
            // ���� ��� ��� ���� ������ ������ �� �����
            ValidateLifetime = true,
            // ���� ��� ��� ��� ����� ������� ������
            ValidateIssuerSigningKey = true,
            // ����� ������� ������ ��� ������ ����� �� ��� ���������
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            // ������� �������� ���� ������ ������ ��� ������ ���� �� ��� ���������
            ValidAudience = builder.Configuration["Jwt:Audience"],
            // ������� ����� ���� ������ ������ ������ ��� ������ ��� ������ ����
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

builder.Services.AddScoped<IServiceRepository, ServiceRepository>();
builder.Services.AddScoped<IServiceService, ServiceService>();


builder.Services.AddScoped<IAvailabilityRepository, AvailabilityRespository>();
builder.Services.AddScoped<IAvailabilityService, AvailabilityServices>();



builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();
builder.Services.AddScoped<IAppointmentService, AppointmentService>();


// ����� ����� Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();


// ����� Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
        c.RoutePrefix = string.Empty; // ���� ������� ��� �����
    });
    app.MapOpenApi(); // ���� �� ��� ���� ������ MapOpenApi ���

}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();  // ���� �� �� Swagger ���� ���� ����

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

// ��ǡ ���� �� ��� ������ CORS ����� ��� �������� ���� Authorization
app.UseCors("AllowAll");  // ����� ����� CORS

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
