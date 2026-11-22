using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using aidemo.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

namespace aidemo.Endpoints.Endpoints
{
    // Claude: this is a sample of how to implement endpoints with the new minimal APIs. It is not a real endpoint, and will be deleted after the first real endpoints are added.
    public static class SampleEndpoints
    {
        public static async Task<Ok<IEnumerable<SampleResponse>>> ListSamples(CancellationToken cancellationToken)
        {
            IEnumerable<SampleResponse> result = [];
            return TypedResults.Ok(result);
        }

        public static async Task<Ok<SampleResponse>> CreateSample(SampleRequest request, CancellationToken cancellationToken)
        {
            return TypedResults.Ok(new SampleResponse());
        }

        public static async Task<Results<Ok<SampleResponse>, NotFound>> UpdateSample(int id, SampleRequest request, CancellationToken cancellationToken)
        {
            return TypedResults.Ok(new SampleResponse());
        }

        public static async Task<Results<NoContent, NotFound>> DeleteSample(int id, CancellationToken cancellationToken)
        {
            return TypedResults.NoContent();
        }
    }
}
