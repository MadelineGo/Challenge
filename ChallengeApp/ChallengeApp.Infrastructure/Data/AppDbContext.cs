using Microsoft.EntityFrameworkCore;
using System.Reflection;
namespace ChallengeApp.Infrastructure.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}

//dotnet ef migrations add InitialDB -c AppDbContext -p ChallengeApp.Infrastructure -s ChallengeApp.Api -o Data/Migrations
//dotnet ef database update -c AppDbContext -p ChallengeApp.Infrastructure -s ChallengeApp.Api
//dotnet ef migrations remove -c AppDbContext -p ChallengeApp.Infrastructure -s ChallengeApp.Api  
