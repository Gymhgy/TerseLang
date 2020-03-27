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
        public FunctionInvocationExpression(BuiltinFunction function, Expression argument) {
            Function = function;
            Argument = argument;
        }

        public FunctionInvocationExpression(BuiltinFunction function, bool isUnary) {
            Function = function;
            IsUnary = isUnary;
        }

        public BuiltinFunction Function { get; }

        public bool IsUnary { get; } = false;
        public Expression Argument { get; }
    }

    public class FunctionArgumentExpression : Expression {
        public FunctionArgumentExpression(Expression body) {
            Body = body;
        }

        public Expression Body { get; }
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
