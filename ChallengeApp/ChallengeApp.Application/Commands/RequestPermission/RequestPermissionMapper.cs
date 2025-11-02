using AutoMapper;
using ChallengeApp.Application.DTOs;
using ChallengeApp.Domain.Entities;

namespace ChallengeApp.Application.Commands.RequestPermission;

public class RequestPermissionMapper : Profile
{
    public RequestPermissionMapper()
    {
        CreateMap<PermissionDto, Permission>();
    }
}