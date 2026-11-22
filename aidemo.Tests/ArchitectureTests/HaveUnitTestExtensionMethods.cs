using ArchUnitNET.Domain;
using ArchUnitNET.Domain.Extensions;
using ArchUnitNET.Fluent;
using ArchUnitNET.Fluent.Conditions;
using ArchUnitNET.Fluent.Syntax.Elements.Members.MethodMembers;
using ArchUnitNET.Fluent.Syntax.Elements.Types.Classes;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace aidemo.Tests.ArchitectureTests
{
    public static partial class HaveUnitTestExtensionMethods
    {
        public static IArchRule HaveUnitTests(this ClassesShould should, string unitTestClassSuffix = "UnitTests")
        {
            var description = "Have unit tests " + should.Description;
            return should.FollowCustomCondition(new ArchitectureCondition<Class>(Condition, description));

            IEnumerable<ConditionResult> Condition(IEnumerable<Class> classes, Architecture architecture)
            {
                foreach (var classItem in classes)
                {
                    var result = GetClassHasUnitTestResult(architecture, classItem, unitTestClassSuffix);
                    if (result is null) continue;
                    yield return result;
                }
            }
        }

        private static ConditionResult GetClassHasUnitTestResult(Architecture architecture, Class classItem, string unitTestClassSuffix)
        {
            var unitTestType = GetUnitTestTypeFromClass(architecture, classItem, unitTestClassSuffix);
            return new ConditionResult(classItem, unitTestType is not null);
        }

        internal static IType GetUnitTestTypeFromClass(Architecture architecture, IType type, string unitTestClassSuffix)
        {
            // classes that are typed will reflect as Type`N where N is the number of generic parameters. We need to remove that part to find the unit test class.
            return type.Name.IndexOf('`') > 0
                ? architecture.Classes.FirstOrDefault(x => x.NameStartsWith(type.Name[..type.Name.IndexOf('`')]) && x.NameEndsWith(unitTestClassSuffix))
                : architecture.Classes.FirstOrDefault(x => x.NameStartsWith(type.Name) && x.NameEndsWith(unitTestClassSuffix));
        }

        public static IArchRule HaveUnitTests(this MethodMembersShould should, string unitTestClassSuffix = "UnitTests")
        {
            var description = "Have unit tests " + should.Description;
            return should.FollowCustomCondition(new ArchitectureCondition<MethodMember>(Condition, description));

            IEnumerable<ConditionResult> Condition(IEnumerable<MethodMember> methodMembers, Architecture architecture)
            {
                foreach (var methodMember in methodMembers)
                {
                    var result = GetMethodHasUnitTestResult(architecture, methodMember, unitTestClassSuffix);
                    if (result is null) continue;
                    yield return result;
                }
            }
        }

        private static ConditionResult GetMethodHasUnitTestResult(Architecture architecture, MethodMember methodMember, string unitTestClassSuffix)
        {
            if (methodMember.DeclaringType is null) return null;

            var unitTestType = GetUnitTestTypeFromClass(architecture, methodMember.DeclaringType, unitTestClassSuffix);
            if (unitTestType is null)
                return new ConditionResult(methodMember, false, $"Class `{methodMember.DeclaringType.FullName}` has no corresponding unit test class.");

            return UnitHasTestMethod(unitTestType, methodMember)
                ? new ConditionResult(methodMember, true)
                : new ConditionResult(methodMember, false);
        }

        private static bool UnitHasTestMethod(IType unitTestType, MethodMember methodMember)
        {
            var methodName = UnitTestRegex().Match(methodMember.Name).Groups[1].Value;
            return unitTestType.Members.OfType<MethodMember>()
                .Any(x => Regex.IsMatch(x.Name, $"^{Regex.Escape(methodName)}(_|$)", RegexOptions.IgnoreCase));
        }

        [GeneratedRegex(@"^\.?(\w+)\W")]
        private static partial Regex UnitTestRegex();
    }
}
