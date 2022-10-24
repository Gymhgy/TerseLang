using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Numerics;

namespace TerseLang.Expressions {
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
        public FunctionInvocationExpression(Expression caller, string function) {
            Caller = caller;
            Function = function;
        }

        public FunctionInvocationExpression(Expression caller, string function, Expression argument) {

            Caller = caller;
            Function = function;
            Argument = argument;
        }


        public Expression Caller { get; }
        public string Function { get; }
        public Expression Argument { get; }

    }
    public enum VectorizationMode { None, Left, Right }

    public class NumericLiteralExpression : Expression {
        public NumericLiteralExpression(double value) {
            Value = value;
        }

        public double Value { get; }
    }
    public class StringExpression : Expression { }
    public class StringLiteralExpression : StringExpression {
        public StringLiteralExpression(string value) {
            Value = value;
        }
        public string Value { get; }
    }

    public class InterpolatedStringExpression : StringExpression {
        public IEnumerable<Expression> Expressions { get; }
        public InterpolatedStringExpression(IEnumerable<Expression> expressions) {
            Expressions = expressions;
        }
    }
    public class StringListExpression : StringExpression {
        public IEnumerable<StringExpression> Strings { get; }
        public StringListExpression(IEnumerable<StringExpression> strings) {
            Strings = strings;
        }
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
