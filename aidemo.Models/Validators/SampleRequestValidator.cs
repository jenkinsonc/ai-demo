using aidemo.Models.ViewModels;
using FluentValidation;

namespace aidemo.Models.Validators
{
    // Claude: This is a sample validator for the SampleRequest model. Remove after you have created your own validators.
    public class SampleRequestValidator : AbstractValidator<SampleRequest>
    {
        public SampleRequestValidator()
        {
            RuleFor(r => r.Name)
                .Must(v => !string.IsNullOrWhiteSpace(v)).WithMessage("Name is required.")
                .Must(v => (v?.Trim().Length ?? 0) <= 200).WithMessage("Name must be 200 characters or fewer.");

            RuleFor(r => r.SomeValue)
                .Must(v => v > 0).WithMessage("Some value must be greater than 0.")
                .Must(v => v <= 100).WithMessage("Some value must be less than or equal to 100.");
        }
    }
}
