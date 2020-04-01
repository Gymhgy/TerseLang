using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace GolfingLanguage1.Expressions {
    public abstract class Expression {
        
    }

    public class AutoExpression : Expression { 
    }

    public class VariableReferenceExpression : Expression {
        public VariableReferenceExpression(string name) {
            Name = name;
        }

        public string Name { get; }
    }

    public class FunctionInvocationExpression : Expression {
        public FunctionInvocationExpression(Expression caller, string function, IList<Expression> arguments) {
            Caller = caller;
            Function = function;
            Arguments = new ReadOnlyCollection<Expression>(arguments);
        }

        public Expression Caller { get; }
        public string Function { get; }
        public ReadOnlyCollection<Expression> Arguments { get; }
    }

    public class ArrayExpression : Expression {
        public ReadOnlyCollection<Expression> Contents { get; }
    }

    public class NumericLiteralExpression : Expression {
        public NumericLiteralExpression(double value) {
            Value = value;
        }

        public double Value { get; }
    }

    public class StringLiteralExpression : Expression {
        public StringLiteralExpression(string value) {
            Value = value;
        }

        public StringLiteralExpression(IList<Expression> interpolations, string value) {
            Interpolations = new ReadOnlyCollection<Expression>(interpolations);
            Value = value;
        }

        public ReadOnlyCollection<Expression> Interpolations { get; }
        public string Value { get; }
    }

}
