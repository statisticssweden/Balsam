using Balsam.Api;
using Balsam.Api.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<HubRepositoryOptions>(builder.Configuration.GetSection(HubRepositoryOptions.SectionName));


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<HttpClient>();
builder.Services.AddTransient<HubClient>();
builder.Services.AddSingleton<HubRepositoryClient>();
builder.Services.AddTransient<S3Client>();
builder.Services.AddTransient<GitClient>();
builder.Services.AddMemoryCache();

builder.Services.Configure<CapabilityOptions>(Capabilities.Git, builder.Configuration.GetSection($"Capabilities:{Capabilities.Git}"));
builder.Services.Configure<CapabilityOptions>(Capabilities.S3, builder.Configuration.GetSection($"Capabilities:{Capabilities.S3}"));
builder.Services.Configure<CapabilityOptions>(Capabilities.Authentication, builder.Configuration.GetSection($"Capabilities:{Capabilities.Authentication}"));


builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme; // OpenIdConnectDefaults.AuthenticationScheme;
}).AddJwtBearer(cfg =>
{
    cfg.RequireHttpsMetadata = false;
    cfg.SaveToken = true;

    cfg.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateIssuerSigningKey = false,
        ValidateAudience = false,
    };

    cfg.Events = new JwtBearerEvents()
    {
        OnTokenValidated = c =>
        {
            Console.WriteLine("User successfully authenticated");
            return Task.CompletedTask;
        },
        OnForbidden = c =>
        {
            Console.WriteLine("Forbidden");
            return c.Response.WriteAsync("ok");
        },
        OnAuthenticationFailed = c =>
        {
            Console.WriteLine("Authentication failed");

            c.NoResult();

            c.Response.StatusCode = 401;
            c.Response.ContentType = "text/plain";

            return c.Response.WriteAsync(c.Exception.ToString());
        }
    };
    cfg.Authority = builder.Configuration.GetSection($"Authentication:Authority").Value;
});

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

app.Run();
