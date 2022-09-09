using TerseLang;
using TerseLang.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using static TerseLang.Tests.Utilities;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace TerseLang.Tests {
    [TestClass]
    public class InputTests {
        [TestMethod]
        public void Input_Number() {
            for(double i = -10; i < 10; i += .1) {
                var r = Program.ParseInput(i.ToString());
                Assert.AreEqual(i, (double)r);
            }
        }

        [TestMethod]
        public void Input_DoubleQuoteString() {
            var tests = new[]
            {
                "abcefgh",
                "hello world",
                "terse is great"
            };
            foreach(var l in tests) {
                var r = Program.ParseInput('"' + l + '"');
                Assert.AreEqual(l, (string)r);
            }
        }

        [TestMethod]
        public void Input_SingleQuoteString() {
            var tests = new[]
            {
                "abcefgh",
                "hello world",
                "terse is great"
            };
            foreach (var l in tests) {
                var r = Program.ParseInput('\'' + l  + '\'');
                Assert.AreEqual(l, (string)r);
            }
        }

        [TestMethod]
        public void Input_1DList() {
            var tests = new (string test, List<dynamic> expected)[]
            {
                ("['abcdefg', 0.24, \"hello world\"]", new List<dynamic>{"abcdefg",0.24,"hello world" }),
                ("['abcdefg\"\"', 0.24, \"'hello world'\"]", new List<dynamic>{"abcdefg\"\"",0.24,"'hello world'" }),
                ("[\"[']'\"]", new List<dynamic>{ "[']'"})

            };
            foreach (var (test, expected) in tests) {
                var r = Program.ParseInput(test) as List<dynamic>;
                Assert.IsTrue(expected.DListEquals(r), r.Dump());
            }
        }

        [TestMethod]
        public void Input_2DList() {
            var tests = new (string test, List<dynamic> expected)[]
            {
                ("[[0,1,2], [3,4,5], [6,7,8]]", new List<dynamic>{ new List<dynamic> { 0d,1d,2d } , new List<dynamic> { 3d,4d,5d }, new List<dynamic> { 6d,7d,8d } }),
                ("[[\"abc\"], [\"def\", '[]'], [\"'ghi'\"]]", new List<dynamic>{ new List<dynamic> { "abc" } , new List<dynamic> { "def", "[]" }, new List<dynamic> { "'ghi'" } }),
                ("[['abc']]", new List<dynamic>{ new List<dynamic> { "abc" } }),
                ("[[1,2]]", new List<dynamic>{ new List<dynamic> { 1d,2d } })

            };
            foreach (var (test, expected) in tests) {
                var r = Program.ParseInput(test) as List<dynamic>;
                Assert.IsTrue(expected.DListEquals(r), r.Dump());
            }
        }

        [TestMethod]
        public void Input_NestedLists() {
            var tests = new (string test, List<dynamic> expected)[]
            {
                ("[[],[[]]]", new List<dynamic>{  new List<dynamic> { } , new List<dynamic> { new List<dynamic> { } } }),
                ("[]", new List<dynamic>{ }),
                ("[[]]", new List<dynamic>{ new List<dynamic> { } })

            };
            foreach (var (test, expected) in tests) {
                var r = Program.ParseInput(test) as List<dynamic>;
                Assert.IsTrue(expected.DListEquals(r), r.Dump());
            }
        }
    }
}
