using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DList = System.Collections.Generic.List<dynamic>;

namespace TerseLang {
    public static class DListExtensions {
        public static int Depth(this DList l) {
            return 1 + l.Max(x => x is DList ? Depth(x) : 0);
        }
        public static bool DListEquals(this DList l1, DList l2) {
            return l1.Count == l2.Count && l1.Zip(l2, (a, b) => {
                
                if (a is DList la && b is DList lb) return DListEquals(la, lb);
                if (a is double da && b is double db) return da == db;
                if (a is string sa && b is string sb) return sa == sb;
                return false;
            }).All(x => x);
        }

        public static string DListToString(this DList list) {
            return "[" + string.Join(", ", list.Select(x => x is DList d ? d.DListToString() : x is string s ? "\"" + s + "\"" : x.ToString())) + "]";
        }

        
        public static DList ToDList(this IEnumerable d) {
            return d.Cast<object>().Select(x => x is IEnumerable dx ? dx.ToDList() : x).ToList();
        }
    }
}
