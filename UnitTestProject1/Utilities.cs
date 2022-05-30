using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Collections;
using System.Reflection;

namespace TerseLang.Tests {
    public static class Utilities {


        public static bool EqualByProperties(object self, object to, params string[] ignore) {
            if (self == null || to == null) return self == to;
            Type type = self.GetType();
            if (type != to.GetType()) return false;
            if (IsSimpleType(type)) return self.Equals(to);
            if (typeof(IEnumerable).IsAssignableFrom(type)) {
                var selfIEnumerable = ((IEnumerable)self).Cast<object>();
                var toEnumerable = ((IEnumerable)to).Cast<object>();
                if (selfIEnumerable.Count() != toEnumerable.Count()) return false;
                else {
                    return selfIEnumerable.Zip(toEnumerable, (a, b) => (a, b)).All(x => EqualByProperties(x.a, x.b));
                }
            }
            else {
                foreach (PropertyInfo pi in type.GetProperties(BindingFlags.Public | BindingFlags.Instance)) {
                    if (!ignore.Contains(pi.Name)) {
                        object selfValue = pi.GetValue(self);
                        object toValue = pi.GetValue(to);

                        if (!EqualByProperties(selfValue, toValue))
                            return false;
                    }
                }
            }
            return true;
        }
        public static bool IsSimpleType(Type type) {
            return
                type.IsPrimitive ||
                new Type[] {
                    typeof(string),
                    typeof(decimal),
                    typeof(DateTime),
                    typeof(DateTimeOffset),
                    typeof(TimeSpan),
                    typeof(Guid)
                }.Contains(type) ||
                type.IsEnum ||
                Convert.GetTypeCode(type) != TypeCode.Object ||
                (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>) && IsSimpleType(type.GetGenericArguments()[0]))
                ;
        }
    }


}
