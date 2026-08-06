using FluentValidation;
using AssignmentManagement.Api.Application.DTOs.Admin;

namespace AssignmentManagement.Api.Application.Validators.Admin;

public class CreateClassValidator : AbstractValidator<CreateClassDto>
{
    public CreateClassValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Class name is required.")
            .MaximumLength(100).WithMessage("Class name must not exceed 100 characters.");

        RuleFor(x => x.AcademicYear)
            .NotEmpty().WithMessage("Academic year is required.")
            .MaximumLength(20)
            .Matches(@"^\d{4}-\d{4}$").WithMessage("Academic year must be in format YYYY-YYYY (e.g., 2025-2026).");

        RuleFor(x => x.Description)
            .MaximumLength(500).When(x => x.Description is not null);
    }
}

public class UpdateClassValidator : AbstractValidator<UpdateClassDto>
{
    public UpdateClassValidator()
    {
        RuleFor(x => x.Name)
            .MaximumLength(100).When(x => x.Name is not null);

        RuleFor(x => x.AcademicYear)
            .MaximumLength(20)
            .Matches(@"^\d{4}-\d{4}$").WithMessage("Academic year must be in format YYYY-YYYY.")
            .When(x => x.AcademicYear is not null);

        RuleFor(x => x.Description)
            .MaximumLength(500).When(x => x.Description is not null);
    }
}
