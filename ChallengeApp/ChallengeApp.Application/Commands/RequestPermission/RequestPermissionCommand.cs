using ChallengeApp.Application.DTOs;
using MediatR;

namespace ChallengeApp.Application.Commands.RequestPermission;

public record RequestPermissionCommand(PermissionDto Permission ) : IRequest<int>;