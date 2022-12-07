﻿using System.Reflection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using OilCaseApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using OilCaseApi.Controllers.Api;
using OilCaseApi.hub;
using Microsoft.OpenApi.Models;
using OilCaseApi.Controllers.Api.UserData.Authorization;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

var connectionStringName = "OilCaseApiContextConnection";
string url;
switch (builder.Environment.EnvironmentName)
{
    case "Development":
        connectionStringName += "Development";
        url = "https://localhost:16745";
        break;

    case "Staging":
        connectionStringName += "Staging";
        url = "http://*:8080";
        break;

    default:
        connectionStringName += "Production";
        url = "http://*:8080";
        break;
}

ApplicationContext.ConnectionString = builder.Configuration.GetConnectionString(connectionStringName);

builder.Services.AddDbContext<ApplicationContext>(options => options.UseSqlServer(ApplicationContext.ConnectionString));

builder.WebHost.UseUrls(url);


builder.Services.AddIdentity<User, IdentityRole>(opt =>
{
    opt.Password.RequireNonAlphanumeric = false;
    opt.Password.RequireUppercase = false;
    opt.Password.RequireLowercase = false;
    opt.Password.RequireDigit = false;
}).AddEntityFrameworkStores<ApplicationContext>();

builder.Services.AddMvc(options => options.EnableEndpointRouting = false);
builder.Services.AddRazorPages();
builder.Services.AddSignalR();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        builder => {
            builder.WithOrigins("http://localhost:8080")
                .AllowAnyHeader()
                .WithMethods("GET", "POST", "PUT", "DELETE", "PATCH")
                .AllowCredentials();
        });
});

builder.Services.AddControllers();
builder.Services.AddControllersWithViews();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        IssuerSigningKey = JwtGenerator.GetSymmetricSecurityKey(),
        ValidateIssuerSigningKey = true
    };
});
builder.Services.AddAuthorization();
builder.Services.AddScoped<IJwtGenerator, JwtGenerator>();
builder.Services.Configure<IISServerOptions>(options => { options.MaxRequestBodySize = int.MaxValue; });

builder.Services.AddSwaggerGen(options => {
    options.SwaggerDoc("v1", new OpenApiInfo()
    {
        Version = "v1",
        Title = "OilCase Api",
    });
    var xmlFileName = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFileName));
});

var app = builder.Build();

var loggerFactory = app.Services.GetService<ILoggerFactory>();
loggerFactory.AddFile(builder.Configuration["Logging:LogFilePath"].ToString()
                      , outputTemplate: " [{Level:u3}] {SourceContext} {Message} ({EventId:x8}){NewLine}{Exception}");

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseDefaultFiles();
app.MapRazorPages();

app.UseRouting();

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.MapRazorPages();
app.UseMvc();

app.MapHub<MouseHub>("/MouseHub");
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

public partial class Program { }