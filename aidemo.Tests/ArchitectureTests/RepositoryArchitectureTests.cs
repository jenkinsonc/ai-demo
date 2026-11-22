using ArchUnitNET.Domain.Extensions;
using ArchUnitNET.Fluent;
using ArchUnitNET.MSTestV4;
using System.Linq;
using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace aidemo.Tests.ArchitectureTests
{
    [TestClass]
    public sealed class RepositoryArchitectureTests
    {
        [Ignore("No repositories yet")] // Claude - unignore this class the first time a repository is added
        [TestMethod]
        public void Repositories_Should_End_With_Repository()
        {
            IArchRule rule = Classes()
                .That().Are(ArchitectureDefinition.RepositoryClasses)
                .As("Repositories should end with `Repository`.")
                .Should().HaveNameEndingWith("Repository");

            rule.Check(ArchitectureDefinition.Architecture);
        }

        [TestMethod]
        public void Repositories_Should_Implement_Interface()
        {
            // Unlike clients, the interface keeps the full class name: `WidgetInstanceRepository` -> `IWidgetInstanceRepository`.
            var violations = ArchitectureDefinition.RepositoryClasses
                .GetObjects(ArchitectureDefinition.Architecture)
                .Where(r => !r.IsAssignableTo($"aidemo.Integrations.Interfaces.I{r.Name}"))
                .Select(r => r.FullName)
                .ToList();

            Assert.IsEmpty(violations, $"Repositories should implement `aidemo.Integrations.Interfaces.I{{Name}}`: {string.Join(", ", violations)}");
        }
    }
}
