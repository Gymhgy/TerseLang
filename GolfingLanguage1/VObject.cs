using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Text;
using GolfingLanguage1.Expressions;

namespace GolfingLanguage1 {

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
            if(other.ObjectType == this.ObjectType) {
                if(this.ObjectType == ObjectType.List) {
                    var thisList = (List<VObject>)this.Value;
                    var otherList = (List<VObject>)other.Value;
                    if(thisList.Count == otherList.Count) {
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

        public T ConvertTo<T> () {
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
    }

    public enum ObjectType { Number, String, List }
}
