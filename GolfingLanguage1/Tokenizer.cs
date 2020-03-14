using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using static GolfingLanguage1.Constants;

namespace GolfingLanguage1 {
    public class Tokenizer {
        private readonly StringReader reader;
        private Token current = null;

        private char PeekChar() => (char)reader.Peek();
        private char ReadChar() => (char)reader.Read();

        private string ReadWhile(Predicate<char> f) {
            string res = "";
            while (!EOF() && f(PeekChar())) {
                res += ReadChar();
            }
            return res;
        }

        public Tokenizer(string start) { 
            reader = new StringReader(start);
            // Initialize current
            current = ReadNext();
        }

        public Token Next() {
            var tok = current;
            current = null;
            return tok ?? ReadNext();
        }

        public Token Peek() {
            current ??= ReadNext();
            return current;
        }

        private Token ReadNext() {
            if (!EOF()) {
                var next = ReadChar();
                if (char.IsDigit(next))
                    return ReadNumber();
                if (next == STRING_DELIMITER)
                    return ReadString();
                if (next == COMPRESSED_STRING_DELIMITER)
                    return ReadCompressedString();
                if (VARIABLES.Contains(next))
                    return new Token(next.ToString(), TokenType.Variable);
                if (PUNCTUATION.Contains(next))
                    return new Token(next.ToString(), TokenType.Punctuation);

            }
            throw new InvalidOperationException();
        }

        //TODO
        private Token ReadCompressedString() {
            var str = ReadWhile(x => x != COMPRESSED_STRING_DELIMITER);
            throw new NotImplementedException();
        }

        private Token ReadString() {
            var str = ReadWhile(x => x != STRING_DELIMITER);
            return new Token(str, TokenType.String);
        }

        private Token ReadNumber() {
            bool encounteredDot = false;
            var num = ReadWhile(x => {
                if (char.IsDigit(x))
                    return true;
                if (!encounteredDot && x == '.') {
                    encounteredDot = true;
                    return true;
                }
                return false;
            });
            return new Token(num, TokenType.Number);
        }

        public bool EOF() => reader.Peek() == -1;

    }
} 