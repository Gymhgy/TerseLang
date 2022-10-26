using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DList = System.Collections.Generic.List<dynamic>;

namespace TerseLang {
    public static class DListExtensions {
        public static int Depth(this DList l) {
            return 1 + l.Append(0).Max(x => x is DList ? Depth(x) : 0);
        }
        public static bool DListEquals(this DList l1, DList l2) {
            return l1.Count == l2.Count && l1.Zip(l2, (a, b) => {
                return D.Equals(a, b);
            }).All(x => x);
        }

        public static string DListToString(this DList list) {
            return "[" + string.Join(", ", list.Select(x => x is DList d ? d.DListToString() : x is string s ? "\"" + s + "\"" : x.ToString())) + "]";
        }

        
        public static DList ToDList(this IEnumerable d) {
            return d.Cast<object>().Select(x => x is IEnumerable dx && x is not string ? dx.ToDList() : x).ToList();
        }
    }
    public static class D {
        public static new bool Equals(dynamic x, dynamic y) {
            Console.WriteLine(x.GetType());
            if (x is string && y is string) return x == y;
            if (x is DList dx && y is DList dy) return dx.DListEquals(dy);
            if (x is double && y is double) return x == y;
            return false;
        }
    }
}
