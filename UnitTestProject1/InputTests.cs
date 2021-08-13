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
            var tests = new (string test, VObject[] expected)[]
            {
                ("['abcdefg', 0.24, \"hello world\"]", new VObject[]{"abcdefg",0.24,"hello world" }),
                ("['abcdefg\"\"', 0.24, \"'hello world'\"]", new VObject[]{"abcdefg\"\"",0.24,"'hello world'" }),
                ("[\"[']'\"]", new VObject[]{ "[']'"})

            };
            foreach (var (test, expected) in tests) {
                var r = Program.ParseInput(test);
                Assert.IsTrue(new VObject(expected.ToList()).Equals(r), r.Dump());
            }
        }

        [TestMethod]
        public void Input_2DList() {
            var tests = new (string test, VObject[] expected)[]
            {
                ("[[0,1,2], [3,4,5], [6,7,8]]", new VObject[]{ new List<VObject> { 0,1,2 } , new List<VObject> { 3,4,5 }, new List<VObject> { 6,7,8 } }),
                ("[[\"abc\"], [\"def\", '[]'], [\"'ghi'\"]]", new VObject[]{ new List<VObject> { "abc" } , new List<VObject> { "def", "[]" }, new List<VObject> { "'ghi'" } }),
                ("[['abc']]", new VObject[]{ new List<VObject> { "abc" } }),
                ("[[1,2]]", new VObject[]{ new List<VObject> { 1,2 } })

            };
            foreach (var (test, expected) in tests) {
                var r = Program.ParseInput(test);
                Assert.IsTrue(new VObject(expected.ToList()).Equals(r), r.Dump());
            }
        }

        [TestMethod]
        public void Input_NestedLists() {
            var tests = new (string test, VObject[] expected)[]
            {
                ("[[],[[]]]", new VObject[]{  new List<VObject> { } , new List<VObject> { new List<VObject> { } } }),
                ("[]", new VObject[]{ }),
                ("[[]]", new VObject[]{ new List<VObject> { } })

            };
            foreach (var (test, expected) in tests) {
                var r = Program.ParseInput(test);
                Assert.IsTrue(new VObject(expected.ToList()).Equals(r), r.Dump());
            }
        }
    }
}
