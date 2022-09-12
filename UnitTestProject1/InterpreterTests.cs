﻿using TerseLang;
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
            var result = interpreter.Interpret().ToList();
            var expected = new List<dynamic> { 22222d };
            Assert.IsTrue(result.DListEquals(expected), result.Dump());
        }

        [TestMethod]
        public void Interpreter_String() {
            var interpreter = new Interpreter("\"Hello, World!", new dynamic[0]);
            var result = interpreter.Interpret().ToList();
            var expected = new List<dynamic> { "Hello, World!" };
            Assert.IsTrue(result.DListEquals(expected), result.Dump());
        }

        [TestMethod]
        [DataRow("22样", new object[] { 22d }, null, DisplayName = "Absolute Value")]
        /*[DataRow("22用", new object[] { 2 }, null, DisplayName = "Number Length")]
        [DataRow("22打", new object[] { -22 }, null, DisplayName = "Negation")]
        [DataRow("22地", new object[] { 44 }, null, DisplayName = "Double")]
        [DataRow("22再", new object[] { 11 }, null, DisplayName = "Halve")]
        [DataRow("22因", new object[] { 23 }, null, DisplayName = "Increment")]
        [DataRow("22呢", new object[] { 21 }, null, DisplayName = "Decrement")]
        [DataRow("22女", new object[] { "22" }, null, DisplayName = "Convert to String")]*/

        public void Interpreter_NumberUnaryFunctions(string program, object[] expected, object[] inputs = null) {
            inputs ??= new object[0];
            dynamic[] dInputs = inputs.ToArray();
            List<dynamic> dExpected = expected.ToList();

            var interpreter = new Interpreter(program, dInputs);
            var result = interpreter.Interpret() as DList;
            Assert.IsTrue(result.DListEquals(dExpected), result.Dump());
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
            DList dExpected = new DList { expected.ToDList() };
            dynamic[] dInputs = ((object[])inputs ?? new object[0]).ToDList().ToArray();
            var interpreter = new Interpreter("也", dInputs);
            var result = interpreter.Interpret() as DList;
            Assert.IsTrue(result.DListEquals(dExpected), result.Dump());
        }

        [TestMethod]
        public void Interpreter_FizzBuzz() {
            var interpreter = new Interpreter("电让一3我5u\"Fizz‘Buzz\"诉开】先", new dynamic[0]);
            var result = interpreter.Interpret() as DList;
            System.Console.WriteLine(result.Dump());
        }
    }
}
