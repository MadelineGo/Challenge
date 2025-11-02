using ChallengeApp.Application.DTOs;
using ChallengeApp.Application.Queries.GetPermissionsType;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ChallengeApp.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PermissionsTypeController(ISender mediator) : ControllerBase
{
    [HttpGet(Name = nameof(GetPermissionsType))]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<GetPermissionTypeDto>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetPermissionsType()
    {
        var permissionTypeDtos = await mediator.Send(new GetPermissionsTypeQuery());
        return Ok(permissionTypeDtos);
    }
}