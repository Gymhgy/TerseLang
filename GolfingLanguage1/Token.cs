using System;
using System.Collections.Generic;
using System.Text;

namespace TerseLang {
    public class Token {
        public string Value { get; }
        public TokenType Type { get; }

        public Token(string value, TokenType type) {
            Value = value; 
            Type = type;
        }
    }

    public enum TokenType {
        Number,
        String,
        Variable,
        Function,
        Punctuation,
        Modifier,
        Auto
    }
}
