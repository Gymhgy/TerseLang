using GolfingLanguage1;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using static UnitTestProject1.Utilities;

namespace UnitTestProject1 {
    [TestClass]
    public class TokenizerTests {

        [TestMethod]
        public void Tokenizer_Empty() {
            var actual = GetTokens("");
            var expected = new List<Token>();
            Assert.IsTrue(TokenListEqual(actual, expected));
        }

        [TestMethod]

        public void Tokenizer_Number() {
            var actual = GetTokens("12");
            var expected = new List<Token> {new Token("12", TokenType.Number)};
            Assert.IsTrue(TokenListEqual(actual, expected), string.Join(" ", actual.Select(TokenToString)));
        }

        [TestMethod]

        public void Tokenizer_Variable() {
            var actual = GetTokens("已");
            var expected = new List<Token> { new Token("已", TokenType.Variable) };
            Assert.IsTrue(TokenListEqual(actual, expected));
        }

        [TestMethod]
        public void Tokenizer_String() {
            var actual = GetTokens("“x“");
            var expected = new List<Token> { new Token("x", TokenType.String) };
            Assert.IsTrue(TokenListEqual(actual, expected), string.Join(" ", actual.Select(TokenToString)));
        }


        private static List<Token> GetTokens(string s) {
            List<Token> tokens = new List<Token>();
            Tokenizer t = new Tokenizer(s);
            while (!t.EOF()) {
                tokens.Add(t.Next());
            }

            return tokens;
        }

        private static string TokenToString(Token t) {
            return "{ " + t.Value + ", " + t.Type + " }";
        }

        private static bool TokenListEqual(IList<Token> a, IList<Token> b) {
            int i = 0;
            return a.All(x => x.Type == b[i].Type & x.Value == b[i++].Value) && a.Count == b.Count;
        }

        
    }
}
