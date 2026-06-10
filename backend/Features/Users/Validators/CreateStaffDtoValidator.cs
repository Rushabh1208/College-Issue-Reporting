using FluentValidation;

namespace backend.Features.Users.Validators
{
    public class CreateStaffDtoValidator : AbstractValidator<CreateStaffDto>
    {
        public CreateStaffDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MinimumLength(2).WithMessage("Name must be at least 2 characters long.")
                .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("A valid email is required.");
        }
    }
}
