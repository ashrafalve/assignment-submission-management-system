using FluentValidation;
using AssignmentManagement.Api.Application.DTOs.Teacher;
using AssignmentManagement.Api.Domain.Enums;

namespace AssignmentManagement.Api.Application.Validators.Teacher;

public class GradeSubmissionValidator : AbstractValidator<GradeSubmissionDto>
{
    public GradeSubmissionValidator()
    {
        RuleFor(x => x.MarksObtained)
            .GreaterThanOrEqualTo(0).WithMessage("Marks obtained cannot be negative.");

        RuleFor(x => x.Feedback)
            .MaximumLength(1000).When(x => x.Feedback is not null)
            .WithMessage("Feedback must not exceed 1000 characters.");

        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Invalid submission status.");
    }
}

public class ChangeSubmissionStatusValidator : AbstractValidator<ChangeSubmissionStatusDto>
{
    public ChangeSubmissionStatusValidator()
    {
        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Invalid submission status.");

        RuleFor(x => x.Feedback)
            .MaximumLength(1000).When(x => x.Feedback is not null)
            .WithMessage("Feedback must not exceed 1000 characters.");
    }
}
