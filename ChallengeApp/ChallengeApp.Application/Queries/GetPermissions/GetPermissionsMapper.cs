using AutoMapper;
using ChallengeApp.Application.DTOs;
using ChallengeApp.Domain.Entities;

namespace ChallengeApp.Application.Queries.GetPermissions;

public class GetPermissionsMapper : Profile
{
    public GetPermissionsMapper()
    {
        CreateMap<Permission, GetPermissionDto>()
            .ConstructUsing(src => new GetPermissionDto(
              src.Id,
              src.EmployeeName,
              src.EmployeeSurname,
              src.PermissionTypeId,
              src.PermissionType != null ? src.PermissionType.Description : string.Empty,
              src.CreatedDate,
              src.LastModifiedDate));
    }
}