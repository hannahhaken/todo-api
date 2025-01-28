using FluentValidation;

namespace AnotherTodoApi;

public class TodoItemDtoValidator : AbstractValidator<TodoItemDto>
{
    public TodoItemDtoValidator()
    {
        RuleFor(dto => dto.Name)
            .NotEmpty().WithMessage("Name is required.")
            .Length(5, 100).WithMessage("Name must be between 5 and 100 characters.");

        RuleFor(dto => dto.IsComplete)
            .NotNull().WithMessage("IsComplete is required.");
    }
}