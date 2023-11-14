using System;
using LcsServer.CommandLayer;
using LcsServer.Controllers;
using LcsServer.DatabaseLayer;
using LcsServer.DevicePollingService;
using LcsServer.DevicePollingService.Interfaces;
using LcsServer.DevicePollingService.Models;
using LcsServer.Models.LCProjectModels.Managers;
using LCSVersionControl;
using LightControlServiceV._2.DevicePollingService.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

var webApplicationOptions = new WebApplicationOptions()
{
    ContentRootPath = AppContext.BaseDirectory,
    Args = args,
    ApplicationName = System.Diagnostics.Process.GetCurrentProcess().ProcessName,
    WebRootPath = "wwwroot",

};
var builder = WebApplication.CreateBuilder(args);
builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true); //load local settings
builder.Host.UseWindowsService();
builder.WebHost.UseKestrel();
// Add services to the container.
builder.Services.AddDbContext<DatabaseContext>(ServiceLifetime.Scoped);//AddSingleton<DesignTimeDbContextFactory>();
builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            // указывает, будет ли валидироваться издатель при валидации токена
            ValidateIssuer = true,
            // строка, представляющая издателя
            ValidIssuer = AuthOptions.ISSUER,
            // будет ли валидироваться потребитель токена
            ValidateAudience = true,
            // установка потребителя токена
            ValidAudience = AuthOptions.AUDIENCE,
            // будет ли валидироваться время существования
            ValidateLifetime = true,
            // установка ключа безопасности
            IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
            // валидация ключа безопасности
            ValidateIssuerSigningKey = true,
        };
    });
builder.Services.AddSingleton<ISerializationManagerRDM, JsonSerializationManagerRDM>();
builder.Services.AddSingleton<IStatusChecker, StatusChecker>();
builder.Services.AddSingleton<IStorageManager, StorageManager>();
builder.Services.AddSingleton<IBackgroundTaskQueue>(_ =>
{
    if (!int.TryParse(builder.Configuration.GetValue<string>("QueueCapacity"),
            out int queueCapacity))
    {
        queueCapacity = 100;
    }

    return new DefaultBackgroundTaskQueue(queueCapacity);
});
builder.Services.AddSingleton<DevicePollService>();
//builder.Services.AddSingleton<BaseVC>();
builder.Services.AddSingleton<JsonSerializationManager>();
builder.Services.AddSingleton<VersionControlManager>();
builder.Services.AddSingleton<RasterManager>();
builder.Services.AddHostedService<BackgroundDevicePolling>();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        creator =>
        {
            creator.WithOrigins("https://127.0.0.1:5173")
                .AllowAnyHeader()
                .WithMethods("GET", "POST")
                .AllowCredentials();
        });
});

var app = builder.Build();
app.UseCors();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication(); 
app.UseAuthorization();  

//app.MapMapControllers();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
