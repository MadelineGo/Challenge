namespace ChallengeApp.Application.DTOs;

public record GetPermissionDto(int Id,
                                string? EmployeeName,
                                string? EmployeeSurname,
                                int? PermissionTypeId,
                                string? PermissionTypeDescription,
                                DateTimeOffset Created,
                                DateTimeOffset LastModified);