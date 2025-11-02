using AutoMapper;
using ChallengeApp.Application.DTOs;
using ChallengeApp.Domain.Entities;
using ChallengeApp.Domain.Enums;
using ChallengeApp.Domain.Primitives;
using ChallengeApp.Domain.Repositories;
using MediatR;

namespace ChallengeApp.Application.Queries.GetPermissions;

public record GetPermissionsQuery(int PageNumber = 1, int PageSize = 10) : IRequest<GetPermissionsResponse>;
public record GetPermissionsResponse(IEnumerable<GetPermissionDto> Permissions, int TotalCount);

public class GetPermissionsHandler (IUnitOfWork unitOfWork, IMapper mapper, IEventBus bus) : IRequestHandler<GetPermissionsQuery, GetPermissionsResponse>
{
    private readonly IMapper _mapper = mapper;
    private readonly IEventBus _bus = bus;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    
    public async Task<GetPermissionsResponse> Handle(GetPermissionsQuery request, CancellationToken cancellationToken)
    {
        var (permissions, totalCount) = await _unitOfWork.PermissionsRepository.GetAllAsync(request.PageNumber, request.PageSize, cancellationToken);
        IEnumerable<GetPermissionDto> permissionsDto;
        await _bus.Publish(new EventMessage(Operation.Get));
        try
        {
         permissionsDto = _mapper.Map<IEnumerable<GetPermissionDto>>(permissions);
        }
        catch (AutoMapperMappingException ex)
        {
            Console.WriteLine($"AutoMapper Error: {ex.StackTrace}");
            throw;
        }
        return new GetPermissionsResponse(permissionsDto, totalCount);
    }
}