using FluentValidation;
using AssignmentManagement.Api.Application.DTOs.Student;

namespace AssignmentManagement.Api.Application.Validators.Student;

public class SubmitAssignmentValidator : AbstractValidator<SubmitAssignmentDto>
{
    public SubmitAssignmentValidator()
    {
        RuleFor(x => x.AssignmentId)
            .NotEmpty().WithMessage("Assignment ID is required.");

        RuleFor(x => x)
            .Must(x => !string.IsNullOrWhiteSpace(x.Content) || !string.IsNullOrWhiteSpace(x.FilePath))
            .WithMessage("Submission must contain either text content or a file attachment.");

        RuleFor(x => x.Content)
            .MaximumLength(5000).When(x => x.Content is not null)
            .WithMessage("Submission text content must not exceed 5000 characters.");

        RuleFor(x => x.FilePath)
            .MaximumLength(500).When(x => x.FilePath is not null)
            .WithMessage("File path must not exceed 500 characters.");
    }
}

public class UpdateSubmissionValidator : AbstractValidator<UpdateSubmissionDto>
{
    public UpdateSubmissionValidator()
    {
        RuleFor(x => x)
            .Must(x => !string.IsNullOrWhiteSpace(x.Content) || !string.IsNullOrWhiteSpace(x.FilePath))
            .WithMessage("Submission must contain either text content or a file attachment.");

        RuleFor(x => x.Content)
            .MaximumLength(5000).When(x => x.Content is not null)
            .WithMessage("Submission text content must not exceed 5000 characters.");

        RuleFor(x => x.FilePath)
            .MaximumLength(500).When(x => x.FilePath is not null)
            .WithMessage("File path must not exceed 500 characters.");
    }
}
