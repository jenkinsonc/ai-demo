using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace aidemo.Endpoints.Filters
{
    public sealed class ValidationFilter<T> : IEndpointFilter where T : class
    {
        public async ValueTask<object> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
        {
            var argument = (T)context.Arguments.FirstOrDefault(a => a is T);

            var validator = (IValidator<T>)context.HttpContext.RequestServices.GetRequiredService(typeof(IValidator<T>));

            var result = argument is null
                ? new ValidationResult([new ValidationFailure("body", "Request body is required.")])
                : await validator.ValidateAsync(argument);

            return result.IsValid
                ? await next(context)
                : TypedResults.ValidationProblem(result.ToDictionary());
        }
    }
}
