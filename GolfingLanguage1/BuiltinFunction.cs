using System;
using System.Collections.ObjectModel;
using System.Text;

namespace GolfingLanguage1 {
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
            throw new NotImplementedException();
        }

        public static BuiltinFunction Get(string val) {
            throw new NotImplementedException();
        }

        public static bool IsUnary(string next) {
            throw new NotImplementedException();
        }
    }

    public enum ArgumentType {
        Normal,
        Function
    }
}
