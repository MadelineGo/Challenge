using AutoMapper;
using ChallengeApp.Domain.Entities;

namespace ChallengeApp.Application.Commands.ModifyPermission;

public class ModifyPermissionMapper: Profile
{
    public ModifyPermissionMapper()
    {
        CreateMap<ModifyPermissionCommand, Permission>().ConvertUsing(new ModifyPermissionTypeConverter());
    }
    
    private class ModifyPermissionTypeConverter : ITypeConverter<ModifyPermissionCommand, Permission>
    {
        public Permission Convert(ModifyPermissionCommand source, Permission destination, ResolutionContext context)
        {
            if (!string.IsNullOrEmpty(source.EmployeeName))
                destination.EmployeeName = source.EmployeeName;
            if (!string.IsNullOrEmpty(source.EmployeeSurname))
                destination.EmployeeSurname = source.EmployeeSurname;
            if (source.PermissionTypeId.HasValue)
                destination.PermissionTypeId = source.PermissionTypeId.Value;

            return destination;
        }
    }
    
}