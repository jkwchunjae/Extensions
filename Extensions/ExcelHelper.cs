using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Extensions
{
    public class FieldNameAttribute : Attribute
    {
        public string FieldName;
        public FieldNameAttribute(string fieldName)
        {
            FieldName = fieldName;
        }
    }

    public static class ExcelHelper
    {
        public static T Create<T>(Dictionary<string, object> memberValues) where T : new()
        {
            var type = typeof(T);
            var data = new T();

            var allowTypes = new[] { MemberTypes.Field, MemberTypes.Property };
            var members = type.GetMembers().Where(x => allowTypes.Contains(x.MemberType));

            foreach (var member in members)
            {
                var memberInfoType = member.MemberType == MemberTypes.Field ? typeof(FieldInfo) : typeof(PropertyInfo);
                var memberType = member.MemberType == MemberTypes.Field ? ((FieldInfo)member).FieldType : ((PropertyInfo)member).PropertyType;
                var fieldName = (member.GetCustomAttribute(typeof(FieldNameAttribute)) as FieldNameAttribute)?.FieldName ?? member.Name;

                var setValueMethod = memberInfoType.GetMethods()
                    .Where(x => x.Name == "SetValue")
                    .FirstOrDefault(x => (x.Attributes & MethodAttributes.Abstract) == 0);

                if (setValueMethod == null)
                    continue;

                if (memberValues.ContainsKey(fieldName))
                {
                    if (memberValues[fieldName] != null)
                        setValueMethod.Invoke(member, new object[] { data, Convert.ChangeType(memberValues[fieldName], memberType) });
                }
            }

            return data;
        }
    }
}
