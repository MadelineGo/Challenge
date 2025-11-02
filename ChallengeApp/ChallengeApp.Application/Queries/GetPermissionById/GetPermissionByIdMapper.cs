using AutoMapper;
using ChallengeApp.Application.DTOs;
using ChallengeApp.Domain.Entities;

namespace ChallengeApp.Application.Queries.GetPermissionById;

public class GetPermissionByIdMapper : Profile
{
    public GetPermissionByIdMapper()
    {
        CreateMap<PermissionDto, Permission>();
    }
}