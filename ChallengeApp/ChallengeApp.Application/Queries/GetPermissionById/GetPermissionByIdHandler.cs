using AutoMapper;
using ChallengeApp.Application.DTOs;
using ChallengeApp.Domain.Enums;
using ChallengeApp.Domain.Exceptions;
using ChallengeApp.Domain.Primitives;
using ChallengeApp.Domain.Repositories;
using MediatR;

namespace ChallengeApp.Application.Queries.GetPermissionById;

public record GetPermissionQuery(int PermissionId) : IRequest<GetPermissionDto>;

public class GetPermissionByIdHandler(IUnitOfWork unitOfWork, IMapper mapper, IEventBus bus) : IRequestHandler<GetPermissionQuery, GetPermissionDto>
{
    public async Task<GetPermissionDto> Handle(GetPermissionQuery request, CancellationToken cancellationToken)
    {
        var permission = await unitOfWork.PermissionsRepository.GetByIdAsync(request.PermissionId, cancellationToken)
                         ?? throw new NotFoundException($"The permission '{request.PermissionId}' was not found");

        await bus.Publish(new EventMessage(Operation.Get));

        return mapper.Map<GetPermissionDto>(permission);
    }
}
