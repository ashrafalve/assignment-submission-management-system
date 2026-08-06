using FluentValidation;
using AssignmentManagement.Api.Application.DTOs.Admin;
using AssignmentManagement.Api.Domain.Enums;

namespace AssignmentManagement.Api.Application.Validators.Admin;

public class CreateUserAdminValidator : AbstractValidator<CreateUserDto>
{
    private static readonly string[] ValidRoles = Enum.GetNames(typeof(UserRole));

    public CreateUserAdminValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required.")
            .MaximumLength(50).WithMessage("First name must not exceed 50 characters.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required.")
            .MaximumLength(50).WithMessage("Last name must not exceed 50 characters.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("A valid email address is required.")
            .MaximumLength(256);

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters.");

        RuleFor(x => x.Role)
            .Must(r => ValidRoles.Contains(r, StringComparer.OrdinalIgnoreCase))
            .WithMessage($"Role must be one of: {string.Join(", ", ValidRoles)}.");
    }
}

public class UpdateUserAdminValidator : AbstractValidator<UpdateUserDto>
{
    private static readonly string[] ValidRoles = Enum.GetNames(typeof(UserRole));

    public UpdateUserAdminValidator()
    {
        RuleFor(x => x.FirstName)
            .MaximumLength(50).When(x => x.FirstName is not null);

        RuleFor(x => x.LastName)
            .MaximumLength(50).When(x => x.LastName is not null);

        RuleFor(x => x.Role)
            .Must(r => ValidRoles.Contains(r, StringComparer.OrdinalIgnoreCase))
            .When(x => x.Role is not null)
            .WithMessage($"Role must be one of: {string.Join(", ", ValidRoles)}.");
    }
}

public class ChangePasswordAdminValidator : AbstractValidator<ChangePasswordDto>
{
    public ChangePasswordAdminValidator()
    {
        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("New password is required.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters.")
            .Matches(@"[A-Z]").WithMessage("Must contain an uppercase letter.")
            .Matches(@"[a-z]").WithMessage("Must contain a lowercase letter.")
            .Matches(@"[0-9]").WithMessage("Must contain a digit.")
            .Matches(@"[\W_]").WithMessage("Must contain a special character.");

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty()
            .Equal(x => x.NewPassword).WithMessage("Passwords do not match.");
    }
}
