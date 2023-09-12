using Balsam.Api;
using Balsam.Api.Models;
using Microsoft.Extensions.Configuration;

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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
