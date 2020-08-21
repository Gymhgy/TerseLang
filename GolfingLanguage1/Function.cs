using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Text;
using static GolfingLanguage1.Constants;
using System.Linq;
using System.Diagnostics;
using System.Diagnostics.Tracing;

namespace GolfingLanguage1 {
    public abstract class Function {

        public static int GetTier(string func) {
            if (IsUnary(func)) return 0;
            if (TIER_ZERO.Contains(func)) return 0;
            if (TIER_ONE.Contains(func)) return 1;
            if (TIER_TWO.Contains(func)) return 2;
            if (TIER_UNLIMITED.Contains(func)) return -1;
            throw new ArgumentException("func");
        }

        public static bool IsUnary(string next) {
            return UNARY_FUNCTIONS.Contains(next);
        }
        //abstract functions to be implemented in Function<T>
        public abstract VObject Invoke(VObject caller, params VObject[] args);

        public abstract VObject Invoke(VObject caller, Func<VObject[], VObject> lambda, params VObject[] args);
        public static Function Get(string func, ObjectType type) {
            //We don't need to check if func is actually a function because that is handled within GetTier()
            var tier = GetTier(func);
            if(!IsUnary(func)) {
                var tierList = new[] { TIER_UNLIMITED, TIER_ZERO, TIER_ONE, TIER_TWO }[tier + 1];
                func = TIER_UNLIMITED[tierList.IndexOf(func)].ToString();
            }
            return type switch
            {
                ObjectType.Number => NumericFunctions[func],
                ObjectType.String => StringFunctions[func],
                ObjectType.List => ListFunctions[func],
                _ => throw new Exception(),
            };
        }


        //Unary Functions:
        //经妈用打地再因呢女告最手前找行快而死先像等被从明中

