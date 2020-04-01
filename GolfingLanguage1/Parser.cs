using System;
using System.Collections.Generic;
using System.Text;
using GolfingLanguage1.Expressions;
using static GolfingLanguage1.Constants;

namespace GolfingLanguage1 {
    public class Parser {

        private readonly Stack<int> breaks = new Stack<int>();
        private readonly Tokenizer tokenizer;
        private Parser(Tokenizer tok) {
            tokenizer = tok;
        }

        public static List<Expression> Parse(string str) {
            return new Parser(new Tokenizer(str)).GetAST();
        }

        private List<Expression> GetAST() {
            var list = new List<Expression>();
            while(!tokenizer.EOF()) {
                list.Add(ParseExpression(true));
            }
            return list;
        }

        private int bracket = -1;
        private void ProcessBrackets() {
            if (tokenizer.EOF()) return;
            var tok = tokenizer.Peek();
            if (!IsBracket()) return;
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

        private Expression GetNextValue() {
            Expression ret = new AutoExpression();
            if(tokenizer.EOF()) {
                return ret;
            }
            var tok = tokenizer.Peek();
            if (tok.Type == TokenType.Number) ret = new NumericLiteralExpression(double.Parse(tok.Value));
            else if (tok.Type == TokenType.String) ret = new StringLiteralExpression(tok.Value);
            else if (tok.Type == TokenType.Variable) ret = new VariableReferenceExpression(tok.Value);
            if(!(ret is AutoExpression)) {
                tokenizer.Next();
                UpdateBreaks(false, true);
            }
            return ret;
        }

        private Expression ParseExpression(bool topLevel) {
            var val = GetNextValue();
            while (!tokenizer.EOF() && !ShouldExit() && tokenizer.Peek().Type == TokenType.Function) {
                var next = tokenizer.Next().Value;
                UpdateBreaks(false, true);
                if(BuiltinFunction.IsUnary(next)) {
                    val = new FunctionInvocationExpression(val, next, Array.Empty<Expression>()) ;
                }
                else {
                    var tier = BuiltinFunction.GetTier(next);
                    breaks.Push(tier);
                    List<Expression> args = new List<Expression>();
                    do {
                        var arg = ParseExpression(false);
                        args.Add(arg);
                    } while (tier < 0 && !tokenizer.EOF() && !IsBracket());
                    val = new FunctionInvocationExpression(val, next, args);
                }
                if (topLevel) {
                    bracket = -1;
                    while (IsBracket())
                        tokenizer.Next();
                    breaks.Clear();
                }
                else
                    ProcessBrackets();
            }
            UpdateBreaks(true, false);

            return val;
        }
    }
}