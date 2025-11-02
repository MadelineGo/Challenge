using ChallengeApp.Api.Contracts.Requests;
using FluentValidation;

namespace ChallengeApp.Api.Validators;

public class CreatePermissionRequestValidator : AbstractValidator<CreatePermissionRequest>
{
    public CreatePermissionRequestValidator()
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
