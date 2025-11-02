using AutoMapper;
using ChallengeApp.Application.DTOs;
using ChallengeApp.Domain.Entities;

namespace ChallengeApp.Application.Queries.GetPermissionsType;

public class GetPermissionsTypeMapper : Profile
{
    public GetPermissionsTypeMapper()
    {
        CreateMap<IEnumerable<PermissionType>, IEnumerable<GetPermissionTypeDto>>().ConvertUsing(new GetPermissionsTypeTypeConverter());
    }
    
    private class GetPermissionsTypeTypeConverter : ITypeConverter<IEnumerable<PermissionType>, IEnumerable<GetPermissionTypeDto>>
    {
        public IEnumerable<GetPermissionTypeDto> Convert(IEnumerable<PermissionType> source, IEnumerable<GetPermissionTypeDto> destination, ResolutionContext context)
        {
            return source
                .Select(type => new GetPermissionTypeDto(type.Id,
                    type.Description));
        }
    }
}