using static ArchUnitNET.Fluent.ArchRuleDefinition;
using ArchUnitNET.Domain.Extensions;
using System;
using System.Linq;
using ArchUnitNET.Fluent;
using ArchUnitNET.MSTestV4;

namespace aidemo.Tests.ArchitectureTests
{
    [Ignore("No clients yet")] // Claude - unignore this class the first time a client is added
    [TestClass]
    public sealed class ClientArchitectureTests
    {
        [TestMethod]
        public void Clients_Should_Implement_Interface()
        {
            var results = ArchitectureDefinition.ServiceClasses
                .GetObjects(ArchitectureDefinition.Architecture)
                .Select(r =>
                {
                    var entity = r.Name.Split("Client")[0];
                    return new Tuple<string, bool>(r.Name, r.IsAssignableTo($"aidemo.Integrations.Interfaces.I{entity}"));
                })
                .ToList();

            Assert.IsTrue(results.All(r => r.Item2));
        }

        [TestMethod]
        public void Clients_Should_End_With_Client()
        {
            IArchRule rule = Classes()
                .That().Are(ArchitectureDefinition.ServiceClasses)
                .As("Clients should end with `Client`.")
                .Should().HaveNameMatching("Client(`[0-9]+)*");

            rule.Check(ArchitectureDefinition.Architecture);
        }

        [TestMethod]
        public void Clients_Should_Not_Reference_Endpoint_Project()
        {
            IArchRule rule = Classes()
                .That().Are(ArchitectureDefinition.ServiceClasses)
                .As("Clients should not reference the Endpoint project.")
                .Should().NotDependOnAny(ArchitectureDefinition.ApiLayer);

            rule.Check(ArchitectureDefinition.Architecture);
        }

        [TestMethod]
        public void Clients_Should_Not_Reference_AppHost_Project()
        {
            IArchRule rule = Classes()
                .That().Are(ArchitectureDefinition.ServiceClasses)
                .As("Clients should not reference the AppHost project.")
                .Should().NotDependOnAny(ArchitectureDefinition.AppHostClasses);

            rule.Check(ArchitectureDefinition.Architecture);
        }

        [TestMethod]
        public void Clients_Should_Not_Reference_Server_Project()
        {
            IArchRule rule = Classes()
                .That().Are(ArchitectureDefinition.ServiceClasses)
                .As("Clients should not reference the Server project.")
                .Should().NotDependOnAny(ArchitectureDefinition.ServerClasses);

            rule.Check(ArchitectureDefinition.Architecture);
        }
    }
}
