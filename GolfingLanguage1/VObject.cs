using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using TerseLang.Expressions;

namespace TerseLang {

    //Wrapper for List<VObject>, String, and Double
    public class VObject {
        private object val;


        public object Value {
            get {

                return val;
            }
            set {
                if (value is List<VObject> l) {
                    val = l;
                    ObjectType = ObjectType.List;
                }
                else if (value is double d) {
                    val = d;
                    ObjectType = ObjectType.Number;
                }
                else if (value is int i) {
                    val = (double)i;
                    ObjectType = ObjectType.Number;
                }
                else if (value is string s) {
                    val = s;
                    ObjectType = ObjectType.String;
                }
                else
                    throw new ArgumentException();
            }
        }

        public bool Equals(VObject other) {
            if (other.ObjectType == this.ObjectType) {
                if (this.ObjectType == ObjectType.List) {
                    var thisList = (List<VObject>)this.Value;
                    var otherList = (List<VObject>)other.Value;
                    if (thisList.Count == otherList.Count) {
                        return thisList.Zip(otherList, (a, b) => a.Equals(b)).All(x => x);
                    }
                    return false;
                }
                if (this.ObjectType == ObjectType.Number) {
                    return (double)this.Value == (double)other.Value;

                }
                if (this.ObjectType == ObjectType.String) {
                    return (string)this.Value == (string)other.Value;
                }
            }
            return false;
        }

        public T ConvertTo<T>() {
            List<Type> types = new List<Type> { typeof(double), typeof(string), (typeof(List<VObject>)) };
            List<ObjectType> objTypes = new List<ObjectType> { ObjectType.Number, ObjectType.String, ObjectType.List };
            if (types.IndexOf(typeof(T)) == objTypes.IndexOf(this.ObjectType)) {
                return (T)this.Value;
            }
            else
                throw new ArgumentException("Generic type provided was not a type that VObject supports or contains currently.");
        }

        public ObjectType ObjectType { get; private set; }

        public VObject(object value) {
            Value = value;
        }

        public static implicit operator double(VObject t) {
            return (double)t.Value;
        }

        public static implicit operator int(VObject t) {
            return (int)(double)t.Value;
        }

        public static implicit operator string(VObject t) {
            return (string)t.Value;
        }

        public static implicit operator List<VObject>(VObject t) {
            return (List<VObject>)t.Value;
        }

        public static implicit operator VObject(double t) {
            return new VObject(t);
        }

        public static implicit operator VObject(int t) {
            return new VObject((double)t);
        }

        public static implicit operator VObject(string t) {
            return new VObject(t);
        }

        public static implicit operator VObject(List<VObject> t) {
            return new VObject(t);
        }

        public static implicit operator VObject(bool b) {
            return new VObject(b ? 1 : 0);
        }

        public static implicit operator VObject(char c) {
            return new VObject(c.ToString());
        }

        //Helper function that determines whether a value is truthy or not
        public bool IsTruthy() {
            switch (this.ObjectType) {
                case ObjectType.Number:
                    return this != 0;
                case ObjectType.String:
                    return this != "";
                case ObjectType.List:
                    return ((List<VObject>)this).Count != 0;
                default:
                    return false;
            }
        }

        public override string ToString() {
            switch (this.ObjectType) {
                case ObjectType.Number:
                    return ((double)this).ToString();
                case ObjectType.String:
                    return (string)this;
                case ObjectType.List:
                    string str = string.Join(", ", ((List<VObject>)this).Select(x => {
                        var ret = x.ToString();
                        if (x.ObjectType == ObjectType.String) ret = '"' + ret.Replace("\\", "\\\\").Replace("\"", "\\\"") + '"';
                        return ret;
                    }));
                    return "[" + str + "]";
                default:
                    ErrorHandler.InternalError("VObject has no type (?)");
                    throw new Exception();
            }
        }
    }

    public static class ObjectTypeExtensions {
        public static Type ObjectTypeToType(this ObjectType objType) {
            switch (objType) {
                case ObjectType.Number:
                    return typeof(double);
                case ObjectType.String:
                    return typeof(string);
                case ObjectType.List:
                    return typeof(List<VObject>);
                default:
                    throw new ArgumentException("objType");
            }
        }
        public static ObjectType TypeToObjectType(this Type type) {
            //Too bad C# doesn't support switching on types. Gotta use this ugly workaround.
            var @switch = new Dictionary<Type, ObjectType> {
                { typeof(double), ObjectType.Number },
                { typeof(string), ObjectType.String },
                { typeof(List<VObject>), ObjectType.List },
            };
            if (@switch.ContainsKey(type)) return @switch[type];
            throw new ArgumentException("type");
        }
    }

    public static class VObjectListExtensions {
        public static VObject ToVList(this IEnumerable<string> obj) {
            return obj.Select(l => new VObject(l)).ToList();
        }

        public static VObject ToVList(this IEnumerable<double> obj) {
            return obj.Select(l => new VObject(l)).ToList();
        }
        public static VObject ToVList(this IEnumerable<int> obj) {
            return obj.Select(l => new VObject(l)).ToList();
        }
    }

    public enum ObjectType { Number = 1, String = 2, List = 3 }
}
