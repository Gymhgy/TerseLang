using TerseLang;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using static TerseLang.Tests.Utilities;
using DList = System.Collections.Generic.List<dynamic>;

namespace TerseLang.Tests {
    [TestClass]
    public class InterpreterTests {
        [TestMethod]
        public void Interpreter_Number() {
            var interpreter = new Interpreter("22222", new dynamic[0]);
            var result = interpreter.Interpret() as object;
            var expected = 22222d;
            Assert.IsTrue(D.Equals(result, expected), result.Dump());
        }

        [TestMethod]
        public void Interpreter_String() {
            var interpreter = new Interpreter("\"Hello, World!", new dynamic[0]);
            var result = interpreter.Interpret() as object;
            var expected = "Hello, World!";
            Assert.IsTrue(D.Equals(result, expected), result.Dump());
        }

        [TestMethod]
        [DataRow("22样", 22d, null, DisplayName = "Absolute Value")]
        /*[DataRow("22用", new object[] { 2 }, null, DisplayName = "Number Length")]
        [DataRow("22打", new object[] { -22 }, null, DisplayName = "Negation")]
        [DataRow("22地", new object[] { 44 }, null, DisplayName = "Double")]
        [DataRow("22再", new object[] { 11 }, null, DisplayName = "Halve")]
        [DataRow("22因", new object[] { 23 }, null, DisplayName = "Increment")]
        [DataRow("22呢", new object[] { 21 }, null, DisplayName = "Decrement")]
        [DataRow("22女", new object[] { "22" }, null, DisplayName = "Convert to String")]*/

        public void Interpreter_NumberUnaryFunctions(string program, dynamic expected, object[] inputs = null) {
            inputs ??= new object[0];
            dynamic[] dInputs = inputs.ToArray();

            var interpreter = new Interpreter(program, dInputs);
            var result = interpreter.Interpret() as object;
            Assert.IsTrue(D.Equals(result, expected), result.Dump());
        }

        [TestMethod]
        [DataRow(new object[] { 6d, 7d, 8d }, new object[] { 1d, 2d, 3d }, 5d, DisplayName = "Left Vectorization D1")]
        [DataRow(new object[] { 6d, 7d, 8d }, 5d, new object[] { 1d, 2d, 3d }, DisplayName = "Right Vectorization D1")]
        [DataRow(new object[] { 6d, 7d, 8d }, new object[] {5d, 5d, 5d}, new object[] { 1d, 2d, 3d }, DisplayName = "Left-Right Vectorization D1")]
        [DataRow(new object[] { new object[] { 6d, 7d, 8d } }, new object[] { new object[] { 5d, 5d, 5d } }, new object[] { 1d, 2d, 3d }, DisplayName = "Left-Right Vectorization D2 D1")]
        [DataRow(new object[] { 6d, 7d, 8d, 5d }, new object[] { 5d, 5d, 5d, 5d }, new object[] { 1d, 2d, 3d }, DisplayName = "Extra elements")]
        [DataRow(new object[] { new object[] { 2d, 3d, 4d }, new object[] { 2d,3d,4d } }, new object[] { new object[] { 1d, 1d, 1d }, 1d }, new object[] { 1d, 2d, 3d }, DisplayName = "Extra elements vectorize")]
        [DataRow(new object[] { new object[] { 2d, 3d, 4d }, new object[] { 2d, 3d, 4d } }, new object[] { new object[] { 1d, 1d, 1d }, 1d }, new object[] { 1d, 2d, 3d }, DisplayName = "Extra elements vectorize")]
        public void Interpreter_AdditionVectorization(object[] expected, params object[] inputs) {
            dynamic[] dInputs = ((object[])inputs ?? new object[0]).ToDList().ToArray();
            var dExpected = expected.ToDList();
            var interpreter = new Interpreter("也", dInputs);
            var result = interpreter.Interpret() as object;
            Assert.IsTrue(D.Equals(result, dExpected), result.Dump());
        }

        [TestMethod]
        public void Interpreter_FizzBuzz() {
            var interpreter = new Interpreter("电让一3我5u\"Fizz‘Buzz\"死开】而", new dynamic[0]);
            var result = interpreter.Interpret() as object;
            System.Console.WriteLine(result.Dump());
        }

        [TestMethod]
        public void Interpreter_FizzaBuzz() {
            var interpreter = new Interpreter("5u\"Fizz\"", new dynamic[0]);
            var result = interpreter.Interpret() as object;
            System.Console.WriteLine(result.Dump());
        }

        [TestMethod]
        public void Interpreter_Rule110() {
            var interpreter = new Interpreter("0^^0{3让打子220打)死哦，下】", new dynamic[] {"00000000000001", 10d});
            var result = interpreter.Interpret() as object;
            System.Console.WriteLine(result.Dump());
        }

        [TestMethod]
        public void Interpreter_HyperbinaryVectorization() {
            var interpreter = new Interpreter("让该K让0k2Y打#)是'/", new dynamic[] { 3 });
            var result = interpreter.Interpret() as object;
            System.Console.WriteLine(result.Dump());
        }
    }
}
