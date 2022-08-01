using System;
using System.Collections.Generic;
using System.Text;

namespace TerseLang {
    public static class ErrorHandler {
        public static void Error(string error) {
            throw new ProgramErrorException(error);
        }

        public static void Error(string error, int line, int col) {
            throw new ProgramErrorException($"({line}, {col}): {error}");
        }

        public static void InternalError(string error) {
            throw new InternalErrorException(error);
        }
    }
    public class InternalErrorException : Exception {
        public InternalErrorException(string error) : base(error) {

        }
    }
    public class ProgramErrorException : Exception {
        public ProgramErrorException(string error) : base(error) {

        }
    }


}
