using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Text;
using static TerseLang.Constants;
using System.Linq;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Linq.Expressions;
using System.Xml;
using System.Net;
using Lambda = System.Func<dynamic[], dynamic>;
using DList = System.Collections.Generic.List<dynamic>;

namespace TerseLang {
    public class FunctionHandler {

        //Returns base form of the function
        private static string GetBase(string func) {
            //Also checks if func is actually a function
            var tier = GetTier(func);
            if (!IsUnary(func)) {
                var tierList = new[] { TIER_UNLIMITED, TIER_ZERO, TIER_ONE, TIER_TWO }[tier + 1];
                func = TIER_UNLIMITED[tierList.IndexOf(func)].ToString();
            }
            return func;
        }

        public static int GetTier(String func) {
            if (IsUnary(func)) return 0;
            if (TIER_ZERO.Contains(func)) return 0;
            if (TIER_ONE.Contains(func)) return 1;
            if (TIER_TWO.Contains(func)) return 2;
            if (TIER_UNLIMITED.Contains(func)) return -1;
            throw new ArgumentException($"{func} is not a function");
        }

        public static bool IsUnary(String next) {
            return UNARY_FUNCTIONS.Contains(next);
        }
        public bool IsHigherOrder(string func, dynamic caller) {
            func = GetBase(func);
            if (IsUnary(func))
                return false;
            return BinaryFunctions[func].IsHigherOrder(caller);
        }

        public Lambda GetDefaultLambda(string func, dynamic caller) {
            func = GetBase(func);
            if (IsUnary(func)) ErrorHandler.InternalError($"Attempted to get default lambda from binary function {func}");
            return BinaryFunctions[func].GetDefaultLambda(caller);
        }
        public int NumLambdaParams(string func, dynamic caller) {
            func = GetBase(func);
            if (!IsHigherOrder(func, caller)) ErrorHandler.InternalError($"Attempted to get lambda parameters from non-higher-order function {func}");
            return BinaryFunctions[func].NumLambdaParams(caller);
        }

        public ProgramState ProgramState { get; }
        public FunctionHandler(ProgramState state) {
            ProgramState = state;
        }

        public dynamic InvokeUnary(string function, dynamic caller) {
            function = GetBase(function);
            if (!IsUnary(function)) ErrorHandler.Error($"No unary function: {function}");
            return UnaryFunctions[function].Invoke(caller);
        }

        public dynamic InvokeBinary(string function, dynamic x, dynamic y) {
            function = GetBase(function);
            if (IsUnary(function)) ErrorHandler.Error($"No binary function: {function}");
            return BinaryFunctions[function].Invoke(x, y);
        }

        //Unary Functions:
        //太该当经妈用打地再因呢女告最手前找行快而死先像等被从明中
        private readonly ReadOnlyDictionary<string, UnaryFunction> UnaryFunctions = new(new Dictionary<string, UnaryFunction> {
            ["太"] = new UnaryFunction {
                N = x => Math.Abs(x)
            }
        });

        //Binary Functions:
        //样也和下真现做大啊怎出点起天把开让给但谢着只些如家后儿多意别所话小自回然果发见心走定听觉
        private readonly ReadOnlyDictionary<string, BinaryFunction> BinaryFunctions = new(new Dictionary<string, BinaryFunction> {
            ["样"] = new BinaryFunction {
                NN = (x, y) => Math.Abs(x - y),
            },
            ["也"] = new BinaryFunction {
                NN = (x, y) => x + y
            }
        });
    }

    class UnaryFunction {
        public Func<double, dynamic> N { get; set; }
        public Func<string, dynamic> S { get; set; }
        public Func<DList, dynamic> L { get; set; }
        public dynamic Invoke(dynamic x) {
            try {
                return Behavior(x);
            }
            catch(NullReferenceException) {
                return null;
            }
        }
        private dynamic Behavior(double x) {
            return N(x);
        }
        private dynamic Behavior(string x) {
            return S(x);
        }
        public int? Depth { get; set; }
        private dynamic Behavior(DList x) {
            if ((Depth.HasValue && x.Depth() > Depth) || L == null) {
                return x.Select(item => Behavior(item)).ToList();
            }
            return L(x);
        }
    }
    class BinaryFunction {

