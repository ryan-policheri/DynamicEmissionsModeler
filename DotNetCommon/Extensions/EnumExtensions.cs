using System.ComponentModel;
using System.Reflection;

namespace DotNetCommon.Extensions
{
    public static class EnumExtensions
    {
        public static int ToInt(this Enum value)
        {
            return Convert.ToInt32(value);
        }

        public static string ToDescription(this Enum value)
        {
            Type type = value.GetType();
            string name = Enum.GetName(type, value);
            if (name != null)
            {
                FieldInfo field = type.GetField(name);
                if (field != null)
                {
                    DescriptionAttribute attr = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
                    if (attr != null)
                    {
                        return attr.Description;
                    }
                }
                return name;
            }
            return null;
        }
    }
}
