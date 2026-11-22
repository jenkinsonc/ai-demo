using System.Collections.Generic;
using aidemo.Endpoints.Endpoints;
using aidemo.Endpoints.Filters;
using aidemo.Models.ViewModels;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace aidemo.Endpoints.Configuration
{
    // Claude: after adding the first real endpoints, delete this file and all references to it
    public static partial class ApiBuilder
    {
        public static void MapSampleEndpoints(this WebApplication app)
        {
            var group = app.MapGroup("api/samples").WithTags("Sample Endpoints");

            group.MapGet("/", SampleEndpoints.ListSamples)
              .WithName("List Samples")
              .Produces<IEnumerable<SampleResponse>>(200)
              .ProducesProblem(500);

            group.MapPost("/", SampleEndpoints.CreateSample)
                .WithName("Create Sample")
                .AddEndpointFilter<ValidationFilter<SampleRequest>>()
                .Produces<SampleResponse>(200)
                .ProducesValidationProblem()
                .ProducesProblem(500);

            group.MapPut("/{id:int}", SampleEndpoints.UpdateSample)
                .WithName("Update Sample")
                .AddEndpointFilter<ValidationFilter<SampleRequest>>()
                .Produces<SampleResponse>(200)
                .Produces(404)
                .ProducesValidationProblem()
                .ProducesProblem(500);

            group.MapDelete("/{id:int}", SampleEndpoints.DeleteSample)
                .WithName("Delete Sample")
                .Produces(204)
                .Produces(404)
                .ProducesProblem(500);
        }
    }
}
