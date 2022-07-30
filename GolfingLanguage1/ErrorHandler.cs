using System;
using System.Collections.Generic;
using System.Text;

namespace TerseLang {
    public static class ErrorHandler {
        public static void Error(string error) {
#if !DEBUG
            throw new ProgramErrorException(error);
#else
            InternalError(error);
#endif
        }

        public static void Error(string error, int line, int col) {
#if !DEBUG
            throw new ProgramErrorException($"({line}, {col}): {error}");
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
    public class ProgramErrorException : Exception {
        public ProgramErrorException(string error) : base(error) {

        }
    }


}
