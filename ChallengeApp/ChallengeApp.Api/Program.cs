using ChallengeApp.Api.Exception;
using ChallengeApp.Api.ExceptionHandler;
using ChallengeApp.Application;
using ChallengeApp.Infrastructure.Data;
using ChallengeApp.Infrastructure;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssemblyContaining<ChallengeApp.Api.Validators.CreatePermissionRequestValidator>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddApplicationLayer();
builder.Services.AddInfrastructureLayer(builder.Configuration);

builder.Services.AddExceptionHandler<NotFoundExceptionHandler>();
builder.Services.AddExceptionHandler<BadRequestExceptionHandler>();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddCors(opt =>
{
    opt.AddDefaultPolicy(builder =>
    {
        builder
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowAnyOrigin();
    });
});

var app = builder.Build();
// Configure the HTTP request pipeline.
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthorization();
app.UseExceptionHandler();
app.MapControllers();

await Program.MigrateDatabaseAsync(app.Services);

app.Run();

public partial class Program
{
    public static async Task MigrateDatabaseAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("DatabaseMigration");

        if (!context.Database.IsRelational())
        {
            logger.LogInformation("Skipping migrations because the database provider is non-relational.");
            return;
        }

        var retryCount = 0;
        const int maxRetry = 5;
        while (true)
        {
            try
            {
                await context.Database.MigrateAsync();
                break;
            }
            catch (Exception ex) when (retryCount < maxRetry)
            {
                retryCount++;
                logger.LogWarning(ex, "Error applying migrations. Retrying {Retry}/{MaxRetry}...", retryCount, maxRetry);
                await Task.Delay(TimeSpan.FromSeconds(5));
            }
        }
    }
}
