using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using TerseLang.Expressions;
using static TerseLang.Constants;

namespace TerseLang {
    public class Parser {

        private readonly Tokenizer tokenizer;
        private Parser(Tokenizer tok) {
            tokenizer = tok;
        }

        public static List<Expression> Parse(string program) {
            return new Parser(new Tokenizer(program)).GetAST();
        }

        private List<Expression> GetAST() {
            var list = new List<Expression>();
            while(!tokenizer.EOF()) {
                list.Add(ParseExpression(true));
            }
            return list;
        }

        // A bracket closes a function call
        // For example, 5怎1) is equivalent to 5.RandomFunction(1)
        // The ')' bracket serves the same purpose as the ')' in the pseudocode example,
        // which is to note the end of the argument
        // Some brackets can close more than one function call
        // For example, 5怎1怎2} is equivalent to 5.RandomFunction(1.RandomFunction(2))
        // The '}' bracket is equivalent to "))" in the pseudocode
        // '）' is equal to ")))", anbd the CLOSE_ALL bracket '】' closes all function calls, no matter what
        private int bracket = -1;
        private void ProcessBrackets() {
            if (tokenizer.EOF()) return;
            var tok = tokenizer.Peek();
            if (!IsBracket()) return;
            // We don't need to deal with the CLOSE_ALL bracket here
            // The CLOSE_ALL bracket is dealt with in ParseExpression
            if (tok.Value == CLOSE_ALL.ToString()) return;
            if (bracket == -1) {
                bracket = BRACKETS.IndexOf(tokenizer.Peek().Value);
            }
            else {
                bracket--;
            }
            if(bracket == 0) {
                tokenizer.Next();
                bracket = -1;
            }
        }

        private bool IsBracket() {
            if (tokenizer.EOF()) return false;
            var tok = tokenizer.Peek();
            return tok.Type == TokenType.Punctuation && (BRACKETS.Contains(tok.Value) || CLOSE_ALL.ToString() == tok.Value);
        }



        // Based off of the next token, return the appropriate value
        // If the next token is punctuation or a function, return an AutoExpression
        private Expression GetNextValue(ref int toks) {
            Expression ret = new AutoExpression();
            
            if(tokenizer.EOF()) {
                return ret;
            }
            if (toks == 0) {
                return ret;
            }
            var tok = tokenizer.Peek();
            if (tok.Type == TokenType.Number) ret = new NumericLiteralExpression(double.Parse(tok.Value));
            else if (tok.Type == TokenType.String) ret = new StringLiteralExpression(tok.Value);
            else if (tok.Type == TokenType.Variable) ret = new VariableReferenceExpression(tok.Value);

            if(!(ret is AutoExpression) && !(ret is ListExpression)) {
                toks--;
                tokenizer.Next();
            }
            return ret;
        }

        // An expression starts out with a starting value, which we get from GetNextValue
        // Functions are applied to that starting value
        // Each function has a "tier", which means how many tokens can go after it
        // toks: the max amount of tokens that should be parsed (determined by tier)
        private Expression ParseExpression(bool topLevel=false, int toks = -1) {
            var val = GetNextValue(ref toks);
            while (!tokenizer.EOF() && toks != 0 && tokenizer.Peek().Type == TokenType.Function) {
                // Another token has been consumed; therefore we need to update the breaks
                var next = tokenizer.Next().Value;
                toks--; 

                if(Function.IsUnary(next)) {
                    val = new FunctionInvocationExpression(val, next, Array.Empty<Expression>()) ;
                }
                else {
                    var tier = Function.GetTier(next);
                    List<Expression> args = new List<Expression>();
                    var arg = ParseExpression(false, tier);
                    args.Add(arg);
                    val = new FunctionInvocationExpression(val, next, args);
                }
                // Get rid of all the brackets if this expression is a top level one
                // And also clear out all the breaks
                if (topLevel) {
                    bracket = -1;
                    while (IsBracket())
                        tokenizer.Next();
                }
                else
                    ProcessBrackets();
            }
            //If a '?' follows this expression, it means it's a conditional expression
            if (!tokenizer.EOF() && tokenizer.Peek().Type == TokenType.Punctuation && tokenizer.Peek().Value == IF.ToString())
            {
                tokenizer.Next();
                val = new ConditionalExpression(val, ParseExpression(), ParseExpression());
            }
            return val;
        }
    }
}