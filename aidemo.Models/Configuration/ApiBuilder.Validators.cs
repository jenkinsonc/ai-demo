using aidemo.Models.Validators;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace aidemo.Models.Configuration
{
    public static partial class ApiBuilder
    {
        public static void AddValidators(this IServiceCollection services)
        {
            ArgumentNullException.ThrowIfNull(services);
            services.AddValidatorsFromAssemblyContaining<SampleRequestValidator>(); // Claude: replace with actual validator class
        }
    }
}
