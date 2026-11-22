using System.Threading.Tasks;
using aidemo.Endpoints.Endpoints;
using aidemo.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

namespace aidemo.Tests.EndpointTests
{
    // Claude: This is a placeholder for the unit tests for the SampleEndpoints class. Remove this file when the SampleEndpoints class is removed.
    [TestClass]
    public sealed class SampleEndpointsUnitTests
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public async Task ListSamples_Passes()
        {
            var result = await SampleEndpoints.ListSamples(TestContext.CancellationToken);

            Assert.AreEqual(StatusCodes.Status200OK, result.StatusCode);
            Assert.IsEmpty(result.Value);
        }

        [TestMethod]
        public async Task CreateSample_Passes()
        {
            SampleRequest request = new() { Name = "Asdf", SomeValue = 1 }; // contents don't matter because the method doesn't do anything

            var result = await SampleEndpoints.CreateSample(request, TestContext.CancellationToken);

            Assert.AreEqual(StatusCodes.Status200OK, result.StatusCode);
            Assert.IsNotNull(result.Value);
        }

        [TestMethod]
        public async Task UpdateSample_Passes()
        {
            SampleRequest request = new() { Name = "Asdf", SomeValue = 1 }; // contents don't matter because the method doesn't do anything

            var result = await SampleEndpoints.UpdateSample(1, request, TestContext.CancellationToken);

            var ok = Assert.IsInstanceOfType<Ok<SampleResponse>>(result.Result);
            Assert.IsNotNull(ok.Value);
        }

        [TestMethod]
        public async Task DeleteSample_Passes()
        {
            var result = await SampleEndpoints.DeleteSample(1, TestContext.CancellationToken);

            Assert.IsInstanceOfType<NoContent>(result.Result);
        }
    }
}
