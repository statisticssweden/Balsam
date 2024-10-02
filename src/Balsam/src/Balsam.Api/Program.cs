using Balsam.Api;
using Balsam.Interfaces;
using Balsam.Model;
using Balsam.Repositories;
using Balsam.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<HubRepositoryOptions>(builder.Configuration.GetSection(HubRepositoryOptions.SectionName));


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<HttpClient>();
builder.Services.AddTransient<IProjectService, ProjectService>(); //TODO: Use AddScoped ?
builder.Services.AddTransient<IWorkspaceService, WorkspaceService>(); //TODO: Use AddScoped ?
builder.Services.AddSingleton<IHubRepositoryClient, HubRepositoryClient>();
builder.Services.AddSingleton<IKnowledgeLibraryService, KnowledgeLibraryService>();

builder.Services.AddTransient<IWorkspaceRepository, WorkspaceRepository>();
builder.Services.AddTransient<IWorkspaceGitOpsRepository, WorkspaceGitOpsRepository>();
builder.Services.AddSingleton<IKnowledgeLibraryContentRepository, KnowledgeLibraryContentRepository>();
builder.Services.AddSingleton<IKnowledgeLibraryRepository, KnowledgeLibraryRepository>();
builder.Services.AddTransient<IProjectRepository, ProjectRepository>();
builder.Services.AddTransient<IProjectGitOpsRepository, ProjectGitOpsRepository>();

builder.Services.AddMemoryCache();
builder.Services.AddSingleton<GitProviderApiClient.Api.IRepositoryApi>(
    new GitProviderApiClient.Api.RepositoryApi(
        builder.Configuration.GetSection($"Capabilities:{Capabilities.Git}:ServiceLocation").Value??"git-provider.balsam-system.svc.cluster.local"));
builder.Services.AddSingleton<GitProviderApiClient.Api.IUserApi>(
    new GitProviderApiClient.Api.UserApi(
        builder.Configuration.GetSection($"Capabilities:{Capabilities.Git}:ServiceLocation").Value ?? "git-provider.balsam-system.svc.cluster.local"));

builder.Services.AddSingleton<S3ProviderApiClient.Api.IBucketApi>(
    new S3ProviderApiClient.Api.BucketApi(
        builder.Configuration.GetSection($"Capabilities:{Capabilities.S3}:ServiceLocation").Value ?? "s3-provider.balsam-system.svc.cluster.local"));

builder.Services.AddSingleton<OidcProviderApiClient.Api.IGroupApi>(
    new OidcProviderApiClient.Api.GroupApi(
        builder.Configuration.GetSection($"Capabilities:{Capabilities.Authentication}:ServiceLocation").Value ?? "oidc-provider.balsam-system.svc.cluster.local"));

builder.Services.AddSingleton<RocketChatChatProviderApiClient.Api.IAreaApi>(
    new RocketChatChatProviderApiClient.Api.AreaApi(
        builder.Configuration.GetSection($"Capabilities:{Capabilities.Chat}:ServiceLocation").Value ?? "chat-provider.balsam-system.svc.cluster.local"));

builder.Services.Configure<CapabilityOptions>(Capabilities.Git, builder.Configuration.GetSection($"Capabilities:{Capabilities.Git}"));
builder.Services.Configure<CapabilityOptions>(Capabilities.S3, builder.Configuration.GetSection($"Capabilities:{Capabilities.S3}"));
builder.Services.Configure<CapabilityOptions>(Capabilities.Authentication, builder.Configuration.GetSection($"Capabilities:{Capabilities.Authentication}"));
builder.Services.Configure<CapabilityOptions>(Capabilities.Chat, builder.Configuration.GetSection($"Capabilities:{Capabilities.Chat}"));

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy =>
        {
            policy.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

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

app.UseHttpLogging();

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();


