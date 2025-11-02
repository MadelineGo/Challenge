using AutoMapper;
using ChallengeApp.Application.DTOs;
using ChallengeApp.Domain.Primitives;
using ChallengeApp.Domain.Repositories;
using MediatR;

namespace ChallengeApp.Application.Queries.GetPermissionsType;

public record GetPermissionsTypeQuery : IRequest<IEnumerable<GetPermissionTypeDto>>;

public class GetPermissionsTypeHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<GetPermissionsTypeQuery, IEnumerable<GetPermissionTypeDto>>
{
    private readonly IMapper _mapper = mapper;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    public async Task<IEnumerable<GetPermissionTypeDto>> Handle(GetPermissionsTypeQuery request, CancellationToken cancellationToken)
    {
        var permissionType = await _unitOfWork.PermissionsTypeRepository.GetAllAsync(cancellationToken);
        return mapper.Map<IEnumerable<GetPermissionTypeDto>>(permissionType);
    }
}