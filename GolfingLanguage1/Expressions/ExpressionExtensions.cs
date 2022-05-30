using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerseLang.Expressions {
    public static class ExpressionExtensions {

        public static FunctionInvocationExpression Invoke(this Expression expr, string func, Expression arg) {
            return new FunctionInvocationExpression(expr, func, arg);
        }
        public static FunctionInvocationExpression Invoke(this Expression expr, string func) {
            return new FunctionInvocationExpression(expr, func);
        }
        public static ConditionalExpression If(this Expression condition, Expression trueExpr, Expression falseExpr) {
            return new ConditionalExpression(condition, trueExpr, falseExpr);
        }
    }
}
