using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using TerseLang.Expressions;
using static TerseLang.Constants;

namespace TerseLang {
    public class Parser {

        // The breaks "break" a ParseExpression call
        // It prevents it from applying a function to it
        private readonly Stack<int> breaks = new Stack<int>();


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
            }
        }

        private bool IsBracket() {
            if (tokenizer.EOF()) return false;
            var tok = tokenizer.Peek();
            return tok.Type == TokenType.Punctuation && (BRACKETS.Contains(tok.Value) || CLOSE_ALL.ToString() == tok.Value);
        }

        private void UpdateBreaks(bool pop, bool update) {
            var x = ShouldExit();
            if (x) {
                if(pop)
                breaks.Pop(); 
            }
            else {
                if (update && breaks.Count > 0)
                    breaks.Push(breaks.Pop() - 1);
            }
        }

        private bool ShouldExit() {
            return breaks.Count != 0 && breaks.Peek() == 0;
        }

        // Based off of the next token, return the appropriate value
        // If the next token is punctuation or a function, return an AutoExpression
        private Expression GetNextValue() {
            Expression ret = new AutoExpression();
            if(tokenizer.EOF()) {
                return ret;
            }
            var tok = tokenizer.Peek();
            if (tok.Type == TokenType.Number) ret = new NumericLiteralExpression(double.Parse(tok.Value));
            else if (tok.Type == TokenType.String) ret = new StringLiteralExpression(tok.Value);
            else if (tok.Type == TokenType.Variable) ret = new VariableReferenceExpression(tok.Value);
            else if (tok.Type == TokenType.Punctuation && tok.Value == LIST_START.ToString()) {
                throw new NotImplementedException();
            }
            if(!(ret is AutoExpression)) {
                tokenizer.Next();
                UpdateBreaks(false, true);
            }
            return ret;
        }

        // An expression starts out with a starting value, which we get from GetNextValue
        // Functions are applied to that starting value
        // Each function has a "tier", which means how many tokens can go after it
        private Expression ParseExpression(bool topLevel) {
            var val = GetNextValue();
            while (!tokenizer.EOF() && !ShouldExit() && tokenizer.Peek().Type == TokenType.Function) {
                // Another token has been consumed; therefore we need to update the breaks
                var next = tokenizer.Next().Value;
                UpdateBreaks(false, true);

                if(Function.IsUnary(next)) {
                    val = new FunctionInvocationExpression(val, next, Array.Empty<Expression>()) ;
                }
                else {
                    var tier = Function.GetTier(next);
                    breaks.Push(tier);
                    List<Expression> args = new List<Expression>();
                    var arg = ParseExpression(false);
                    args.Add(arg);
                    val = new FunctionInvocationExpression(val, next, args);
                }
                // Get rid of all the brackets if this expression is a top level one
                // And also clear out all the breaks
                if (topLevel) {
                    bracket = -1;
                    while (IsBracket())
                        tokenizer.Next();
                    breaks.Clear();
                }
                else
                    ProcessBrackets();
                //If a '?' follows this expression, it means it's a conditional expression
                if(!tokenizer.EOF() && tokenizer.Peek().Type == TokenType.Punctuation && tokenizer.Peek().Value == IF.ToString()) {
                    val = new ConditionalExpression(val, ParseExpression(false), ParseExpression(false));
                }
            }
            UpdateBreaks(true, false);

            return val;
        }
    }
}