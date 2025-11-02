using AutoMapper;
using ChallengeApp.Application.DTOs;
using ChallengeApp.Application.Elastic;
using ChallengeApp.Domain.Entities;
using ChallengeApp.Domain.Events;
using ChallengeApp.Domain.Repositories;
using MediatR;

namespace ChallengeApp.Application.Commands.RequestPermission;

public class RequestPermissionHandler(IUnitOfWork unitOfWork, IMapper mapper, IElasticsearchService<ElasticPermissionDto> elasticsearchService) : IRequestHandler<RequestPermissionCommand, int>
{
    public async Task<int> Handle(RequestPermissionCommand request, CancellationToken cancellationToken)
    {
        var permission = mapper.Map<Permission>(request.Permission);
        permission.AddDomainEvent(new RequestPermissionEvent(permission));
        await unitOfWork.PermissionsRepository.AddAsync(permission, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        var elasticPermissionDto = mapper.Map<ElasticPermissionDto>(permission);
        
        await elasticsearchService.RequestOrModify(permission.Id, elasticPermissionDto);
        
        return permission.Id;
    }
}