using System;
using System.Collections.Generic;
using System.Text;

namespace TerseLang {
    public static class ErrorHandler {
        public static void Error(string error) {
#if !DEBUG
            Console.Error.WriteLine(error);
            Environment.Exit(1);
#else
            InternalError(error);
#endif
        }

        public static void Error(string error, int line, int col) {
#if !DEBUG
            Console.Error.WriteLine($"({line}, {col}): {error}");
            Environment.Exit(1);
#else
            InternalError(error);
#endif
        }

        public static void InternalError(string error) {
            throw new InternalErrorException(error);
        }
    }
    public class InternalErrorException : Exception {
        public InternalErrorException(string error) : base(error) {

        }
    }

    
}
