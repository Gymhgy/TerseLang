using TerseLang;
using TerseLang.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using static TerseLang.Tests.Utilities;

namespace TerseLang.Tests {
    [TestClass]
    public class ParserTests {
        private static IList<Expression> none = Array.Empty<Expression>();
        private static IList<Expression> auto = new[] { new AutoExpression() };

        [TestMethod]
        public void Parser_Number() {
            var actual = Parser.Parse("2");
            var expected = new List<Expression> { new NumericLiteralExpression(2) };
            Assert.IsTrue(EqualByProperties(actual, expected), actual.Dump());
        }

        [TestMethod]
        public void Parser_Variable() {
            var actual = Parser.Parse("作");
            var expected = new List<Expression> { new VariableReferenceExpression("作") };
            Assert.IsTrue(EqualByProperties(actual, expected), actual.Dump());
        }

        [TestMethod]
        public void Parser_AutoPlusFunction() {
            var actual = Parser.Parse("$");
            var expected = new List<Expression> { new FunctionInvocationExpression(new AutoExpression(), "$", auto) };
            Assert.IsTrue(EqualByProperties(actual, expected), actual.Dump());
        }

        [TestMethod]
        public void Parser_AutoAsArgument() {
            var actual = Parser.Parse("2$$2");
            var expected = new List<Expression> { new FunctionInvocationExpression(new FunctionInvocationExpression(new NumericLiteralExpression(2), "$", auto), "$", auto), new NumericLiteralExpression(2) };
            Assert.IsTrue(EqualByProperties(actual, expected), actual.Dump());
        }

        [TestMethod]
        public void Parser_TiersOneAndZero() {
            var actual = Parser.Parse("感$x之$");
            var expected = new List<Expression> { new FunctionInvocationExpression(new FunctionInvocationExpression(
                new FunctionInvocationExpression(new VariableReferenceExpression("感"), "$", auto), "x", new[] {new VariableReferenceExpression("之") }
                ), "$", auto)      
            };
            Assert.IsTrue(EqualByProperties(actual, expected), actual.Dump());
        }

        [TestMethod]
        public void Parser_TierUnlimited() {
            var actual = Parser.Parse("点3因而");
            var expected = new List<Expression> { new FunctionInvocationExpression(new AutoExpression(), "点", new[] { 
                new FunctionInvocationExpression(new FunctionInvocationExpression(
                    new NumericLiteralExpression(3), "因", none), 
                    "而", none)
            })
            };
            Assert.IsTrue(EqualByProperties(actual, expected), actual.Dump());
        }

        [TestMethod]
        public void Parser_TierOverlap() {
            var actual = Parser.Parse("3有5怎6/$");
            var expected = new List<Expression> { new FunctionInvocationExpression(new NumericLiteralExpression(3), "有", new[]{
                new FunctionInvocationExpression(new NumericLiteralExpression(5), "怎", new Expression[]{ new FunctionInvocationExpression(
                new FunctionInvocationExpression(
                    new NumericLiteralExpression(6), "/", auto), "$", auto) }) })
            };
            Assert.IsTrue(EqualByProperties(actual, expected), actual.Dump());
        }
        [TestMethod]
        public void Parser_Brackets() {
            var actual = Parser.Parse("3怎3怎}");
            var expected = new List<Expression> { new FunctionInvocationExpression(new NumericLiteralExpression(3), "怎", new[]{ 
                new FunctionInvocationExpression(new NumericLiteralExpression(3), "怎", auto) })
            };
            Assert.IsTrue(EqualByProperties(actual, expected), actual.Dump());
        }

        [TestMethod]
        public void Parser_MultipleExpressions() {
            var actual = Parser.Parse("3感4感");
            var expected = new List<Expression> { 
                    new NumericLiteralExpression(3),
                    new VariableReferenceExpression("感"),
                    new NumericLiteralExpression(4),
                    new VariableReferenceExpression("感")
            };
            Assert.IsTrue(EqualByProperties(actual, expected), actual.Dump());
        }

        [TestMethod]
        public void Parser_CloseAll() {
            var actual = Parser.Parse("怎怎怎怎】$");
            var expected = new List<Expression> {
                new FunctionInvocationExpression(
                    new FunctionInvocationExpression(new AutoExpression(), "怎", new[]{
                    new FunctionInvocationExpression(new AutoExpression(), "怎", new[]{
                    new FunctionInvocationExpression(new AutoExpression(), "怎", new[]{
                    new FunctionInvocationExpression(new AutoExpression(), "怎", auto)})})}), "$",
                    auto)
            };
            Assert.IsTrue(EqualByProperties(actual, expected), actual.Dump());
        }

        [TestMethod]
        public void Parser_EmptyList() {
            var actual = Parser.Parse("[]");
            var expected = new List<Expression> {
                new ListExpression(none)
            };
            Assert.IsTrue(EqualByProperties(actual, expected), actual.Dump());
        }

        [TestMethod]
        public void Parser_1DList() {
            var actual = Parser.Parse("[12]");
            var expected = new List<Expression> {
                new ListExpression(new[]{new NumericLiteralExpression(12) })
            };
            Assert.IsTrue(EqualByProperties(actual, expected), actual.Dump());
        }

        [TestMethod]
        public void Parser_1DListMultipleItems() {
            var actual = Parser.Parse("[12$12$12$]");
            var expected = new List<Expression> {
                new ListExpression(new[]{
                    new FunctionInvocationExpression(new NumericLiteralExpression(12), "$", auto), 
                    new FunctionInvocationExpression(new NumericLiteralExpression(12), "$", auto),
                    new FunctionInvocationExpression(new NumericLiteralExpression(12), "$", auto) })
            };
            Assert.IsTrue(EqualByProperties(actual, expected), actual.Dump());
        }
        [TestMethod]
        public void Parser_1DListAsArgumentToTier2Func() {
            var actual = Parser.Parse("去[12$1");
            var expected = new List<Expression> {
                    new FunctionInvocationExpression(new AutoExpression(), "去", new[]{new ListExpression(new[]{
                        new FunctionInvocationExpression(new NumericLiteralExpression(12), "$", auto)
                    }) }), new NumericLiteralExpression(1)
                    };
            Assert.IsTrue(EqualByProperties(actual, expected), actual.Dump());
        }
    }
}
