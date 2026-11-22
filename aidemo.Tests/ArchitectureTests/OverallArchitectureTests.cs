using ArchUnitNET.Fluent;
using ArchUnitNET.Fluent.Slices;
using ArchUnitNET.MSTestV4;
using System.Runtime.CompilerServices;
using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace aidemo.Tests.ArchitectureTests
{
    [TestClass]
    public sealed class OverallArchitectureTests
    {
        [TestMethod]
        public void Application_Should_Be_Free_Of_Cycles()
        {
            SliceRule rule = SliceRuleDefinition.Slices().Matching("aidemo.(*)..").Should().BeFreeOfCycles();

            rule.Check(ArchitectureDefinition.Architecture);
        }

        [TestMethod]
        public void Method_Names_Should_Start_With_Uppercase_Letter()
        {
            IArchRule rule = MethodMembers()
                .That().AreDeclaredIn(ArchitectureDefinition.ClassesThatShouldBeTested)
                .And().AreNoConstructors()
                .And().DoNotHaveAnyAttributes(typeof(SpecialNameAttribute), typeof(CompilerGeneratedAttribute))
                .As("Methods should start with uppercase letter.")
                .Should().HaveNameMatching("^[A-Z]");

            rule.Check(ArchitectureDefinition.Architecture);
        }

        [TestMethod]
        public void Testable_Classes_Should_Never_Be_Excluded_From_Code_Coverage()
        {
            IArchRule rule = Classes()
                .That().Are(ArchitectureDefinition.ClassesThatShouldBeTested)
                .As("Testable classes should never be excluded from code coverage.")
                .Should().NotHaveAnyAttributes(typeof(System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute));

            rule.Check(ArchitectureDefinition.Architecture);
        }

        [TestMethod]
        public void Testable_Classes_Should_Have_Unit_Tests()
        {
            IArchRule rule = Classes()
                .That().Are(ArchitectureDefinition.ClassesThatShouldBeTested)
                .As("Testable classes should have a matching *UnitTests class.")
                .Should().HaveUnitTests();

            rule.Check(ArchitectureDefinition.Architecture);
        }

        [TestMethod]
        public void UnitTests_Should_Be_In_Unit_Test_Layer()
        {
            IArchRule rule = Classes()
                .That().Are(ArchitectureDefinition.UnitTestClasses)
                .As("Unit tests should be in the unit test layer.")
                .Should().ResideInAssembly(ArchitectureDefinition.TestsAssembly);

            rule.Check(ArchitectureDefinition.Architecture);
        }
    }
}
