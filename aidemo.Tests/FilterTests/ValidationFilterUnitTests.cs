using FluentValidation;
using aidemo.Endpoints.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace aidemo.Tests.FilterTests
{
    [TestClass]
    public sealed class ValidationFilterUnitTests
    {
        private static readonly object NextResult = new();
        private static readonly string[] BodyRequiredErrors = ["Request body is required."];
        private static readonly string[] NameRequiredErrors = ["Name is required."];

        [TestMethod]
        public async Task InvokeAsync_With_No_Body_Returns_A_Validation_Problem_And_Skips_Next()
        {
            var nextCalled = false;

            var result = await InvokeAsync(null, () => nextCalled = true);

            var problem = Assert.IsInstanceOfType<ValidationProblem>(result);
            Assert.AreEqual(StatusCodes.Status400BadRequest, problem.StatusCode);
            CollectionAssert.AreEqual(BodyRequiredErrors, problem.ProblemDetails.Errors["body"]);
            Assert.IsFalse(nextCalled);
        }

        [TestMethod]
        public async Task InvokeAsync_With_An_Invalid_Body_Returns_The_Validator_Errors_And_Skips_Next()
        {
            var nextCalled = false;

            var result = await InvokeAsync(new TestRequest { Name = "" }, () => nextCalled = true);

            var problem = Assert.IsInstanceOfType<ValidationProblem>(result);
            Assert.AreEqual(StatusCodes.Status400BadRequest, problem.StatusCode);
            CollectionAssert.AreEqual(NameRequiredErrors, problem.ProblemDetails.Errors[nameof(TestRequest.Name)]);
            Assert.IsFalse(nextCalled);
        }

        [TestMethod]
        public async Task InvokeAsync_With_A_Valid_Body_Returns_The_Result_Of_Next()
        {
            var nextCalled = false;

            var result = await InvokeAsync(new TestRequest { Name = "clock" }, () => nextCalled = true);

            Assert.AreSame(NextResult, result);
            Assert.IsTrue(nextCalled);
        }

        private static async Task<object> InvokeAsync(TestRequest request, Action onNext)
        {
            var validator = new InlineValidator<TestRequest>();
            validator.RuleFor(r => r.Name).NotEmpty().WithMessage("Name is required.");
            var httpContext = new DefaultHttpContext
            {
                RequestServices = new ServiceCollection()
                    .AddSingleton<IValidator<TestRequest>>(validator)
                    .BuildServiceProvider(),
            };
            var context = new DefaultEndpointFilterInvocationContext(httpContext, request);

            return await new ValidationFilter<TestRequest>().InvokeAsync(context, _ =>
            {
                onNext();
                return ValueTask.FromResult(NextResult);
            });
        }

        private sealed class TestRequest
        {
            public string Name { get; set; }
        }
    }
}
