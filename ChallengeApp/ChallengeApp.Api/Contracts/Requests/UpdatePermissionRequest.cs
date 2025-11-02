using System.ComponentModel.DataAnnotations;

namespace ChallengeApp.Api.Contracts.Requests;

public class UpdatePermissionRequest
{
    [Required]
    public string EmployeeName { get; set; } = string.Empty;

    [Required]
    public string EmployeeSurname { get; set; } = string.Empty;

    [Required]
    [Range(1, int.MaxValue)]
    public int? PermissionTypeId { get; set; }
}