        private static readonly ReadOnlyDictionary<string, Function<double>> NumericFunctions = new ReadOnlyDictionary<string, Function<double>>(new Dictionary<string, Function<double>> {
            #region Unary
            //Function "经", Unary
            //Stores the current value into the assignable variable "间"
            ["经"] = new Function<double>(x => {
                ProgramState.Variables["间"] = x;
                return x;
            }),

            //Function "妈", Unary
            //Makes the first autofill in this scope the current value
            ["妈"] = new Function<double>(x => {
                ProgramState.Variables["HiddenAutofill"] = x;
                ProgramState.Autofill1Name = "HiddenAutofill";
                return x;
            }),

            //Function "用", Unary
            //Return length of number, inc. decimal point
            ["用"] = new Function<double>(x => x.ToString().Length),

            //Function "打", Unary
            //Negate the number
            ["打"] = new Function<double>(x => -x),

            //Function "地", Unary
            //Double the number
            ["地"] = new Function<double>(x => x * 2),

            //Function "再", Unary
            //Halve (not rounded)
            ["再"] = new Function<double>(x => x / 2),

            //Function "因", Unary
            //Increment
            ["因"] = new Function<double>(x => x + 1),

            //Function "呢", Unary
            //Decrement
            ["呢"] = new Function<double>(x => x - 1),

            //Function "女", Unary
            //Convert to string
            ["女"] = new Function<double>(x => x.ToString()),

            //Function "告", Unary
            //Convert to base-10 digit array
            ["告"] = new Function<double>(x => x.ToString().Where(y => y != '.').Select(y => new VObject(char.GetNumericValue(y))).ToList()),

            //Function "最", Unary
            //Convert to hexadecimal string
            //Note: Rounds down
            ["最"] = new Function<double>(x => ((int)x).ToString("X")),

            //Function "手", Unary
            //Cpmvert to binary string
            //Note: Rounds down
            ["手"] = new Function<double>(x => ((int)x).ToString()),

            //Function "前", Unary
            //1 if 0, else 0
            ["前"] = new Function<double>(x => x == 0 ? 1 : 0),

            //Function "找", Unary
            //Absolute value
            ["找"] = new Function<double>(x => Math.Abs(x)),

            //Function "行", Unary
            //Factorial
            //Note: Returns the caller if caller is negative
            ["行"] = new Function<double>(x => x < 0 ? x : x == 0 ? 1d : Enumerable.Range(1, (int)x).Aggregate((c, n) => c * n)),

            //Function "快", Unary
            //Square root
            ["快"] = new Function<double>(x => Math.Sqrt(x)),

            //Function "而", Unary
            //Square
            ["而"] = new Function<double>(x => x * x),

            //Function "死", Unary
            //Range [0..x) (exclusive)
            ["死"] = new Function<double>(x => Enumerable.Range(0, (int)x).Select(n => (VObject)n).ToList()),

            //Function "先", Unary
            //Range [1..x] (inclusive)
            ["先"] = new Function<double>(x => Enumerable.Range(1, (int)x).Select(n => (VObject)n).ToList()),

            //Function "像", Unary
            //Binary complement
            ["像"] = new Function<double>(x => ~((int)x)),

            //Function "等", Unary
            //Number parity
            ["等"] = new Function<double>(x => (int)x % 2),

            //Function "被", Unary
            //Round up
            ["被"] = new Function<double>(x => Math.Ceiling(x)),

            //Function "从", Unary
            //Round down
            ["从"] = new Function<double>(x => Math.Floor(x)),

            //Function "明", Unary
            //Sign
            ["明"] = new Function<double>(x => Math.Sign(x)),

            //Function "中", Unary
            //To character
            ["中"] = new Function<double>(x => ((char)x).ToString()),
            #endregion

            #region Binary
            #endregion
        });
        private static readonly ReadOnlyDictionary<string, Function> StringFunctions = new ReadOnlyDictionary<string, Function>(new Dictionary<string, Function> {
            #region Unary
            //Function "经", Unary
            //Stores the current expression result into the assignable variable "间"
            ["经"] = new Function<string>(x => {
                ProgramState.Variables["间"] = x;
                return x;
            }),

            //Function "妈", Unary
            //Makes the first autofill in this scope the current value
            ["妈"] = new Function<string>(x => {
                ProgramState.Variables["HiddenAutofill"] = x;
                ProgramState.Autofill1Name = "HiddenAutofill";
                return x;
            }),

            //Function "用", Unary
            //Length
            ["用"] = new Function<string>(x => x.Length),

            //Function "打", Unary
            //Reverse
            ["打"] = new Function<string>(x => string.Concat(x.Reverse())),

            //Function "地", Unary
            //1st char
            ["地"] = new Function<string>(x => x[0]),

            //Function "再", Unary
            //Last char
            ["再"] = new Function<string>(x => x.Last()),

            //Function "因", Unary
            //To number (returns caller string if not parsable)
            ["因"] = new Function<string>(x => double.TryParse(x, out var r) ? (VObject)r : (VObject)x),

            //Function "呢", Unary
            //To char array
            ["呢"] = new Function<string>(x => x.Select(n => (VObject)n.ToString()).ToList()),

            //Function "女", Unary
            //Split on " "
            ["女"] = new Function<string>(x => x.Split(" ").Select(x => (VObject)x).ToList()),

            //Function "告", Unary
            //Split on newline "\n"
            ["告"] = new Function<string>(x => x.Split("\n").Select(x => (VObject)x).ToList()),

            //Function "最", Unary
            //Array of char values, or just an int if length 1
            ["最"] = new Function<string>(x => x.Select(n => new VObject((int)n)).ToList()),

            //Function "手", Unary
            //All permutations of the string
            ["手"] = new Function<string>(x => {
                List<VObject> permList = new List<VObject>();
                char[] chars = x.ToCharArray();
                int len = chars.Length;

                void Permutations(int i) {
                    if (i >= len - 1)
                        permList.Add(string.Concat(chars));
                    else {
                        Permutations(i + 1);
                        for(int j = i + 1; j < len; j++) {
                            (chars[i], chars[j]) = (chars[j], chars[i]);
                            Permutations(i + 1);
                            (chars[i], chars[j]) = (chars[j], chars[i]);
                        }
                    }
                }
                Permutations(0);
                return permList;
            }),
            #region Not Implemented Yet
            //Function "前", Unary
            //
            ["前"] = new Function<string>(x => x),

            //Function "找", Unary
            //
            ["找"] = new Function<string>(x => x),

            //Function "行", Unary
            //
            ["行"] = new Function<string>(x => x),

            //Function "快", Unary
            //
            ["快"] = new Function<string>(x => x),

            //Function "而", Unary
            //
            ["而"] = new Function<string>(x => x),

            //Function "死", Unary
            //
            ["死"] = new Function<string>(x => x),

            //Function "先", Unary
            //
            ["先"] = new Function<string>(x => x),

            //Function "像", Unary
            //
            ["像"] = new Function<string>(x => x),

            //Function "等", Unary
            //
            ["等"] = new Function<string>(x => x),

            //Function "被", Unary
            //
            ["被"] = new Function<string>(x => x),

            //Function "从", Unary
            //
            ["从"] = new Function<string>(x => x),

            //Function "明", Unary
            //
            ["明"] = new Function<string>(x => x),

            //Function "中", Unary
            //
            ["中"] = new Function<string>(x => x)
            #endregion
            #endregion

            #region Binary
            #endregion
        });
        private static readonly ReadOnlyDictionary<string, Function> ListFunctions = new ReadOnlyDictionary<string, Function>(new Dictionary<string, Function>
        {
            #region Unary
            //Function "经", Unary
            //Stores the current expression result into the assignable variable "间"
            ["经"] = new Function<List<VObject>>(x => {
                ProgramState.Variables["间"] = x;
                return x;
            }),

            //Function "妈", Unary
            //Makes the first autofill in this scope the current value
            ["妈"] = new Function<List<VObject>>(x => {
                ProgramState.Variables["HiddenAutofill"] = x;
                ProgramState.Autofill1Name = "HiddenAutofill";
                return x;
            }),

            //Function "用", Unary
            //Length
            ["用"] = new Function<List<VObject>>(x => x.Count),

            //Function "打", Unary
            //Reverse the list
            ["打"] = new Function<List<VObject>>(x => { x.Reverse(); return x; }),

            //Function "地", Unary
            //1st element
            ["地"] = new Function<List<VObject>>(x => x[0]),

            //Function "再", Unary
            //Last element
            ["再"] = new Function<List<VObject>>(x => x.Last()),

            //Function "因", Unary
            //Sort
            ["因"] = new Function<List<VObject>>(x => x.OrderBy(n => n).ToList()),

            //Function "呢", Unary
            //Concatenate
            ["呢"] = new Function<List<VObject>>(x => string.Concat(x)),

            //Function "女", Unary
            //Join w/ " "
            ["女"] = new Function<List<VObject>>(x => string.Join(" ", x)),

            //Function "告", Unary
            //Join w/ "\n"
            ["告"] = new Function<List<VObject>>(x => string.Join("\n", x)),

            //Function "最", Unary
            //Sum (Map strings by parsing them or 0, lists to 0)
            ["最"] = new Function<List<VObject>>(x => x.Sum(n => {
                switch(n.ObjectType) {
                    case ObjectType.Number:
                        return n;
                    case ObjectType.String:
                        return double.TryParse(n, out double d) ? d : 0;
                    case ObjectType.List:
                        return 0;
                }
                throw new Exception("Object is not of type number, list or string in function 最");

            })),

            //Function "手", Unary
            //Product (Map strings by parsing them or 1, lists to 1)
            ["手"] = new Function<List<VObject>>(x => x.Aggregate((product, n) => {
                switch (n.ObjectType) {
                    case ObjectType.Number:
                        return n * product;
                    case ObjectType.String:
                        return double.TryParse(n, out double d) ? product * d : (double) product;
                    case ObjectType.List:
                        return product;
                }
                throw new Exception("Object is not of type number, list or string in function 手");
            })),

            //Function "前", Unary
            //Flatten Completely
            ["前"] = new Function<List<VObject>>(x => {
                List<VObject> flattened = new List<VObject>();
                void Search(List<VObject> l) {
                    foreach(var obj in l) {
                        if (obj.ObjectType == ObjectType.List)
                            Search(obj);
                        else
                            flattened.Add(obj);
                    }
                }
                return flattened;
            }),

            //Function "找", Unary
            //Remove first
            ["找"] = new Function<List<VObject>>(x => x.Skip(1).ToList()),

            //Function "行", Unary
            //Max (strings to parsed or NegativeInfinity, lists to NegativeInfinity)
            ["行"] = new Function<List<VObject>>(x => x.Max(n => {
                switch (n.ObjectType) {
                    case ObjectType.Number:
                        return n;
                    case ObjectType.String:
                        return double.TryParse(n, out double d) ? d : double.NegativeInfinity;
                    case ObjectType.List:
                        return double.NegativeInfinity;
                }
                throw new Exception("Object is not of type number, list or string in function 行");
            })),

            //Function "快", Unary
            //Max (strings to parsed or PositiveInfinity, lists to PositiveInfinity)
            ["快"] = new Function<List<VObject>>(x => x.Min(n => {
                switch (n.ObjectType) {
                    case ObjectType.Number:
                        return n;
                    case ObjectType.String:
                        return double.TryParse(n, out double d) ? d : double.PositiveInfinity;
                    case ObjectType.List:
                        return double.PositiveInfinity;
                }
                throw new Exception("Object is not of type number, list or string in function 快");
            })),

            //Function "而", Unary
            //Cumulative Sum (strings to parsed or 0, lists to 0)
            ["而"] = new Function<List<VObject>>(x => {
                //A very "hacky" method
                double sum = 0;
                //Modifying sum in a select query...
                return x.Select(n => {
                    switch (n.ObjectType) {
                        case ObjectType.Number:
                            return new VObject(sum += n);
                        case ObjectType.String:
                            return new VObject(sum += double.TryParse(n, out double d) ? d : 0);
                        case ObjectType.List:
                            return new VObject(sum += 0);
                    }
                    throw new Exception("Object is not of type number, list or string in function 行");
                }).ToList();
            }),

            //Function "死", Unary
            //Subsections (inc. self)
            ["死"] = new Function<List<VObject>>(x => {
                List<VObject> subsecs = new List<VObject>();
                for (int i = 0; i < x.Count; i++) {
                    for(int j = x.Count - i; j > 0; j--) {
                        subsecs.Add(x.Skip(i).Take(j).ToList());
                    }
                }
                return subsecs;
            }),

            //Function "先", Unary
            //Permutations
            ["先"] = new Function<List<VObject>>(x => {
                List<VObject> permList = new List<VObject>();
                int len = x.Count;

                void Permutations(int i) {
                    if (i >= len - 1)
                        permList.Add(x.ToList());
                    else {
                        Permutations(i + 1);
                        for (int j = i + 1; j < len; j++) {
                            (x[i], x[j]) = (x[j], x[i]);
                            Permutations(i + 1);
                            (x[i], x[j]) = (x[j], x[i]);
                        }
                    }
                }
                Permutations(0);
                return permList;
            }),

            //Function "像", Unary
            //Combinations
            ["像"] = new Function<List<VObject>>(x => {
                List<VObject> combos = new List<VObject>();
                /*
                 * Based off of https://stackoverflow.com/a/127856
                */
                int len = x.Count;
                for(int i = 1; i <= len; i++) {
                    Iterate(Enumerable.Empty<int>(), 0, 0, i);
                }
                void Iterate(IEnumerable<int> indices, int s, int j, int n) {
                    if (j == n) {
                        combos.Add(indices.Select(l => x[l]).ToList());
                    }
                    else for (int i = s; i < len; i++) {
                            Iterate(indices.Prepend(i), i + 1, j + 1, n);
                    }
                }       
                return combos;
            }),

            //Function "等", Unary
            //Group
            ["等"] = new Function<List<VObject>>(x => x.GroupBy(n => n).Select(n => (VObject)n.ToList()).ToList()),

            //Function "被", Unary
            //
            ["被"] = new Function<List<VObject>>(x => x),

            //Function "从", Unary
            //
            ["从"] = new Function<List<VObject>>(x => x),

            //Function "明", Unary
            //
            ["明"] = new Function<List<VObject>>(x => x),

            //Function "中", Unary
            //
            ["中"] = new Function<List<VObject>>(x => x)
            #endregion            

            #region Binary
            #endregion
        });
    }

