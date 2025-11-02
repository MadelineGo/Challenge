using AutoMapper;
using ChallengeApp.Application.DTOs;
using ChallengeApp.Application.Elastic;
using ChallengeApp.Domain.Events;
using ChallengeApp.Domain.Exceptions;
using ChallengeApp.Domain.Repositories;
using MediatR;

namespace ChallengeApp.Application.Commands.ModifyPermission;

public class ModifyPermissionHandler(IUnitOfWork unitOfWork, IMapper  mapper, IElasticsearchService<ElasticPermissionDto> elasticsearchService) : IRequestHandler<ModifyPermissionCommand>
{
    public async Task Handle(ModifyPermissionCommand request, CancellationToken cancellationToken)
    {
        var permission = await unitOfWork.PermissionsRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"The permission '{request.Id}' was not found");
        
        mapper.Map(request, permission);
        permission.AddDomainEvent(new ModifyPermissionEvent(permission));
        unitOfWork.PermissionsRepository.Update(permission);
        
        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        var elasticPermissionDto = mapper.Map<ElasticPermissionDto>(permission);
        
        await elasticsearchService.RequestOrModify(permission.Id, elasticPermissionDto);
        
    }
}
