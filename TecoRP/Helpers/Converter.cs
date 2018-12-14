using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TecoRP.Helpers
{
    public static class Converter
    {
        public static T ConvertTo<T>(this object from)
        {
            return (T)from.ConvertTo(typeof(T));
        }
        public static object ConvertTo(this object from, Type type)
        {
            var tmp = Activator.CreateInstance(type);
            foreach (var property in type.GetProperties())
            {
                property.SetValue(tmp, from.GetType().GetProperty(property.Name)?.GetValue(from));
            }
            return tmp;
        }
    }
}