    public class Function<T> : Function {

        //Designates how much parameters a lambda that is passed into this function takes
        //0 means it doesn't take a lambda at all
        public int LambdaParameters { get; } = 0;

        //For regular functions
        private Func<T, VObject[], VObject> Behavior { get; }
        //For higher-order functions
        private Func<T, Func<VObject[], VObject>, VObject[], VObject> LambdaBehavior { get; }

        //For functions taking more than one parameter
        public Function(Func<T, VObject[], VObject> behavior) {
            Behavior = behavior;
        }
        //For Unary Functions
        public Function(Func<T, VObject> behavior) {
            Behavior = new Func<T, VObject[], VObject>((input, _) => behavior(input));
        }
        //For funtions taking only one parameter
        public Function(Func<T, VObject, VObject> behavior) {
            Behavior = new Func<T, VObject[], VObject>((input, argument) => behavior(input, argument[0]));
        }

        //For higher-order functions
        public Function(Func<T, Func<VObject[], VObject>, VObject[], VObject> behavior, int lambdaParameters) {
            LambdaBehavior = behavior;
            LambdaParameters = lambdaParameters;
        }
        //For higher-order functions that don't take additional arguments besides the function
        public Function(Func<T, Func<VObject[], VObject>, VObject> behavior, int lambdaParameters) {
            LambdaBehavior = new Func<T, Func<VObject[], VObject>, VObject[], VObject>((input, func, _) => behavior(input, func));
            LambdaParameters = lambdaParameters;
        }

        public override VObject Invoke(VObject caller, params VObject[] args) {
            return Behavior(caller.ConvertTo<T>(), args);
        }

        public override VObject Invoke(VObject caller, Func<VObject[], VObject> lambda, params VObject[] args) {
            return LambdaBehavior(caller.ConvertTo<T>(), lambda, args);
        }
    }
}
