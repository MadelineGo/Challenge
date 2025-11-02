using ChallengeApp.Api.Contracts.Requests;
using FluentValidation;

namespace ChallengeApp.Api.Validators;

public class UpdatePermissionRequestValidator : AbstractValidator<UpdatePermissionRequest>
{
    public UpdatePermissionRequestValidator()
    {
        RuleFor(x => x.EmployeeName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.EmployeeSurname)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.PermissionTypeId)
            .NotNull()
            .GreaterThan(0);
    }
}
