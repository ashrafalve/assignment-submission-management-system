using FluentValidation;
using AssignmentManagement.Api.Application.DTOs.Admin;

namespace AssignmentManagement.Api.Application.Validators.Admin;

public class CreateSubjectValidator : AbstractValidator<CreateSubjectDto>
{
    public CreateSubjectValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Subject name is required.")
            .MaximumLength(100).WithMessage("Subject name must not exceed 100 characters.");

        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Subject code is required.")
            .MaximumLength(20).WithMessage("Subject code must not exceed 20 characters.")
            .Matches(@"^[A-Z0-9\-]+$").WithMessage("Subject code must contain only uppercase letters, digits, and hyphens.");

        RuleFor(x => x.Description)
            .MaximumLength(500).When(x => x.Description is not null);
    }
}

public class UpdateSubjectValidator : AbstractValidator<UpdateSubjectDto>
{
    public UpdateSubjectValidator()
    {
        RuleFor(x => x.Name)
            .MaximumLength(100).When(x => x.Name is not null);

        RuleFor(x => x.Code)
            .MaximumLength(20)
            .Matches(@"^[A-Z0-9\-]+$").WithMessage("Subject code must contain only uppercase letters, digits, and hyphens.")
            .When(x => x.Code is not null);

        RuleFor(x => x.Description)
            .MaximumLength(500).When(x => x.Description is not null);
    }
}
