using ArchUnitNET.Domain.Dependencies;
using ArchUnitNET.Fluent;
using ArchUnitNET.MSTestV4;
using FluentValidation;
using aidemo.Models.DataModels;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace aidemo.Tests.ArchitectureTests
{
    [TestClass]
    public sealed partial class ModelArchitectureTests
    {
        [TestMethod]
        public void Entities_Should_Inherit_BaseEntity()
        {
            IArchRule rule = Classes()
                .That().Are(ArchitectureDefinition.DataModelClasses)
                .And().AreNot(typeof(BaseEntity))
                .As("EF entities should inherit `BaseEntity`.")
                .Should().BeAssignableTo(typeof(BaseEntity));

            rule.Check(ArchitectureDefinition.Architecture);
        }

        [TestMethod]
        public void Models_Should_Not_Use_DateTime_Properties()
        {
            var violations = ArchitectureDefinition.ToSystemTypes(ArchitectureDefinition.DataModelClasses)
                .Concat(ArchitectureDefinition.ToSystemTypes(ArchitectureDefinition.ViewModelClasses))
                .SelectMany(t => t.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly))
                .Where(p => (Nullable.GetUnderlyingType(p.PropertyType) ?? p.PropertyType) == typeof(DateTime))
                .Select(p => $"{p.DeclaringType.FullName}.{p.Name}")
                .ToList();

            Assert.IsEmpty(violations, $"Timestamps should use `DateTimeOffset`, never `DateTime` (its `Kind` defaults to `Unspecified`): {string.Join(", ", violations)}");
        }

        [GeneratedRegex(@"^set_(CreatedDate|LastUpdatedDate)\(")]
        private static partial Regex AuditFieldSetter();

        [TestMethod]
        public void Audit_Fields_Should_Only_Be_Set_By_The_Interceptor()
        {
            // The interceptor sets them through reflection, so no compiled call to their setters belongs in production code.
            var violations = ArchitectureDefinition.Architecture.Types
                .Where(t => t.Assembly != ArchitectureDefinition.TestsAssembly)
                .SelectMany(t => t.Dependencies.OfType<MethodCallDependency>())
                .Where(d => AuditFieldSetter().IsMatch(d.TargetMember.Name))
                .Select(d => $"{d.Origin.FullName} calls {d.TargetMember.Name}")
                .Distinct()
                .ToList();

            Assert.IsEmpty(violations, $"`CreatedDate` and `LastUpdatedDate` are populated by `AuditFieldsSaveChangesInterceptor`; never set them in constructors, object initializers or repositories: {string.Join(", ", violations)}");
        }

        private static readonly string[] NonRootAssemblies = ["aidemo.Endpoints", "aidemo.Integrations", "aidemo.Models"];

        [TestMethod]
        public void Only_Composition_Roots_Should_Depend_On_IConfiguration()
        {
            // aidemo.Server, aidemo.AppHost and the `*.Configuration` namespaces wire settings up; everything else reads them as `IOptions<T>`.
            const BindingFlags members = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;

            var violations = NonRootAssemblies
                .SelectMany(a => Assembly.Load(a).GetTypes())
                .Where(t => t.Namespace is not null && !t.Namespace.EndsWith(".Configuration", StringComparison.Ordinal))
                .SelectMany(t => t.GetConstructors(members).SelectMany(c => c.GetParameters()).Select(p => (t, p.ParameterType))
                    .Concat(t.GetMethods(members).SelectMany(m => m.GetParameters()).Select(p => (t, p.ParameterType)))
                    .Concat(t.GetFields(members).Select(f => (t, f.FieldType)))
                    .Concat(t.GetProperties(members).Select(p => (t, p.PropertyType))))
                .Where(x => typeof(IConfiguration).IsAssignableFrom(x.Item2))
                .Select(x => x.t.FullName)
                .Distinct()
                .ToList();

            Assert.IsEmpty(violations, $"Settings should be bound with the Options pattern and consumed as `IOptions<T>`, not read from `IConfiguration`: {string.Join(", ", violations)}");
        }

        [Ignore("No migrations yet")] // Claude - unignore this method the first time a migration is added
        [TestMethod]
        public void Migrations_Should_Be_Named_In_PascalCase()
        {
            var violations = Assembly.Load("aidemo.Integrations").GetTypes()
                .Where(t => typeof(Migration).IsAssignableFrom(t))
                .Where(t => !PascalCase().IsMatch(t.Name))
                .Select(t => t.FullName)
                .ToList();

            Assert.IsEmpty(violations, $"Migrations should be named in PascalCase after the change (e.g. `AddWidgetInstances`): {string.Join(", ", violations)}");
        }

        [GeneratedRegex("^[A-Z][A-Za-z0-9]*$")]
        private static partial Regex PascalCase();

        [TestMethod]
        public void ViewModels_Should_End_With_Response_Or_Request()
        {
            IArchRule rule = Classes()
                .That().Are(ArchitectureDefinition.ViewModelClasses)
                .As("View models should end with `Response` or `Request`.")
                .Should().HaveNameMatching("(Response|Request)$");

            rule.Check(ArchitectureDefinition.Architecture);
        }

        [TestMethod]
        public void Validators_Should_Reside_In_Validators_Namespace()
        {
            var violations = ArchitectureDefinition.Architecture.Assemblies
                .Where(a => a != ArchitectureDefinition.TestsAssembly)
                .SelectMany(a => Assembly.Load(a.Name).GetTypes())
                .Where(t => t.IsClass && typeof(IValidator).IsAssignableFrom(t))
                .Where(t => t.Namespace != "aidemo.Models.Validators")
                .Select(t => t.FullName)
                .ToList();

            Assert.IsEmpty(violations, $"Validators should reside in `aidemo.Models.Validators`: {string.Join(", ", violations)}");
        }
    }
}
