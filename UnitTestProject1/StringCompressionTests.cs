using TerseLang;
using TerseLang.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using static TerseLang.Tests.Utilities;
using System.IO;

namespace TerseLang.Tests {

    [TestClass]
    public class StringCompressionTests {
        [TestMethod]
        public void CompressionAlgoA() {
            foreach (string src in File.ReadAllLines("UnitTestProject1/StringCompressionTestsCases.txt")) {
                Assert.AreEqual(Tokenizer.DecompressionAlgoA(Tokenizer.CompressionAlgoA(src)), src, Tokenizer.CompressionAlgoA(src));
            }
        }

        [TestMethod]
        public void CompressionComparison() {
            foreach (string src in File.ReadAllLines("UnitTestProject1/StringCompressionTestsCases.txt")) {
                var compressedA = Tokenizer.CompressionAlgoA(src);
                Console.WriteLine("'{0}' -> {1}\nCompression rate: {2}", src, compressedA, compressedA.Length / src.Length);
            }
        }
    }
}