        public dynamic Invoke(dynamic x, dynamic y) {
            //Let the type checker do its work...
            return Behavior(x, y);
        }
        public Func<double, double, dynamic> NN { get; set; }
        private dynamic Behavior(double x, double y) {
            return NN(x, y);
        }
        public Func<double, string, dynamic> NS { get; set; }
        private dynamic Behavior(double x, string y) {
            return NS(x, y);
        }
        public Func<double, DList, dynamic> NL { get; set; }
        private dynamic Behavior(double x, DList y) {
            if (RDepth.HasValue) {
                if (y.Depth() > RDepth)
                    return y.Select(item => Behavior(x, item)).ToList();
            }
            return NL(x, y);
        }
        public Func<string, double, dynamic> SN { get; set; }
        private dynamic Behavior(string x, double y) {
            return SN(x, y);
        }
        public Func<string, string, dynamic> SS { get; set; }
        private dynamic Behavior(string x, string y) {
            return SS(x, y);
        }
        public Func<string, DList, dynamic> SL { get; set; }
        private dynamic Behavior(string x, DList y) {
            if ((RDepth.HasValue && y.Depth() > RDepth) || SL == null) {
                return y.Select(item => Behavior(x, item)).ToList();
            }
            return SL(x, y);
        }
        public Func<DList, double, dynamic> LN { get; set; }
        private dynamic Behavior(DList x, double y) {
            if ((LDepth.HasValue && x.Depth() > LDepth) || LN == null) {
                    return x.Select(item => Behavior(item, y)).ToList();
            }
            return LN(x, y);
        }
        public Func<DList, string, dynamic> LS { get; set; }
        private dynamic Behavior(DList x, string y) {
            if ((LDepth.HasValue && x.Depth() > LDepth) || LS == null) {
                return x.Select(item => Behavior(item, y)).ToList();
            }
            return LS(x, y);
        }
        public Func<DList, DList, dynamic> LL { get; set; }
        private dynamic Behavior(DList x, DList y) {
            if (LDepth.HasValue) {
                if (x.Depth() > LDepth)
                    return x.Select(item => Behavior(item, y)).ToList();
            }
            if (RDepth.HasValue) {
                if (y.Depth() > RDepth)
                    return y.Select(item => Behavior(x, item)).ToList();
            }
            if (LL != null) 
                return LL(x, y);
            int min = Math.Min(x.Count, y.Count);
            return x.Zip(y, (xi, yi) => Behavior(xi, yi)).Concat(x.Skip(min)).Concat(y.Skip(min)).ToList();
        }

        public Func<double, Lambda, dynamic> NLambda { get; set; }
        private dynamic Behavior(double x, Lambda l) {
            return NLambda(x, l);
        }
        public Func<string, Lambda, dynamic> SLambda { get; set; }
        private dynamic Behavior(string x, Lambda l) {
            return SLambda(x, l);
        }
        public Func<DList, Lambda, dynamic> LLambda { get; set; }
        private dynamic Behavior(DList x, Lambda l) {
            if ((LDepth.HasValue && x.Depth() > LDepth) || LLambda == null) {
                return x.Select(item => Behavior(item, l)).ToList();
            }
            return LLambda(x, l);
        }

        public int? LDepth { get; set; }
        public int? RDepth { get; set; }

        public bool IsHigherOrder(DList caller) => LLambda != null;
        public bool IsHigherOrder(double caller) => NLambda != null;
        public bool IsHigherOrder(string caller) => SLambda != null;

        public Lambda NDefaultLambda { get; set; } = args => args[0];
        public Lambda SDefaultLambda { get; set; } = args => args[0];
        public Lambda LDefaultLambda { get; set; } = args => args[0];
        public Lambda GetDefaultLambda(double caller) => NDefaultLambda;
        public Lambda GetDefaultLambda(string caller) => SDefaultLambda;
        public Lambda GetDefaultLambda(DList caller) => LDefaultLambda;

        public int NLambdaParams { get; set; } = 1;
        public int SLambdaParams { get; set; } = 1;
        public int LLambdaParams { get; set; } = 1;

        public int NumLambdaParams(double caller) => NLambdaParams;
        public int NumLambdaParams(string caller) => SLambdaParams;
        public int NumLambdaParams(DList caller) => LLambdaParams;

    }
}
