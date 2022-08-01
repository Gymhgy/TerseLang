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
        private static Expression auto = new AutoExpression();

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
            var expected = new List<Expression> { auto.Invoke("$", auto) };
            Assert.IsTrue(EqualByProperties(actual, expected), actual.Dump());
        }

        [TestMethod]
        public void Parser_AutoAsArgument() {
            var actual = Parser.Parse("2$$2");
            var expected = new List<Expression> { 
                new NumericLiteralExpression(2).Invoke("$", auto).Invoke("$", auto),
                new NumericLiteralExpression(2)
            };
            Assert.IsTrue(EqualByProperties(actual, expected), actual.Dump());
        }

        [TestMethod]
        public void Parser_TiersOneAndZero() {
            var actual = Parser.Parse("感$x之$");
            var expected = new List<Expression> { 
                new VariableReferenceExpression("感").Invoke("$", auto).Invoke("x", new VariableReferenceExpression("之")).Invoke("$", auto)
            };
            Assert.IsTrue(EqualByProperties(actual, expected), actual.Dump());
        }

        [TestMethod]
        public void Parser_TierUnlimited() {
            var actual = Parser.Parse("点3因而");
            var expected = new List<Expression> { 
                auto.Invoke("点", new NumericLiteralExpression(3).Invoke("因").Invoke("而"))
            };
            Assert.IsTrue(EqualByProperties(actual, expected), actual.Dump());
        }

        [TestMethod]
        public void Parser_TierOverlap() {
            //2, U, 0, 0
            var actual = Parser.Parse("3有5怎6/$");
            var expected = new List<Expression> {
                new NumericLiteralExpression(3).Invoke("有", 
                    new NumericLiteralExpression(5).Invoke("怎", 
                        new NumericLiteralExpression(6).Invoke("/", auto).Invoke("$", auto)))
            };
            Console.WriteLine(actual.Dump());
            Console.WriteLine(expected.Dump());
            Assert.IsTrue(EqualByProperties(actual, expected), actual.Dump());
        }
        [TestMethod]
        public void Parser_Brackets() {
            var actual = Parser.Parse("3怎3怎}");
            var expected = new List<Expression> { 
                new NumericLiteralExpression(3).Invoke("怎", new NumericLiteralExpression(3).Invoke("怎", auto))
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
                    auto.Invoke("怎", auto.Invoke("怎", auto.Invoke("怎", auto.Invoke("怎", auto)))).Invoke("$", auto)
            };
            Assert.IsTrue(EqualByProperties(actual, expected), actual.Dump());
        }

        [TestMethod]
        public void Parser_HigherOrderTiers() {
            //1 U 1 U
            var actual = Parser.Parse("j因j因");
            var expected = new List<Expression> {
                auto.Invoke("j", auto.Invoke("因")).Invoke("j", auto.Invoke("因"))
            };
            Assert.IsTrue(EqualByProperties(actual, expected), actual.Dump() + "\n" + expected.Dump());
        }

		[TestMethod]
		public void Parser_BracketMultiple() {
			var actual = Parser.Parse("点点点点来)))用");
			var expected = Parser.Parse("点点点点来})用");
			Assert.IsTrue(EqualByProperties(actual, expected), actual.Dump() + "\n" + expected.Dump());
		}

        [TestMethod]
        public void Parser_CloseAll2() {
            var actual = Parser.Parse("定定定定】定");
            var expected = new List<Expression> {
                    auto.Invoke("定", auto.Invoke("定", auto.Invoke("定", auto.Invoke("定", auto)))).Invoke("定", auto)
            };
            Assert.IsTrue(EqualByProperties(actual, expected), actual.Dump());
        }

		[TestMethod]
		public void Parser_SimpleIf()
		{
			var actual = Parser.Parse("1?2..3");
			var expected = new List<Expression>
			{
				new ConditionalExpression(new NumericLiteralExpression(1), new NumericLiteralExpression(2), new NumericLiteralExpression(0.3))
			};
			Assert.IsTrue(EqualByProperties(actual, expected), actual.Dump());
		}

		[TestMethod]
		public void Parser_StringInterpolation() {
			var actual = Parser.Parse("“abcd定efgh情ijk");
			var expected = new List<Expression> {
				new InterpolatedStringExpression(new List<Expression> {
					new StringLiteralExpression("abcd"),
					new FunctionInvocationExpression(new AutoExpression(), "定", auto),
					new StringLiteralExpression("efgh"),
					new VariableReferenceExpression("情"),
					new StringLiteralExpression("ijk")
				})
			};

			Assert.IsTrue(EqualByProperties(actual, expected), actual.Dump());
		}

		[TestMethod]
		public void Parser_StringInterpolationWithBrackets() {
			var actual = Parser.Parse("“abcd定定定）定efgh定定定定】定ijk");
			var expected = new List<Expression> {
				new InterpolatedStringExpression(new List<Expression> {
                    new StringLiteralExpression("abcd"),
                    auto.Invoke("定", auto.Invoke("定", auto.Invoke("定", auto))).Invoke("定",auto),
                    new StringLiteralExpression("efgh"),
                    auto.Invoke("定", auto.Invoke("定", auto.Invoke("定", auto.Invoke("定", auto)))).Invoke("定", auto),
                    new StringLiteralExpression("ijk")
                })
			};

			Assert.IsTrue(EqualByProperties(actual, expected), actual.Dump());
		}

        [TestMethod]
        public void Parser_StringListInterpolation() {
            var actual = Parser.Parse("“abcd定‘efgh‘情ijk");
            var expected = new List<Expression> {
                new StringListExpression(new StringExpression[]{
                    new InterpolatedStringExpression(new List<Expression> {
                        new StringLiteralExpression("abcd"),
                        new FunctionInvocationExpression(new AutoExpression(), "定", auto),
                    }),
                    new StringLiteralExpression("efgh"),
                    new InterpolatedStringExpression(new List<Expression> {
                        new VariableReferenceExpression("情"),
                        new StringLiteralExpression("ijk")
                    })
                })
            };

            Assert.IsTrue(EqualByProperties(actual, expected), actual.Dump() + expected.Dump());
        }

        [TestMethod]
        public void Parser_LeftVectorize() {
            var actual = Parser.Parse("3。和3");
            var expected = new List<Expression> {
                new NumericLiteralExpression(3).Invoke("和", new NumericLiteralExpression(3)).LeftVectorize()

            };

            Assert.IsTrue(EqualByProperties(actual, expected), actual.Dump());
        }
        public void Parser_RightVectorize() {
            var actual = Parser.Parse("1，和1该2");
            var expected = new List<Expression> {
                new NumericLiteralExpression(1).Invoke("和", new NumericLiteralExpression(1)
                .Invoke("该", new NumericLiteralExpression(2))).RightVectorize()
            };

            Assert.IsTrue(EqualByProperties(actual, expected), actual.Dump());
        }
    }
}
