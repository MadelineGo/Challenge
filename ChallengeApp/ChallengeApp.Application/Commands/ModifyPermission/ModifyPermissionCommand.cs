using MediatR;

namespace ChallengeApp.Application.Commands.ModifyPermission;

public record ModifyPermissionCommand(int Id,
                                        string? EmployeeName,
                                        string? EmployeeSurname,
                                        int? PermissionTypeId) : IRequest;