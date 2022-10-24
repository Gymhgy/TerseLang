using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using static TerseLang.Constants;

namespace TerseLang {
    public class Tokenizer {
        private readonly StringReader reader;
        private Token current = null;

        private char PeekChar() => (char)reader.Peek();
        private char ReadChar() => (char)reader.Read();

        // Helper function
        // Reads until EOF() or the predicate returns false for the next character
        private string ReadWhile(Predicate<char> f) {
            string res = "";
            while (!EOF() && f(PeekChar())) {
                res += ReadChar();
            }
            return res;
        }

        public Tokenizer(string start) { 
            reader = new StringReader(start);
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
                var next = PeekChar();
                if (char.IsDigit(next))
                    return ReadNumber();
                next = ReadChar();
                if (next == '.') 
                    return HandleDot();
                if (next == STRING_DELIMITER)
                    return ReadString();
                int i = 0;
                if (next == SINGLE_CHAR_STRING) 
                    return new Token(ReadWhile(_ => i++ < 1), TokenType.String);
                if (next == DOUBLE_CHAR_STRING)
                    return new Token(ReadWhile(_ => i++ < 2), TokenType.String);
                if (next == TRIPLE_CHAR_STRING)
                    return new Token(ReadWhile(_ => i++ < 3), TokenType.String);
                if (next == COMPRESSED_STRING_DELIMITER)
                    return ReadCompressedString();
                if (next == COMPRESSED_NUMBER_DELIMITER)
                    return ReadCompressedNumber();

                if (VARIABLES.Contains(next))
                    return new Token(next.ToString(), TokenType.Variable);
                if (PUNCTUATION.Contains(next))
                    return new Token(next.ToString(), TokenType.Punctuation);
                if (FUNCTIONS.Contains(next))
                    return new Token(next.ToString(), TokenType.Function);

                if (next == ADDITIONAL_FUNCTIONS)
                    return new Token(next + ReadWhile(_ => i++ < 1), TokenType.Function);
            }
            throw new InvalidOperationException("Attempted to read next token, but encountered EOF");
        }

        private Token ReadCompressedNumber() {
            throw new NotImplementedException("Compressed Numbers are not yet implemented.");
        }

        // Handles two cases:
        // Case 1: .### (where ### is any number of digits)
        // Returns a token representing a number, where the number is < 1
        // Case 2: .x (where x is anything else)
        // Not implemented yet, hoping this could be something that modifies function behaviour
        private Token HandleDot () {
            if(!EOF()) {
                var str = "";
                while (char.IsDigit(PeekChar()))
                    str += ReadChar();
                if (str != "") return new Token("0." + str, TokenType.Number);
                else return new Token(".", TokenType.Punctuation);
            }
            //Single dot
            return new Token(".", TokenType.Punctuation);
        }

        //TODO
        private Token ReadCompressedString() {
            var str = ReadWhile(x => x != COMPRESSED_STRING_DELIMITER);
            if (!EOF()) ReadChar();

            throw new NotImplementedException();
        }

        private Token ReadString() {
            var str = "";
            TokenType type = TokenType.String;
            while(!EOF() && PeekChar() != STRING_DELIMITER && PeekChar() != '\n') {
                var ch = ReadChar();
                str += ch;
            }
            if (!EOF()) ReadChar();
            return new Token(str, type);
        }

        // Reads number in form of ##.## (where # are digits)
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

        public bool EOF() => reader.Peek() == -1 && current == null;

    }
} 