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
            InitFunctions();
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
        ReadOnlyDictionary<string, UnaryFunction> UnaryFunctions;
        ReadOnlyDictionary<string, BinaryFunction> BinaryFunctions;

        private static bool Truthy(dynamic o) {
            if (o is double d) return d != 0;
            if (o is string s) return s != "";
            if (o is DList l) return l.Count != 0;
            return false;
        }
        private static DList Range(double start, double count) => Enumerable.Range((int)start, (int)count).Select(x => (dynamic)(double)x).ToList();
        private void InitFunctions() {
            //Unary Functions:
            //太该当经妈用打地再因呢女告最手前找行快而死先像等被从明中
            UnaryFunctions = new(new Dictionary<string, UnaryFunction> {
                ["太"] = new UnaryFunction {
                    N = x => Math.Abs(x)
                },
                ["该"] = new UnaryFunction {
                    N = x => x - 1
                },
                ["当"] = new UnaryFunction {
                    N = x => x.ToString().Distinct().ToList(),
                    S = x => string.Concat(x.Distinct().ToList()),
                    L = x => x.Distinct().ToList()
                },
                ["经"] = new UnaryFunction {
                    N = x => x * 2
                },
                ["妈"] = new UnaryFunction {
                    N = x => x / 2
                },
                ["用"] = new UnaryFunction {
                    N = x => x + 1
                },
                ["打"] = new UnaryFunction {
                    N = x => Convert.ToString((int)x, 2),
                    S = x => (double)Convert.ToInt32(x, 2),
                    L = x => (double)Convert.ToInt32(string.Concat(x), 2),
                },
                ["地"] = new UnaryFunction {
                    N = x => x.ToString().Length,
                    S = x => x.Length,
                    L = x => x.Count
                },
                ["再"] = new UnaryFunction {
                    N = x => x == 0 ? 1 : 0,
                    S = x => x == "" ? 1 : 0,
                    L = x => x.Count == 0 ? 1 : 0
                },
                ["因"] = new UnaryFunction {
                    N = x => -x
                },
                ["呢"] = new UnaryFunction {
                    L = x => x.Aggregate(1d, (a, b) => a * b),
                    Depth = 1
                },
                ["女"] = new UnaryFunction {
                    N = x => Range(1, x).Reverse<dynamic>().ToList(),
                    S = x => string.Concat(x.Reverse()),
                    L = x => x.Reverse<dynamic>().ToList()
                },
                ["告"] = new UnaryFunction {
                    N = x => Math.Sign(x)
                },
                ["最"] = new UnaryFunction {
                    N = x => x * x
                },
                ["手"] = new UnaryFunction {
                    N = x => Math.Sqrt(x)
                },
                ["前"] = new UnaryFunction {
                    S = x => {
                        DList l = new DList();
                        for (int i = 1; i <= x.Length; i++) {
                            for (int j = 0; j <= x.Length - i; j++) {
                                l.Add(x.Substring(j, i));
                            }
                        }
                        return l;
                    },
                    L = x => {
                        DList l = new DList();
                        for (int i = 1; i <= x.Count; i++) {
                            for (int j = 0; j <= x.Count - i; j++) {
                                l.Add(x.Skip(j).Take(i).ToList());
                            }
                        }
                        return l;
                    }
                },
                ["找"] = new UnaryFunction {
                    N = x => (char)x + "",
                    S = x => x.Length == 1 ? (double)x[0] : x.Select(a => (dynamic)(double)a).ToList()
                },
                ["行"] = new UnaryFunction {
                    N = x => x.ToString(),
                    S = x => double.Parse(x)
                },
                ["快"] = new UnaryFunction {
                    S = x => x.Split(" ").ToDList(),
                    L = x => string.Join(" ", x),
                    Depth = 1
                },
                ["而"] = new UnaryFunction {
                    S = x => x.Split("\n").ToDList(),
                    L = x => string.Join("\n", x),
                    Depth = 1
                },
                ["死"] = new UnaryFunction {
                    S = x => x.Select(a => (dynamic)(a + "")).ToList(),
                    L = x => string.Concat(x)
                },
                ["先"] = new UnaryFunction {
                    N = x => Range(1, x),
                    S = x => Range(1, x.Length),
                    L = x => Range(1, x.Count)
                },
                ["像"] = new UnaryFunction {
                    N = x => x % 2
                },
                ["等"] = new UnaryFunction {
                    N = x => (double)(x.ToString()[0] - 48),
                    S = x => x[0] + "",
                    L = x => x[0]
                },
                ["被"] = new UnaryFunction {
                    N = x => (double)(x.ToString().Last() - 48),
                    S = x => x.Last() + "",
                    L = x => x.Last()
                },
                ["从"] = new UnaryFunction {

                },
                ["明"] = new UnaryFunction {

                },
                ["中"] = new UnaryFunction {
                    N = x => new DList { x },
                    S = x => new DList { x },
                    L = x => new DList { x }
                },
                ["诉"] = new UnaryFunction {
                    N = x => {
                        ProgramState.Variables["间"] = x;
                        ProgramState.Autofill1Name = "间";
                        return x;
                    },
                    S = x => {
                        ProgramState.Variables["间"] = x;
                        ProgramState.Autofill1Name = "间";
                        return x;
                    },
                    L = x => {
                        ProgramState.Variables["间"] = x;
                        ProgramState.Autofill1Name = "间";
                        return x;
                    }
                }
            });

            //Binary Functions:
            //样也和下真现做大啊怎出点起天把开让给但谢着只些如家后儿多意别所话小自回然果发见心走定听觉
            BinaryFunctions = new(new Dictionary<string, BinaryFunction> {
                ["样"] = new BinaryFunction {
                    NN = (x, y) => Math.Abs(x - y),
                },
                ["也"] = new BinaryFunction {
                    NN = (x, y) => x + y
                },
                ["和"] = new BinaryFunction {
                    NN = (x, y) => x.ToString().Contains(y.ToString()) ? 1 : 0,
                    NS = (x, y) => y.Contains(x.ToString()) ? 1 : 0,
                    NL = (x, y) => y.Contains(x) ? 1 : 0
                    //TODO: rest of the contains
                },
                ["下"] = new BinaryFunction {
                    NN = (x, y) => x % y == 0 ? 1 : 0,
                },
                ["真"] = new BinaryFunction {
                    NN = (x, y) => x / y
                },
                ["现"] = new BinaryFunction {
                    NN = (x, y) => x == y ? 1 : 0,
                    NS = (x, y) => x.ToString() == y ? 1 : 0,
                    NL = (x, y) => 0,
                    SN = (x, y) => x == y.ToString() ? 1 : 0,
                    SS = (x, y) => x == y ? 1 : 0,
                    SL = (x, y) => 0,
                    LN = (x, y) => 0,
                    LS = (x, y) => 0,
                    LL = (x, y) => x.DListEquals(y) ? 1 : 0
                },
                ["做"] = new BinaryFunction {
                    NN = (x, y) => Range(1, x).Where(a => a % y == 0).ToList(),
                    SN = (x, y) => string.Concat(x.Where((_, i) => (i + 1) % y == 0)),
                    LN = (x, y) => x.Where((_, i) => (i + 1) % y == 0).ToList()
                },
                ["大"] = new BinaryFunction {
                    NN = (x, y) => Math.Pow(x, y)
                },
                ["啊"] = new BinaryFunction {
                    NLambda = (x, f) => Range(1, x).Where(a => Truthy(f(new dynamic[] { a }))).ToList(),
                    NLambdaParams = 1,
                    SLambda = (x, f) => string.Concat(x.Where((a, i) => Truthy(f(new dynamic[] { a.ToString(), (double)i })))),
                    SLambdaParams = 2,
                    LLambda = (x, f) => x.Where((a, i) => Truthy(f(new dynamic[] { a, (double)i }))),
                    LLambdaParams = 2
                },
                ["怎"] = new BinaryFunction {
                    NN = (x, y) => x > y ? 1 : 0
                },
                ["出"] = new BinaryFunction {
                    NN = (x, y) => Range(x, y - x + 1)
                },
                ["点"] = new BinaryFunction {
                    NN = (x, y) => (double)((int)x / (int)y)
                },
                ["起"] = new BinaryFunction {
                    NN = (x, y) => {
                        int xx = (int)x;
                        int b = (int)y;
                        if (b == 1) return new string('1', xx);
                        string chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
                        string str = "";
                        DList res = new DList();
                        for (; xx > 0; xx /= b) {
                            if (b > 36)
                                res.Insert(0, xx % b);
                            else
                                str = chars[xx % b] + str;
                        }
                        return b > 36 ? res : str;
                    }
                },
                ["天"] = new BinaryFunction {
                    NN = (x, y) => x < y ? 1 : 0
                },
                ["把"] = new BinaryFunction {
                    NN = (x, y) => x == 0 ? x : y,
                    NS = (x, y) => x == 0 ? x : y,
                    NL = (x, y) => x == 0 ? x : y,
                    SN = (x, y) => x == "" ? x : y,
                    SS = (x, y) => x == "" ? x : y,
                    SL = (x, y) => x == "" ? x : y,
                    LN = (x, y) => x.Count == 0 ? x : y,
                    LS = (x, y) => x.Count == 0 ? x : y,
                    LL = (x, y) => x.Count == 0 ? x : y
                },
                ["开"] = new BinaryFunction {
                    NN = (x, y) => x != 0 ? x : y,
                    NS = (x, y) => x != 0 ? x : y,
                    NL = (x, y) => x != 0 ? x : y,
                    SN = (x, y) => x != "" ? x : y,
                    SS = (x, y) => x != "" ? x : y,
                    SL = (x, y) => x != "" ? x : y,
                    LN = (x, y) => x.Count != 0 ? x : y,
                    LS = (x, y) => x.Count != 0 ? x : y,
                    LL = (x, y) => x.Count != 0 ? x : y
                },
                ["让"] = new BinaryFunction {
                    NLambda = (x, f) => Enumerable.Range(1, (int)x).Select(a => f(new dynamic[] { (double)a })).ToList(),
                    NLambdaParams = 1,
                    SLambda = (x, f) => x.Select((a, i) => f(new dynamic[] { a.ToString(), (double)i })).ToList(),
                    SLambdaParams = 2,
                    LLambda = (x, f) => x.Select((a, i) => f(new dynamic[] { a, (double)i })).ToList(),
                    LLambdaParams = 2
                },
                ["给"] = new BinaryFunction {
                    NN = (x, y) => Math.Max(x, y),
                    LLambda = (x, f) => x.Max(a => f(new dynamic[] { a })),
                    LLambdaParams = 1,
                },
                ["但"] = new BinaryFunction {
                    NN = (x, y) => Math.Min(x, y),
                    LLambda = (x, f) => x.Min(a => f(new dynamic[] { a })),
                    LLambdaParams = 1,
                },
                ["谢"] = new BinaryFunction {
                    NN = (x, y) => x % y
                },
                ["着"] = new BinaryFunction {
                    NN = (x, y) => x * y,
                    NS = (x, y) => new StringBuilder(y.Length * (int)x).Insert(0, y, (int)x).ToString(),
                    SN = (x, y) => new StringBuilder(x.Length * (int)y).Insert(0, x, (int)y).ToString()
                },
                ["只"] = new BinaryFunction {
                    NN = (x, y) => {
                        DList combos = new DList();
                        DList source = Range(1, x);
                        void Iterate(IEnumerable<int> indices, int s, int j, int n) {
                            if (j == n) {
                                combos.Add(indices.Select(l => source[l]).ToList());
                            }
                            else for (int i = s; i < x; i++) {
                                    Iterate(indices.Prepend(i), i + 1, j + 1, n);
                                }
                        }
                        Iterate(Enumerable.Empty<int>(), 0, 0, (int)y);
                        return combos;
                    },
                    SN = (x, y) => {
                        DList combos = new DList();

                        void Iterate(IEnumerable<int> indices, int s, int j, int n) {
                            if (j == n) {
                                combos.Add(indices.Select(l => (dynamic)(double)x[l]).ToList());
                            }
                            else for (int i = s; i < x.Length; i++) {
                                    Iterate(indices.Prepend(i), i + 1, j + 1, n);
                                }
                        }
                        Iterate(Enumerable.Empty<int>(), 0, 0, (int)y);
                        return combos;
                    },
                    LN = (x, y) => {
                        DList combos = new DList();

                        void Iterate(IEnumerable<int> indices, int s, int j, int n) {
                            if (j == n) {
                                combos.Add(indices.Select(l => x[l]).ToList());
                            }
                            else for (int i = s; i < x.Count; i++) {
                                    Iterate(indices.Prepend(i), i + 1, j + 1, n);
                                }
                        }
                        Iterate(Enumerable.Empty<int>(), 0, 0, (int)y);
                        return combos;
                    }
                },
                ["些"] = new BinaryFunction {
                    NN = (x, y) => x == y ? 0 : 1,
                    NS = (x, y) => x.ToString() == y ? 0 : 1,
                    NL = (x, y) => 1,
                    SN = (x, y) => x == y.ToString() ? 0 : 1,
                    SS = (x, y) => x == y ? 0 : 1,
                    SL = (x, y) => 1,
                    LN = (x, y) => 1,
                    LS = (x, y) => 1,
                    LL = (x, y) => x.DListEquals(y) ? 0 : 1
                },
                ["如"] = new BinaryFunction {
                    NN = (x, y) => x - y
                },
                ["家"] = new BinaryFunction {
                    //Cartesian product

                },
                ["后"] = new BinaryFunction {
                    //Chunk

                },
                ["儿"] = new BinaryFunction {
                    NN = (x, y) => x + "" + y,
                    NS = (x, y) => x + y,
                    NL = (x, y) => y.Prepend(x).ToList(),
                    SN = (x, y) => x + y,
                    SS = (x, y) => x + y,
                    SL = (x, y) => y.Prepend(x).ToList(),
                    LN = (x, y) => x.Append(y).ToList(),
                    LS = (x, y) => x.Append(y).ToList(),
                    LL = (x, y) => x.Concat(y).ToList()
                },
                ["多"] = new BinaryFunction {
                    LN = (x, y) => x.Count(a => a == y),
                    LS = (x, y) => x.Count(a => a == y),

                },
                ["意"] = new BinaryFunction {
                    NLambda = (x, f) => Range(1, x).Count(a => Truthy(f(new dynamic[] { a }))),
                    NLambdaParams = 1,
                    SLambda = (x, f) => x.Select((a, i) => new dynamic[] { a.ToString(), i }).Count(a => Truthy(f(a))),
                    SLambdaParams = 2,
                    LLambda = (x, f) => x.Select((a, i) => new dynamic[] { a, i }).Count(a => Truthy(f(a))),
                    LLambdaParams = 2
                },
                ["别"] = new BinaryFunction {
                    NLambda = (x, f) => Range(1, x).GroupBy(a => f(new dynamic[] { a })).Select(a => a.ToList()).ToList(),
                    NLambdaParams = 1,
                    SLambda = (x, f) => x.Select(a => a.ToString()).GroupBy(a => f(new dynamic[] { a })).Select(a => a.ToList()).ToList(),
                    SLambdaParams = 1,
                    LLambda = (x, f) => x.GroupBy(a => f(new dynamic[] { a })).Select(a => a.ToList()).ToList(),
                    LLambdaParams = 1
                },
                ["所"] = new BinaryFunction {
                    SN = (x, y) => x.IndexOf(y.ToString()) + 1,
                    SS = (x, y) => x.IndexOf(y) + 1,
                    LS = (x, y) => x.IndexOf(y) + 1,
                    LN = (x, y) => x.IndexOf(y) + 1,
                    LL = (x, y) => x.IndexOf(y) + 1
                },
                ["话"] = new BinaryFunction {
                    NL = (x, y) => y.Select((a, i) => a == x ? i : -1).Where(a => a != -1).ToDList(),
                    SS = (x, y) => x.Select((_, i) => x[i..].StartsWith(y) ? i : -1).Where(a => a != -1).ToDList(),
                    SL = (x, y) => y.Select((a, i) => a == x ? i : -1).Where(a => a != -1).ToDList(),
                    LN = (x, y) => x.Select((a, i) => a == y ? i : -1).Where(a => a != -1).ToDList(),
                    LS = (x, y) => x.Select((a, i) => a == y ? i : -1).Where(a => a != -1).ToDList(),
                    LL = (x, y) => y.Select((a, i) => a is DList d ? d.DListEquals(x) ?  i : -1 : -1).Where(a => a != -1).ToDList()
                },
                ["小"] = new BinaryFunction {
                    NLambda = (x, f) => Range(1, x).Any(a => Truthy(f(new dynamic[] { a }))),
                    NLambdaParams = 1,
                    SLambda = (x, f) => Range(1, x.Length).Any(a => Truthy(f(new dynamic[] { x[a]+"", a }))),
                    SLambdaParams = 2,
                    LLambda = (x, f) => Range(1, x.Count).Any(a => Truthy(f(new dynamic[] { x[a], a }))),
                    LLambdaParams = 2
                },
                ["自"] = new BinaryFunction {
                    LN = (x, y) => y >= 0 ? x.Skip((int)y).Concat(x.Take((int)y)) : x.TakeLast(-(int)y).Concat(x.SkipLast(-(int)y)),
                    SN = (x, y) => y >= 0 ? x[(int)y..] + x[..(int)y] : x[(^(int)-y)..] + x[..(^(int)-y)]
                },
                ["回"] = new BinaryFunction {
                    NS = (x, y) => (int)x < 0 ? y[..^((int)-x)] : y[(int)x..],
                    NL = (x, y) => (int)x < 0 ? y.SkipLast((int)-x) : y.Skip((int)x).ToList(),
                    SN = (x, y) => (int)y < 0 ? x[..^((int)-y)] : x[(int)y..],
                    LN = (x, y) => (int)y < 0 ? x.SkipLast((int)-y) : x.Skip((int)y).ToList()
                },
                ["然"] = new BinaryFunction {
                    SN = (x, y) => x.Split(y.ToString()).Select(a => (dynamic)a).ToList(),
                    SS = (x, y) => x.Split(y).Select(a => (dynamic)a).ToList()
                },
                ["果"] = new BinaryFunction {
                    NLambda = (x, f) => Range(1, (int)x).OrderBy(a => f(new dynamic[] { a })).ToList(),
                    NLambdaParams = 1,
                    SLambda = (x, f) => string.Concat(x.OrderBy(a => f(new dynamic[] { a.ToString() }))),
                    SLambdaParams = 1,
                    LLambda = (x, f) => x.OrderBy(a => f(new dynamic[] { a })).ToList(),
                    LLambdaParams = 1
                },
                ["发"] = new BinaryFunction {
                    NLambda = (x, f) => Range(1, x).Any(a => Truthy(f(new dynamic[] { a }))),
                    NLambdaParams = 1,
                    SLambda = (x, f) => Range(1, x.Length).Any(a => Truthy(f(new dynamic[] { x[a] + "", a }))),
                    SLambdaParams = 2,
                    LLambda = (x, f) => Range(1, x.Count).Any(a => Truthy(f(new dynamic[] { x[a], a }))),
                    LLambdaParams = 2
                },
                ["见"] = new BinaryFunction {
                    NS = (x, y) => (int)x < 0 ? y[..^((int)-x)] : y[..(int)x],
                    NL = (x, y) => (int)x < 0 ? y.SkipLast((int)-x) : y.Skip((int)x).ToList(),
                    SN = (x, y) => (int)y < 0 ? x[..^((int)-y)] : x[..(int)y],
                    LN = (x, y) => (int)y < 0 ? x.SkipLast((int)-y) : x.Skip((int)y).ToList()
                },
                ["心"] = new BinaryFunction {
                    NLambda = (x, f) => Range(1, (int)x).Aggregate((a, b) => f(new dynamic[] { a, b })),
                    NLambdaParams = 2,
                    NDefaultLambda = x => x[0] + x[1],
                    SLambda = (x, f) => x.Aggregate("", (a, b) => f(new dynamic[] { a.ToString(), b.ToString() })),
                    SLambdaParams = 2,
                    SDefaultLambda = x => x[0] + x[1],
                    LLambda = (x, f) => x.Aggregate((a, b) => f(new dynamic[] { a, b })),
                    LLambdaParams = 2,
                    LDefaultLambda = x => x[0] + x[1]
                },
                ["走"] = new BinaryFunction {
                    NN = (x, y) => new DList { x, y },
                    NS = (x, y) => new DList { x, y },
                    NL = (x, y) => new DList { x, y },
                    SN = (x, y) => new DList { x, y },
                    SS = (x, y) => new DList { x, y },
                    SL = (x, y) => new DList { x, y },
                    LN = (x, y) => new DList { x, y },
                    LS = (x, y) => new DList { x, y },
                    LL = (x, y) => new DList { x, y }
                },
                ["定"] = new BinaryFunction {
                    NS = (x, y) => y[(int)x].ToString(),
                    NL = (x, y) => y[(int)x],
                    SN = (x, y) => x[(int)y].ToString(),
                    LN = (x, y) => x[(int)y],
                },
                ["听"] = new BinaryFunction {
                    SN = (x, y) => x.Split(y.ToString()).ToDList(),
                    SS = (x, y) => x.Split(y).ToDList(),
                    SL = (x, y) => string.Join(x, y),
                    LN = (x, y) => string.Join(y.ToString(), x),
                    LS = (x, y) => string.Join(y, x),
                },
                ["觉"] = new BinaryFunction {
                    SN = (x, y) => x.Replace(y.ToString(), ""),
                    SS = (x, y) => x.Replace(y, ""),
                    SL = (x, y) => {
                        for(int i = 0; i < y.Count; i+=2) {
                            x = x.Replace(y[i], y[i + 1]);
                        }
                        return x;
                    },
                    LN = (x, y) => x.Where(a => a != y).ToList(),
                    LS = (x, y) => x.Where(a => a != y).ToList(),
                    LL = (x, y) => {
                        x = x.ToList();
                        for (int i = 0; i < y.Count; i += 2) {
                            for(int j = 0; j < x.Count; j++) {
                                if (x[j] == y[i]) x[j] = y[i + 1];
                            }
                        }
                        return x;
                    },
                }
            });
        }
    }

    class UnaryFunction {
        public Func<double, dynamic> N { get; set; }
        public Func<string, dynamic> S { get; set; }
        public Func<DList, dynamic> L { get; set; }
        public dynamic Invoke(dynamic x) {
            try {
                dynamic d = Behavior(x);
                if (d is int i) return (double)i;
                if (d is System.Collections.IEnumerable e) return e.ToDList();
                if (d is bool b) return b ? 1d : 0d;
                return d;
            }
            catch(NullReferenceException) {
                return 0;
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
            try {
                dynamic d = Behavior(x, y);
                if (d is int i) return (double)i;
                if (d is System.Collections.IEnumerable e) return e.ToDList();
                if (d is bool b) return b ? 1d : 0d;
                return d;
            }
            catch (NullReferenceException) {
                return 0;
            }
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
            if ((RDepth.HasValue && y.Depth() > RDepth) || NL == null) {
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
            if(x.Depth() > y.Depth()) return x.Select(item => Behavior(item, y)).ToList();
            if(y.Depth() > x.Depth()) return y.Select(item => Behavior(x, item)).ToList();
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
