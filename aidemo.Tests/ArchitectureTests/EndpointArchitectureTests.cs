using static ArchUnitNET.Fluent.ArchRuleDefinition;
using ArchUnitNET.Fluent;
using ArchUnitNET.MSTestV4;
using Microsoft.AspNetCore.Http.HttpResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace aidemo.Tests.ArchitectureTests
{
    [TestClass]
    public sealed class EndpointArchitectureTests
    {
        [TestMethod]
        public void Endpoints_Should_End_With_Endpoints()
        {
            IArchRule rule = Classes()
                .That().Are(ArchitectureDefinition.EndpointClasses)
                .As("Classes in the `*.Endpoints` namespace should end with `Endpoints`.")
                .Should().HaveNameEndingWith("Endpoints");

            rule.Check(ArchitectureDefinition.Architecture);
        }

        [TestMethod]
        public void ApiLayer_Should_Not_Reference_AppHost_Classes()
        {
            IArchRule rule = Classes()
                .That().Are(ArchitectureDefinition.ApiLayer)
                .As("Classes in the `*.Endpoints` namespace should not reference classes in the `aidemo.AppHost` namespace.")
                .Should().NotDependOnAny(ArchitectureDefinition.AppHostClasses);
            rule.Check(ArchitectureDefinition.Architecture);
        }

        [TestMethod]
        public void ApiLayer_Should_Not_Reference_Server_Classes()
        {
            IArchRule rule = Classes()
                .That().Are(ArchitectureDefinition.ApiLayer)
                .As("Classes in the `*.Endpoints` namespace should not reference classes in the `aidemo.Server` namespace.")
                .Should().NotDependOnAny(ArchitectureDefinition.ServerClasses);
            rule.Check(ArchitectureDefinition.Architecture);
        }

        // The "Accepted return types" table in .docs/agents/endpoint-result-types.md. Generic entries are open definitions.
        private static readonly HashSet<Type> AcceptedResultTypes =
        [
            typeof(Ok<>),
            typeof(Ok),
            typeof(ValidationProblem),
            typeof(NotFound),
            typeof(NotFound<>),
            typeof(UnauthorizedHttpResult),
            typeof(ForbidHttpResult),
            typeof(NoContent),
            typeof(ProblemHttpResult),
        ];

        [TestMethod]
        public void Endpoints_Should_Return_Accepted_Result_Types()
        {
            var violations = EndpointMethods()
                .Where(m => !IsAcceptedResult(UnwrapTask(m.ReturnType)))
                .Select(m => $"{m.DeclaringType.FullName}.{m.Name} returns {m.ReturnType}")
                .ToList();

            Assert.IsEmpty(violations, $"Endpoints should return only the accepted result types in .docs/agents/endpoint-result-types.md: {string.Join(", ", violations)}");
        }

        [TestMethod]
        public void Endpoints_Should_Not_Return_Entities()
        {
            var violations = EndpointMethods()
                .Where(m => ReferencedTypes(m.ReturnType).Any(t => t.Namespace == "aidemo.Models.DataModels"))
                .Select(m => $"{m.DeclaringType.FullName}.{m.Name} returns {m.ReturnType}")
                .ToList();

            Assert.IsEmpty(violations, $"Endpoints should return view models, never entities: {string.Join(", ", violations)}");
        }

        private static IEnumerable<MethodInfo> EndpointMethods()
        {
            return ArchitectureDefinition.ToSystemTypes(ArchitectureDefinition.EndpointClasses)
                .SelectMany(t => t.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly))
                .Where(m => !m.IsSpecialName);
        }

        private static Type UnwrapTask(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Task<>)
                ? type.GetGenericArguments()[0]
                : type;
        }

        private static bool IsAcceptedResult(Type type)
        {
            if (IsResultsUnion(type))
                return type.GetGenericArguments().All(t => AcceptedResultTypes.Contains(Definition(t)));

            return AcceptedResultTypes.Contains(Definition(type));
        }

        // `Results<TResult1, ..., TResultN>` unions, of any arity.
        private static bool IsResultsUnion(Type type)
        {
            return type.IsGenericType
                && type.Namespace == typeof(Ok).Namespace
                && type.Name.StartsWith("Results`", StringComparison.Ordinal);
        }

        private static Type Definition(Type type)
        {
            return type.IsGenericType ? type.GetGenericTypeDefinition() : type;
        }

        private static IEnumerable<Type> ReferencedTypes(Type type)
        {
            yield return type;

            var children = type.HasElementType ? [type.GetElementType()] : type.GetGenericArguments();
            foreach (var child in children)
            {
                foreach (var referenced in ReferencedTypes(child))
                    yield return referenced;
            }
        }
    }
}
