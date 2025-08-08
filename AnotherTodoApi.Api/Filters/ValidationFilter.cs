using FluentValidation;

namespace AnotherTodoApi.Api.Filters;

public class ValidationFilter<T> : IEndpointFilter where T : class
{
    private readonly IValidator<T> _validator;

    public ValidationFilter(IValidator<T> validator)
    {
        _validator = validator;
    }

    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context, 
        EndpointFilterDelegate next)
    {
        var argToValidate = context.Arguments
            .FirstOrDefault(x => x?.GetType() == typeof(T));
        
        if (argToValidate is T objectToValidate)
        {
            var validationResult = await _validator.ValidateAsync(objectToValidate);
            
            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }
        }

        return await next(context);
    }
}