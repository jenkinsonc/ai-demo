using FluentValidation.TestHelper;
using aidemo.Models.Validators;
using aidemo.Models.ViewModels;
using System.Threading.Tasks;

namespace aidemo.Tests.ValidatorTests
{
    // Claude: This is a sample class and should be removed when the SampleRequestValidator is removed.
    [TestClass]
    public sealed class SampleRequestValidatorUnitTests
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public async Task Validate_With_Whitespace_Only_Name_Fails_On_Name()
        {
            var request = ValidRequest();
            request.Name = "   ";

            var result = await CreateValidator().TestValidateAsync(request, cancellationToken: TestContext.CancellationToken);

            result.ShouldHaveValidationErrorFor(r => r.Name);
        }

        [TestMethod]
        public async Task Validate_With_Name_Over_200_Characters_Fails_On_Name()
        {
            var request = ValidRequest();
            request.Name = new string('n', 201);

            var result = await CreateValidator().TestValidateAsync(request, cancellationToken: TestContext.CancellationToken);

            result.ShouldHaveValidationErrorFor(r => r.Name);
        }

        [TestMethod]
        public async Task Validate_With_Negative_SomeValue_Fails_On_SomeValue()
        {
            var request = ValidRequest();
            request.SomeValue = -1;

            var result = await CreateValidator().TestValidateAsync(request, cancellationToken: TestContext.CancellationToken);

            result.ShouldHaveValidationErrorFor(r => r.SomeValue);
        }

        [TestMethod]
        public async Task Validate_With_SomeValue_Over_100_Fails_On_SomeValue()
        {
            var request = ValidRequest();
            request.SomeValue = 101;
            var result = await CreateValidator().TestValidateAsync(request, cancellationToken: TestContext.CancellationToken);
            result.ShouldHaveValidationErrorFor(r => r.SomeValue);
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(20)]
        [DataRow(99)]
        public async Task Validate_With_SomeValue_In_Range_Passes(int val)
        {
            var request = ValidRequest();
            request.SomeValue = val;

            var result = await CreateValidator().TestValidateAsync(request, cancellationToken: TestContext.CancellationToken);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [TestMethod]
        public async Task Validate_With_Name_At_Limit_Plus_Surrounding_Spaces_Passes()
        {
            var request = ValidRequest();
            request.Name = $"  {new string('n', 100)}  ";

            var result = await CreateValidator().TestValidateAsync(request, cancellationToken: TestContext.CancellationToken);

            result.ShouldNotHaveAnyValidationErrors();
        }

        private static SampleRequestValidator CreateValidator()
        {
            return new SampleRequestValidator();
        }

        private static SampleRequest ValidRequest()
        {
            return new SampleRequest
            {
                Name = "Some Name",
                SomeValue = 67
            };
        }
    }
}
