using TerseLang;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using static TerseLang.Tests.Utilities;

namespace TerseLang.Tests {
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
            Assert.IsTrue(TokenListEqual(actual, expected), actual.Dump());
        }

        [TestMethod]
        public void Tokenizer_NumberWithDecimalPoint() {
            var actual = GetTokens("12.8");
            var expected = new List<Token> { new Token("12.8", TokenType.Number) };
            Assert.IsTrue(TokenListEqual(actual, expected), actual.Dump());
        }

        [TestMethod]
        public void Tokenizer_NumberWithMultipleDecimalPoint() {
            var actual = GetTokens("12.8.2");
            var expected = new List<Token> { new Token("12.8", TokenType.Number), new Token("0.2", TokenType.Number) };
            Assert.IsTrue(TokenListEqual(actual, expected), actual.Dump());
        }
        [TestMethod]
        public void Tokenizer_NumberWithLeadingDot() {
            var actual = GetTokens(".2");
            var expected = new List<Token> { new Token("0.2", TokenType.Number) };
            Assert.IsTrue(TokenListEqual(actual, expected), actual.Dump());
        }

        [TestMethod]
        public void Tokenizer_Dot() {
            var actual = GetTokens(".感");
            var expected = new List<Token> { new Token(".", TokenType.Punctuation), new Token("感", TokenType.Variable) };
            Assert.IsTrue(TokenListEqual(actual, expected), actual.Dump());
        }

        [TestMethod]

        public void Tokenizer_Variable() {
            var expected = GetTokens("感");
            var actual = new List<Token> { new Token("感", TokenType.Variable) };
            Assert.IsTrue(TokenListEqual(actual, expected));
        }

        [TestMethod]
        public void Tokenizer_String() {
            var actual = GetTokens("“x“");
            var expected = new List<Token> { new Token("x", TokenType.String) };
            Assert.IsTrue(TokenListEqual(actual, expected));
        }
        [TestMethod]
        public void Tokenizer_Punctuation() {
            var expected = GetTokens(")");
            var actual = new List<Token> { new Token(")", TokenType.Punctuation) };
            Assert.IsTrue(TokenListEqual(actual, expected));
        }

        [TestMethod]
        public void Tokenizer_TripleCharString() {
            var expected = GetTokens("‘aaa");
            var actual = new List<Token> { new Token("aaa", TokenType.String) };
            Assert.IsTrue(TokenListEqual(actual, expected));
        }

        [TestMethod]
        public void Tokenizer_TripleCharStringEOF() {
            var expected = GetTokens("‘a");
            var actual = new List<Token> { new Token("a", TokenType.String) };
            Assert.IsTrue(TokenListEqual(actual, expected));
        }

        [TestMethod]
        public void Tokenizer_Function() {
            var actual = GetTokens("x");
            var expected = new List<Token> { new Token("x", TokenType.Function) };
            Assert.IsTrue(TokenListEqual(actual, expected), actual.Dump());
        }

        [TestMethod]
        public void Tokenizer_StringWithNoClosing() {
            var expected = GetTokens("“hello");
            var actual = new List<Token> { new Token("hello", TokenType.String) };
            Assert.IsTrue(TokenListEqual(actual, expected));
        }

        [TestMethod]
        public void Tokenizer_StringWithUnicodeFail() {
            Assert.ThrowsException<NotImplementedException>(() =>
            {
                GetTokens("“最");
            });
        }

        private static List<Token> GetTokens(string s) {
            List<Token> tokens = new List<Token>();
            Tokenizer t = new Tokenizer(s);
            while (!t.EOF()) {
                tokens.Add(t.Next());
            }

            return tokens;
        }

        private static bool TokenListEqual(IList<Token> a, IList<Token> b) {
            int i = 0;
            return a.All(x => x.Type == b[i].Type & x.Value == b[i++].Value) && a.Count == b.Count;
        }

        
    }
}
