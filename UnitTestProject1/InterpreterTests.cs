using TerseLang;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace TerseLang.Tests {
    [TestClass]
    public class InterpreterTests {
        [TestMethod]
        public void Interpreter_Number() {
            var interpreter = new Interpreter("22222", new VObject[0]);
            var result = new VObject(interpreter.Interpret());
            var expected = new List<VObject> { new VObject(22222) };
            Assert.IsTrue(result.Equals(new VObject(expected)), result.Dump());
        }

        [TestMethod]
        public void Interpreter_String() {
            var interpreter = new Interpreter("“Hello, World!", new VObject[0]);
            var result = new VObject(interpreter.Interpret());
            var expected = new List<VObject> { new VObject("Hello, World!") };
            Assert.IsTrue(result.Equals(new VObject(expected)), result.Dump());
        }

        [TestMethod]
        [DataRow("22用", new object[] { 2 }, null, DisplayName = "Number Length")]
        [DataRow("22打", new object[] { -22 }, null, DisplayName = "Negation")]
        [DataRow("22地", new object[] { 44 }, null, DisplayName = "Double")]
        [DataRow("22再", new object[] { 11 }, null, DisplayName = "Halve")]
        [DataRow("22因", new object[] { 23 }, null, DisplayName = "Increment")]
        [DataRow("22呢", new object[] { 21 }, null, DisplayName = "Decrement")]
        [DataRow("22女", new object[] { "22" }, null, DisplayName = "Convert to String")]

        public void Interpreter_NumberUnaryFunctions(string program, object[] expected, object[] inputs = null) {
            inputs ??= new object[0];
            var vInputs = inputs.Select(x => new VObject(x)).ToArray();
            var vExpected = new VObject(expected.Select(x => new VObject(x)).ToList());

            var interpreter = new Interpreter(program, vInputs);
            var result = new VObject(interpreter.Interpret());
            Assert.IsTrue(result.Equals(vExpected), result.Dump());
        }

        [TestMethod]
        public void Interpreter_LambdaTest() {
            var interpreter = new Interpreter("点最因中", new VObject[] { "string" });
            var result = new VObject(interpreter.Interpret());
            var expected = new List<VObject> { new List<VObject> { "t","u","s","j","o","h" } };
            Assert.IsTrue(result.Equals(new VObject(expected)), result.Dump());
        }

        [TestMethod]
        public void Interpreter_FilterTest() {
            var interpreter = new Interpreter("“string“起当's", new VObject[0]);
            var result = new VObject(interpreter.Interpret());
            var expected = new List<VObject> { "s" };
            Assert.IsTrue(result.Equals(new VObject(expected)), result.Dump());
        }

        [TestMethod]
        public void Interpreter_HigherOrderTiers() {
            var interpreter = new Interpreter("j因j因", new VObject[] { new List<VObject> { 1, 2, 3 } });
            var result = new VObject(interpreter.Interpret());
            var expected = new List<VObject> { new List<VObject> { 3, 4, 5 } };
            Assert.IsTrue(result.Equals(new VObject(expected)), result.Dump());
        }

        [TestMethod]
        public void Interpreter_LambdaParameterTest() {
            var interpreter = new Interpreter("点斯J起它不1", new VObject[] { new List<VObject> { new List<VObject> { 1, 2 } } } );
            var result = new VObject(interpreter.Interpret());
            var expected = new List<VObject> { new List<VObject> { new List<VObject> {1 } } };
            Assert.IsTrue(result.Equals(new VObject(expected)), result.Dump());
        }

        [TestMethod]
        public void Interpreter_PairLambda() {
            var interpreter = new Interpreter("电如3了5", new VObject[] { } );
            var result = new VObject(interpreter.Interpret());
            var expected = new List<VObject> { Enumerable.Repeat(new VObject(new List<VObject> { 3, 5 }), 100).ToList() };
            Assert.IsTrue(result.Equals(new VObject(expected)), result.Dump());
        }

        [TestMethod]
        public void Interpreter_MultMapLambda() {
            var interpreter = new Interpreter("a0点真2", new VObject[] { new int[] { 1, 2, 3 }.ToVList() });
            var result = new VObject(interpreter.Interpret());
            var expected = new List<VObject> { new int[] { 2, 4, 6, 0 }.ToVList() };
            Assert.IsTrue(result.Equals(new VObject(expected)), result.Dump());
        }
        [TestMethod]
        public void Interpreter_RightVectorize() {
            var interpreter = new Interpreter("1，和1该2", new VObject[] { });
            var result = new VObject(interpreter.Interpret());
            var expected = new List<VObject> { new int[] { 2, 3 }.ToVList() };
            Assert.IsTrue(result.Equals(new VObject(expected)), result.ToString());
        }
        [TestMethod]
        public void Interpreter_LeftVectorize() {
            var interpreter = new Interpreter("3。和3", new VObject[] { });
            var result = new VObject(interpreter.Interpret());
            var expected = new List<VObject> { new int[] { 4, 5, 6 }.ToVList() };
            Assert.IsTrue(result.Equals(new VObject(expected)), result.ToString());
        }
    }
}
