using ChallengeApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChallengeApp.Infrastructure.Data.Configuration;

public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {   
        builder.ToTable("Permissions");
        builder
            .HasKey(x => x.Id)
            .HasName("PK_PermissionId");
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.EmployeeName).IsRequired();
        builder.Property(x => x.EmployeeSurname).IsRequired();
        builder.Property(x => x.PermissionTypeId).IsRequired();
        builder.Property(x => x.CreatedDate).IsRequired();
        
        builder
            .HasOne(permission => permission.PermissionType)
            .WithMany(type => type.Permissions)
            .HasForeignKey(permission => permission.PermissionTypeId);
       
    }
    
}