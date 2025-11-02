using ChallengeApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChallengeApp.Infrastructure.Data.Configuration;

public class PermissionTypeConfiguration : IEntityTypeConfiguration<PermissionType>
{
    public void Configure(EntityTypeBuilder<PermissionType> builder)
    {   
        builder.ToTable("PermissionType");
        builder.HasKey(x => x.Id).HasName("PK_PermissionTypeId");;
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        
        builder.HasData(
        [
            new PermissionType { Id = 1, Description = "Admin" },
            new PermissionType { Id = 2, Description = "Developer" },
        ]);
    }
}