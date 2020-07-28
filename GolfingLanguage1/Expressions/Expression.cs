using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Numerics;

namespace GolfingLanguage1.Expressions {
    public abstract class Expression {

    }

    // An AutoExpression tells the Interpreter that this value will be filled in based on the state of the program
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

    public class ListExpression : Expression {
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

    public class ConditionalExpression : Expression {
        public ConditionalExpression(Expression condition, Expression trueExpression, Expression falseExpression) {
            Condition = condition;
            TrueExpression = trueExpression;
            FalseExpression = falseExpression;
        }

        public Expression Condition { get; }
        public Expression TrueExpression { get; }
        public Expression FalseExpression { get; }
    }

}
