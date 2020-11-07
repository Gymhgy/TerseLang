using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using static TerseLang.Constants;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Numerics;

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
                if (VARIABLES.Contains(next))
                    return new Token(next.ToString(), TokenType.Variable);
                if (PUNCTUATION.Contains(next))
                    return new Token(next.ToString(), TokenType.Punctuation);
                if (FUNCTIONS.Contains(next))
                    return new Token(next.ToString(), TokenType.Function);

            }
            throw new InvalidOperationException("This shouldn't happen.");
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
            while(!EOF() && PeekChar() != STRING_DELIMITER && PeekChar() != '\n') {
                var ch = ReadChar();
                if (ch == NEWLINE_SUBSTITUTE) {
                    str += "\n";
                }
                else if (CHARSET.Contains(ch) && (ch > 126 || ch < 32)) {
                    throw new NotImplementedException("No support for certain unicode characters in strings yet. " +
                        "Currently working on a feature that unicode characters do special things in strings.");
                }
                else str += ch;
            }
            if (!EOF()) ReadChar();
            return new Token(str, TokenType.String);
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


        #region String Compression

        public static BigInteger FromBase254(string str) {
            BigInteger res = 0, multiplier = 1;
            string map = CHARSET.Replace(STRING_SEPARATOR.ToString(), "").Replace(COMPRESSED_STRING_DELIMITER.ToString(), "");
            for(int i = str.Length - 1; i >= 0; i--) {
                res += map.IndexOf(str[i]) * multiplier;
                multiplier *= 254;
            }
            return res;
        }

        public static string ToBase254(BigInteger i) {
            string res = "";
            string map = CHARSET.Replace(STRING_SEPARATOR.ToString(), "").Replace(COMPRESSED_STRING_DELIMITER.ToString(), "");
            while(i > 0) {
                res = map[(int)(i % 254)] + res;
                i /= 254;
            }
            return res;
        }

        static readonly int longest = DICTIONARY.Max(x => x.Length);
        public static string CompressionAlgoA(string src) {
            BigInteger[] compressedInts = new BigInteger[src.Length + 1];
            for (int i = src.Length - 1; i >= 0; i--) {
                BigInteger compressed = compressedInts[i + 1];
                char target = src[i];
                if (target >= ' ' && target <= '~')
                    compressed = compressed * 96 + (target - 32);
                else if (target == '\n')
                    compressed = compressed * 96 + 95;
                else
                    throw new ArgumentException("A character in 'src' is neither a linefeed or printable ASCII.");
                compressed *= 2;
                compressedInts[i] = compressed;

                for(int j = i + 3; j < Math.Min(i + 31, src.Length + 1); j++) {
                    string word = src.Substring(i, j - i);
                    string entryWord = (word[0] == ' ' ? word.Substring(1) : word).ToUpper();
                    if (DICTIONARY.Contains(entryWord)) {
                        var wordIndex = DICTIONARY.IndexOf(entryWord);
                        int modifiers = 0;
                        if (word[0] == ' ') {
                            if (char.IsUpper(word[0]))
                                modifiers = 3;
                            else modifiers = 1;
                        }
                        if(char.IsUpper(word[0])) {
                            modifiers = 2;
                        }
                        BigInteger start = compressedInts[j];
                        start = start * 2 + 1;
                        start = start * DICTIONARY.Count + wordIndex;
                        start = start * 4 + modifiers;
                        compressedInts[i] = BigInteger.Min(compressedInts[i], start); 
                    }
                }
            }
            return ToBase254(compressedInts[0]);

        }

        public static string DecompressionAlgoA(string src) {
            BigInteger data = FromBase254(src);
            string res = "";
            //*96+index
            while (data > 0) {
                data = BigInteger.DivRem(data, 2, out BigInteger mode);
                if (mode == 0) {
                    data = BigInteger.DivRem(data, 96, out BigInteger charIndex);
                    res += charIndex == 95 ? '\n' : (char)(charIndex + 32);
                }
                else {
                    data = BigInteger.DivRem(data, DICTIONARY.Count, out BigInteger dictIndex);
                    string word = DICTIONARY[(int)dictIndex];
                    data = BigInteger.DivRem(data, 4, out mode);
                    switch ((int)mode) {
                        //Cap + Space
                        case 3:
                            word = " " + char.ToUpper(word[0]) + word.Substring(1);
                            break;
                        //Cap
                        case 2:
                            word = char.ToUpper(word[0]) + word.Substring(1);
                            break;
                        //Space
                        case 1:
                            word = " " + word;
                            break;
                        default: break;
                    }
                    res += word;
                }
            }
            return res;
        }

        #endregion
    }
} 