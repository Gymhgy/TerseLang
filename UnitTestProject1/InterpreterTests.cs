using GolfingLanguage1;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace GolfingLanguage1.Tests {
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
            var interpreter = new Interpreter("“string", new VObject[0]);
            var result = new VObject(interpreter.Interpret());
            var expected = new List<VObject> { new VObject("string") };
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

        //Wraps the single element into a VObject list
        private static List<VObject> Wrap(VObject x) => new List<VObject> { x };
    }
}
