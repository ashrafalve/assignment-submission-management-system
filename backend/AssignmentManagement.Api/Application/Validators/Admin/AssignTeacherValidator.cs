using FluentValidation;
using AssignmentManagement.Api.Application.DTOs.Admin;

namespace AssignmentManagement.Api.Application.Validators.Admin;

public class AssignTeacherValidator : AbstractValidator<AssignTeacherDto>
{
    public AssignTeacherValidator()
    {
        RuleFor(x => x.TeacherId)
            .NotEmpty().WithMessage("Teacher ID is required.");

        RuleFor(x => x.SubjectId)
            .NotEmpty().WithMessage("Subject ID is required.");

        RuleFor(x => x.ClassId)
            .NotEmpty().WithMessage("Class ID is required.");
    }
}
