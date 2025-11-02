using ChallengeApp.Api.Contracts.Requests;
using ChallengeApp.Application.Commands.ModifyPermission;
using ChallengeApp.Application.Commands.RequestPermission;
using ChallengeApp.Application.DTOs;
using ChallengeApp.Application.Queries.GetPermissionById;
using ChallengeApp.Application.Queries.GetPermissions;
using ChallengeApp.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ChallengeApp.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PermissionsController(ISender mediator) : ControllerBase
{
    [HttpGet(Name = "GetPermissions")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetPermissionsResponse))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetPermissions([FromQuery] GetPermissionsQuery query)
    {
        var permissionsDto = await mediator.Send(query);
        return Ok(permissionsDto);
    }
    
    [HttpGet("{permissionId}", Name = nameof(GetPermission))]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetPermissionDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetPermission(int permissionId)
    {
        var permissionDto = await mediator.Send(new GetPermissionQuery(permissionId));

        return Ok(permissionDto);
    }
    
    [HttpPost(Name = nameof(RequestPermission))]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RequestPermission([FromBody] CreatePermissionRequest request)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var permissionDto = new PermissionDto(
            request.EmployeeName,
            request.EmployeeSurname,
            request.PermissionTypeId!.Value);

        var permissionId = await mediator.Send(new RequestPermissionCommand(permissionDto));

        return CreatedAtRoute(nameof(GetPermission), new { permissionId }, new { permissionId });
    }
    
    [HttpPut("{permissionId}", Name = nameof(ModifyPermissions))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ModifyPermissions(int permissionId, [FromBody] UpdatePermissionRequest request)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var command = new ModifyPermissionCommand(
            permissionId,
            request.EmployeeName,
            request.EmployeeSurname,
            request.PermissionTypeId);

        await mediator.Send(command);

        return NoContent();
    }
}
