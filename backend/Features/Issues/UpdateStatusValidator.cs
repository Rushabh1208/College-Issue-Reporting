using FluentValidation;
using backend.Features.Issues;

public class UpdateStatusDtoValidator : AbstractValidator<UpdateStatusDto>
{
    public UpdateStatusDtoValidator()
    {
        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Invalid status value");
    }
}
