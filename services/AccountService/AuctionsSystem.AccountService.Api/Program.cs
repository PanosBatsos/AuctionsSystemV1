using AuctionsSystem.AccountService.Api.ExceptionHandling;
using AuctionsSystem.AccountService.Application;
using AuctionsSystem.AccountService.Infrastructure;
using AuctionsSystem.AccountService.Infrastructure.Configuration;
using DotNetEnv;
using Microsoft.OpenApi;

var envPath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "..", ".env"));
Env.Load(envPath);

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Paste Access Token"
    });

    var securityRequirement = new OpenApiSecurityRequirement();
    securityRequirement.Add(new OpenApiSecuritySchemeReference("Bearer"), new List<string>());

    options.AddSecurityRequirement(_ => securityRequirement);
});



builder.Services.AddExceptionHandler<ValidationExceptionHandler>();
builder.Services.AddExceptionHandler<PropertyAlreadyInUseExceptionHandler>();
builder.Services.AddExceptionHandler<AuthenticationExceptionHandler>();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddProblemDetails();

var app = builder.Build();

app.UseExceptionHandler();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();
