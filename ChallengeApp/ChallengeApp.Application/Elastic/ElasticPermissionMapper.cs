using AutoMapper;
using ChallengeApp.Application.DTOs;
using ChallengeApp.Domain.Entities;

namespace ChallengeApp.Application.Elastic;

public class ElasticPermissionMapper : Profile
{
    public ElasticPermissionMapper()
    {
        CreateMap<Permission, ElasticPermissionDto>();
    }
}