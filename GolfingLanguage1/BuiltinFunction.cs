using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Text;
using static GolfingLanguage1.Constants;

namespace GolfingLanguage1 {
    public enum ArgumentType {
        Normal,
        Function
    }
    public class BuiltinFunction {
        private Func<VObject, VObject[], VObject> Behavior { get; }

        public int Arity { get; }
        public string Name { get; }
        public ReadOnlyCollection<ArgumentType> ArgumentsTypes;
        public ObjectType InputType { get; }
        public ObjectType OutputType { get; }

        public BuiltinFunction(string name, ObjectType inputType, ObjectType outputType, Func<VObject, VObject[], VObject> behavior, ArgumentType[] argTypes, bool variableArity = false) {
            Name = name;
            InputType = inputType;
            OutputType = outputType;
            Arity = variableArity ? -1 : argTypes.Length;
            Behavior = behavior;
            ArgumentsTypes = new ReadOnlyCollection<ArgumentType>(argTypes);
        }

        public VObject Apply(VObject caller, VObject[] args) {
            throw new NotImplementedException();
        }

        public static int GetTier(string func) {
            if (IsUnary(func)) return 0;
            if (TIER_ZERO.Contains(func)) return 0;
            if (TIER_ONE.Contains(func)) return 1;
            if (TIER_TWO.Contains(func)) return 2;
            if (TIER_UNLIMITED.Contains(func)) return -1;
            throw new ArgumentException("func");
        }

        public static BuiltinFunction Get(string val) {
            return Functions[val];
        }

        public static bool IsUnary(string next) {
            return UNARY_FUNCTIONS.Contains(next);
        }

        private static readonly ReadOnlyDictionary<string, BuiltinFunction> Functions = new ReadOnlyDictionary<string, BuiltinFunction>(new Dictionary<string, BuiltinFunction>
        {

        });
    }
}
