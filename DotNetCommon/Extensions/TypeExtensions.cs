using System.Reflection;

namespace DotNetCommon.Extensions
{
    public static class TypeExtensions
    {
        public static IEnumerable<string> GetAllPropertyNames(this Type source) => source.GetProperties().Select(x => x.Name);

        public static object CreateInstance(this Type source)
        {
            return Activator.CreateInstance(source);
        }

        public static bool HasCloseMatchingProperty(this Type source, string nameToMatchTo)
        {
            return !String.IsNullOrWhiteSpace(source.GetCloseMatchPropertyName(nameToMatchTo));
        }

        public static string GetCloseMatchPropertyName(this Type source, string nameToMatchTo)
        {
            PropertyInfo[] matchingProps = source.GetProperties().Where(prop => prop.Name.CapsAndTrim() == nameToMatchTo.CapsAndTrim().RemoveWhitespace()).ToArray();
            if (matchingProps.Count() == 0) return null;
            else if (matchingProps.Count() > 1) throw new InvalidOperationException("The given type has more that one close matching property.");
            else return matchingProps.First().Name;
        }
    }
}
