using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Exiger.JWT.Core.Extensions
{
    public static class EnumExtensions
    {
        /// <summary>
        /// Returns the string Description value for an enum member that was attributed with a DescriptionAttribute, else null
        /// </summary>
        /// <param name="value">Desired enum member value</param>
        /// <returns>DescriptionAttribute value string, else null if no there is no attribute for the member</returns>
        public static string GetDescription(this Enum value)
        {
            if (value == null) return null;
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
                    else
                    {
                        return value.ToString();
                    }
                }
            }
            return null;
        }

        public static TAttribute GetAttribute<TAttribute>(this Enum value)
            where TAttribute : Attribute
        {
            var type = value.GetType();
            var name = Enum.GetName(type, value);

            return type.GetField(name)
                .GetCustomAttributes(false)
                .OfType<TAttribute>()
                .SingleOrDefault();
        }
    }
}
