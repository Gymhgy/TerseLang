using System;
using System.Collections.Generic;
using System.Text;

namespace GolfingLanguage1 {

    //Wrapper for List<VObject>, String, and Double
    public class VObject {
        private object val;
        
        public object Value {
            get {
                return val;
            }
            set {
                if (value is IEnumerable<VObject> l) {
                    val = l;
                    ObjectType = ObjectType.List;
                }
                if (value is double d) {
                    val = d;
                    ObjectType = ObjectType.Number;
                }
                if (value is string s) { 
                    val = s;
                    ObjectType = ObjectType.String;
                }
                else
                    throw new ArgumentException();
            }
        }

        public ObjectType ObjectType { get; private set; }

        public VObject(object value) {
            Value = value;
        }
    }

    public enum ObjectType { String, Number, List }
}
