using AnotherTodoApi.Api.Api.Requests;
using FluentValidation;

namespace AnotherTodoApi.Api.Validators;

public class TodoCreateRequestValidator : AbstractValidator<TodoCreateRequest>
{
    public TodoCreateRequestValidator()
    {
        RuleFor(dto => dto.Name)
            .NotEmpty().WithMessage("Name is required.")
            .Must(name => !string.IsNullOrWhiteSpace(name)).WithMessage("Name cannot be just whitespace.")
            .Length(5, 100).WithMessage("Name must be between 5 and 100 characters.");

        RuleFor(dto => dto.IsComplete)
            .NotNull().WithMessage("IsComplete is required.");
    }
}