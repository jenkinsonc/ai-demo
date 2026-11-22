using ArchUnitNET.Domain;
using ArchUnitNET.Loader;
using System.Collections.Generic;
using System.Linq;
using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace aidemo.Tests.ArchitectureTests
{
    internal static class ArchitectureDefinition
    {
        public static Architecture Architecture { get; }

        public static Assembly AppHostAssembly { get; }
        public static Assembly EndpointsAssembly { get; }
        public static Assembly IntegrationsAssembly { get; }
        public static Assembly ModelsAssembly { get; }
        public static Assembly ServerAssembly { get; }
        public static Assembly TestsAssembly { get; }

        public static IObjectProvider<IType> AppHostClasses { get; }
        public static IObjectProvider<IType> ServerClasses { get; }
        public static IObjectProvider<IType> EndpointClasses { get; }
        public static IObjectProvider<IType> ApiLayer { get; }
        public static IObjectProvider<IType> DataModelClasses { get; }
        public static IObjectProvider<Interface> Interfaces { get; }
        public static IObjectProvider<IType> RepositoryClasses { get; }
        public static IObjectProvider<IType> ServiceClasses { get; }
        public static IObjectProvider<IType> ViewModelClasses { get; }
        public static IObjectProvider<IType> ValidatorClasses { get; }
        public static IObjectProvider<IType> UnitTestClasses { get; }
        public static IObjectProvider<IType> ClassesThatShouldBeTested { get; }

        static ArchitectureDefinition()
        {
            Architecture = new ArchLoader().LoadAssemblies(
                System.Reflection.Assembly.Load("aidemo.AppHost"),
                System.Reflection.Assembly.Load("aidemo.Endpoints"),
                System.Reflection.Assembly.Load("aidemo.Integrations"),
                System.Reflection.Assembly.Load("aidemo.Models"),
                System.Reflection.Assembly.Load("aidemo.Server"),
                System.Reflection.Assembly.Load("aidemo.Tests")
            ).Build();

            AppHostAssembly = Architecture.Assemblies.Single(a => a.Name == "aidemo.AppHost");
            EndpointsAssembly = Architecture.Assemblies.Single(a => a.Name == "aidemo.Endpoints");
            IntegrationsAssembly = Architecture.Assemblies.Single(a => a.Name == "aidemo.Integrations");
            ModelsAssembly = Architecture.Assemblies.Single(a => a.Name == "aidemo.Models");
            ServerAssembly = Architecture.Assemblies.Single(a => a.Name == "aidemo.Server");
            TestsAssembly = Architecture.Assemblies.Single(a => a.Name == "aidemo.Tests");

            AppHostClasses = Classes().That().ArePublic()
                .And().ResideInNamespace("aidemo.AppHost")
                .As("AppHost Classes");

            ServerClasses = Classes().That().ArePublic()
                .And().ResideInNamespace("aidemo.Server")
                .As("Server Classes");

            EndpointClasses = Classes().That().ArePublic()
                .And().ResideInNamespace("aidemo.Endpoints.Endpoints")
                .As("Endpoint Classes");

            ApiLayer = Classes().That().ArePublic()
                .And().ResideInAssembly(EndpointsAssembly)
                .As("API Layer");

            DataModelClasses = Classes()
                .That().ResideInNamespace("aidemo.Models.DataModels")
                .As("Data Model Classes");

            ViewModelClasses = Classes()
                .That().ResideInNamespace("aidemo.Models.ViewModels")
                .As("View Model Classes");

            RepositoryClasses = Classes().That().ArePublic()
                .And().ResideInNamespace("aidemo.Integrations.Repositories")
                .As("Repository Classes");

            Interfaces = Interfaces().That().ArePublic()
                .And().ResideInNamespace("aidemo.Integrations.Interfaces")
                .As("Interfaces");

            ServiceClasses = Classes().That().ArePublic()
                .And().ResideInNamespace("aidemo.Integrations.Services")
                .As("Service Classes");

            ValidatorClasses = Classes().That().ArePublic()
                .And().ResideInNamespace("aidemo.Models.Validators")
                .As("Validator Classes");

            UnitTestClasses = Classes().That().ArePublic()
                .And().HaveNameEndingWith("Tests").Or().HaveAnyAttributes(typeof(TestClassAttribute))
                .As("Unit Test Classes");

            ClassesThatShouldBeTested = Types()
                .That().ResideInNamespace("aidemo.Endpoints.Endpoints")
                .Or().ResideInNamespace("aidemo.Endpoints.Filters")
                .Or().ResideInNamespace("aidemo.Integrations.Interceptors")
                .Or().ResideInNamespace("aidemo.Integrations.Repositories")
                .Or().ResideInNamespace("aidemo.Integrations.HealthChecks")
                .Or().ResideInNamespace("aidemo.Integrations.Services")
                .Or().ResideInNamespace("aidemo.Models.Validators")
                .And().ArePublic()
                .As("Testable Classes");
        }

        /// <summary>
        /// Resolves the types a provider selects to their runtime <see cref="System.Type"/>, for rules
        /// that inspect member signatures (property types, method return types) via reflection.
        /// </summary>
        public static IEnumerable<System.Type> ToSystemTypes(IObjectProvider<IType> provider)
        {
            return provider.GetObjects(Architecture)
                .Select(t => System.Type.GetType($"{t.FullName}, {t.Assembly.Name}", throwOnError: true));
        }
    }
}
