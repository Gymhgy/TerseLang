using System;
using System.Collections.Generic;
using System.Text;
using GolfingLanguage1.Expressions;
using static GolfingLanguage1.Constants;

namespace GolfingLanguage1 {
    public class Parser {

        private readonly Stack<int> breaks = new Stack<int>();
        private readonly Tokenizer tokenizer;
        public Parser(Tokenizer tok) {
            tokenizer = tok;
        }

        public List<Expression> GetAST() {
            var list = new List<Expression>();
            while(!tokenizer.EOF()) {
                list.Add(ParseExpression());
                if (!tokenizer.EOF() && BRACKETS.Contains(tokenizer.Peek().Value) && tokenizer.Peek().Type == TokenType.Punctuation) {
                    bracket = -1;
                    tokenizer.Next();
                }
            }
            return list;
        }

        private int bracket = -1;
        private bool AttemptExitBracket() {
            if (tokenizer.EOF()) return false;
            var tok = tokenizer.Peek();
            if (tok.Type != TokenType.Punctuation) return false;
            if (tok.Value == CLOSE_ALL.ToString()) return false;
            if (BRACKETS.Contains(tok.Value)) {
                if (bracket == -1) {
                    bracket = BRACKETS.IndexOf(tokenizer.Peek().Value);
                }
                else {
                    bracket--;
                }
                if(bracket == 0) {
                    tokenizer.Next();
                    return true;
                }
            }
            return false;
        }

        private bool UpdateBreaks() {
            var x = ShouldExit();
            if (x) breaks.Pop();
            else breaks.Push(breaks.Pop() - 1);
            return x;
        }

        private bool ShouldExit() {
            return breaks.Count == 0 || breaks.Peek() == 0;
        }

        private bool GetNextValue(out Expression ret) {
            ret = null;
            if (tokenizer.EOF()) return false;
            var tok = tokenizer.Peek();
            if(ShouldExit()) {
                ret = new AutoExpression(); 
                return false;
            }
            if (tok.Type == TokenType.Number) ret = new NumericLiteralExpression(double.Parse(tok.Value));
            else if (tok.Type == TokenType.String) ret = new StringLiteralExpression(tok.Value);
            else if (tok.Type == TokenType.Variable) ret = new VariableReferenceExpression(tok.Value);
            if(ret != null) {
                tokenizer.Next();
                ret = new AutoExpression();
            }
            return UpdateBreaks();
        }

        private Expression ParseExpression() {
            GetNextValue(out var val);
            while (!tokenizer.EOF() && !ShouldExit() && tokenizer.Peek().Type == TokenType.Function) {
                var next = tokenizer.Next().Value;
                breaks.Push(BuiltinFunction.GetTier(next));
                var func = BuiltinFunction.Get(next);
                if(BuiltinFunction.IsUnary(next)) {
                    val = new FunctionInvocationExpression(func, true);
                }
                else {
                    var arg = ParseExpression();
                    val = new FunctionInvocationExpression(func, arg);
                }
                if (AttemptExitBracket()) break;
            }
            return val;
        }
    }
}