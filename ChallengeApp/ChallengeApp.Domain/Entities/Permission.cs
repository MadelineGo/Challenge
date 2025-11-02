using ChallengeApp.Domain.Primitives;

namespace ChallengeApp.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
public class Permission : BaseEntity, IAuditableEntity
{
    public int Id { get; set; }
    public required string EmployeeName { get; set; }
    public required string EmployeeSurname { get; set; }
    public required int PermissionTypeId { get; set; }
    public PermissionType PermissionType { get; set; }
    public DateTimeOffset CreatedDate { get; set; }
    public DateTimeOffset LastModifiedDate { get; set; }
}