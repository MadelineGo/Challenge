namespace ChallengeApp.Application.DTOs;

public record ElasticPermissionDto(int Id,
                                   string EmployeeName,
                                   string EmployeeSurname,
                                   int PermissionTypeId,
                                   DateTimeOffset CreatedDate,
                                   DateTimeOffset LastModifiedDate
                                   );