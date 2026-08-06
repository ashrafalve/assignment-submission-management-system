using FluentValidation;
using AssignmentManagement.Api.Application.DTOs.Teacher;

namespace AssignmentManagement.Api.Application.Validators.Teacher;

public class CreateAssignmentValidator : AbstractValidator<CreateAssignmentDto>
{
    public CreateAssignmentValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required.")
            .MaximumLength(2000).WithMessage("Description must not exceed 2000 characters.");

        RuleFor(x => x.DueDate)
            .NotEmpty().WithMessage("Due date is required.")
            .GreaterThan(DateTime.UtcNow).WithMessage("Due date must be in the future.");

        RuleFor(x => x.TotalMarks)
            .GreaterThan(0).WithMessage("Total marks must be greater than zero.");

        RuleFor(x => x.ClassId)
            .NotEmpty().WithMessage("Class selection is required.");

        RuleFor(x => x.SubjectId)
            .NotEmpty().WithMessage("Subject selection is required.");
    }
}

public class UpdateAssignmentValidator : AbstractValidator<UpdateAssignmentDto>
{
    public UpdateAssignmentValidator()
    {
        RuleFor(x => x.Title)
            .MaximumLength(200).When(x => x.Title is not null);

        RuleFor(x => x.Description)
            .MaximumLength(2000).When(x => x.Description is not null);

        RuleFor(x => x.DueDate)
            .GreaterThan(DateTime.UtcNow).When(x => x.DueDate.HasValue)
            .WithMessage("Due date must be in the future.");

        RuleFor(x => x.TotalMarks)
            .GreaterThan(0).When(x => x.TotalMarks.HasValue)
            .WithMessage("Total marks must be greater than zero.");
    }
}
