using TerseLang;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace TerseLang.Tests {
    [TestClass]
    public class VObjectTests {
        [TestMethod]
        [DataRow(1d, "1")]
        [DataRow(2d, "2")]
        [DataRow(3d, "3")]
        [DataRow(3.23d, "3.23")]

        public void NumberToStringTest(double num, string expected) {
            var obj = new VObject(num);
            Assert.AreEqual(obj.ToString(), expected);
        }

        [TestMethod]
        [DataRow("a", "a")]
        [DataRow("abcd", "abcd")]
        [DataRow("a\"bv\\", "a\"bv\\")]

        public void StringToStringTest(string str, string expected) {
            var obj = new VObject(str);
            Assert.AreEqual(obj.ToString(), expected);
        }

        [TestMethod]
        public void ListToStringTest1() {
            var obj = new VObject(new List<VObject> { 1, 3, 2 });
            Assert.AreEqual(obj.ToString(), "[1, 3, 2]");
        }
        [TestMethod]
        public void ListToStringTest2() {
            var obj = new VObject(new List<VObject> { 1, "abc", 2 });
            Assert.AreEqual(obj.ToString(), "[1, \"abc\", 2]");
        }

        [TestMethod]

        public void ListToStringTest3() {
            var obj = new VObject(new List<VObject> { "hello", "abc", "world" });
            Assert.AreEqual(obj.ToString(), "[\"hello\", \"abc\", \"world\"]");
        }

        [TestMethod]

        public void NestedListToStringTest() {
            var obj = new VObject(new List<VObject> { new List<VObject> { 1 }, new List<VObject> { 2, 3 } });
            Assert.AreEqual(obj.ToString(), "[[1], [2, 3]]");
        }

        [TestMethod]

        public void StringWithQuotesInListToStringTest() {
            var obj = new VObject(new List<VObject> { "hello\\\"" });
            Assert.AreEqual(obj.ToString(), "[\"hello\\\\\\\"\"]");
        }
    }
}
