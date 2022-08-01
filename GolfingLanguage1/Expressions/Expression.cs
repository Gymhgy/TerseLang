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

        public FunctionInvocationExpression(Expression caller, string function, Expression argument, 
            VectorizationMode vectorizationMode = VectorizationMode.None) {

            Caller = caller;
            Function = function;
            Argument = argument;
            VectorizationMode = vectorizationMode;
        }


        public Expression Caller { get; }
        public string Function { get; }
        public Expression Argument { get; }
        public VectorizationMode VectorizationMode { get; } = VectorizationMode.None;

        public FunctionInvocationExpression LeftVectorize() {
            return new FunctionInvocationExpression(this.Caller, this.Function, this.Argument, VectorizationMode.Left);
        }

        public FunctionInvocationExpression RightVectorize() {
            return new FunctionInvocationExpression(this.Caller, this.Function, this.Argument, VectorizationMode.Right);
        }
    }
    public enum VectorizationMode { None, Left, Right }

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
        public string Value { get; }
    }

    public class InterpolatedStringExpression : Expression {
        public IEnumerable<Expression> Expressions { get; }
        public InterpolatedStringExpression(IEnumerable<Expression> expressions) {
            Expressions = expressions;
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
