using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Text;
using static GolfingLanguage1.Constants;
using System.Linq;

namespace GolfingLanguage1 {
    public class Function {
        //For regular functions
        private Func<VObject, VObject[], VObject> Behavior { get; }
        //For higher-order functions
        private Func<VObject, Func<VObject[], VObject>, VObject[], VObject> LambdaBehavior { get; }

        //Designates how much parameters a lambda that is passed into this function takes
        //0 means it doesn't take a lambda at all
        public int LambdaParameters { get; }

        //For functions taking more than one parameter
        private Function (Func<VObject, VObject[], VObject> behavior) {
            Behavior = behavior;
            LambdaParameters = 0;
        }
        //For Unary Functions
        private Function(Func<VObject, VObject> behavior) {
            Behavior = new Func<VObject, VObject[], VObject>((input, _) => behavior(input));
            LambdaParameters = 0;
        }
        //For funtions taking only one parameter
        private Function(Func<VObject, VObject, VObject> behavior) {
            Behavior = new Func<VObject, VObject[], VObject>((input, argument) => behavior(input, argument[0]));
            LambdaParameters = 0;
        }
        
        //For higher-order functions
        private Function(Func<VObject, Func<VObject[], VObject>, VObject[], VObject> behavior, int lambdaParameters) {
            LambdaBehavior = behavior;
            LambdaParameters = lambdaParameters;
        }
        //For higher-order functions that don't take additional arguments besides the function
        private Function(Func<VObject, Func<VObject[], VObject>, VObject> behavior, int lambdaParameters) {
            LambdaBehavior = new Func<VObject, Func<VObject[], VObject>, VObject[], VObject>((input, func, _) => behavior(input, func));
            LambdaParameters = lambdaParameters;
        }

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
        public VObject Invoke(VObject caller, params VObject[] args) {
            return Behavior(caller, args);
        }

        public VObject Invoke(VObject caller, Func<VObject[], VObject> lambda, params VObject[] args) {
            return LambdaBehavior(caller, lambda, args);
        }
        public static Function Get(string func, ObjectType type) {
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

        private static readonly ReadOnlyDictionary<string, Function> NumericFunctions = new ReadOnlyDictionary<string, Function>(new Dictionary<string, Function> {
            //Function "经", Unary
            //Stores the current value into the assignable variable "间"
            ["经"] = new Function(x => {
                ProgramState.Variables["间"] = x;
                return x;
            }),

            //Function "妈", Unary
            //Makes the first autofill in this scope the current value
            ["妈"] = new Function(x => {
                ProgramState.Variables["HiddenAutofill"] = x;
                ProgramState.Autofill1Name = "HiddenAutofill";
                return x;
            }),

            //Function "用", Unary
            //Return length of number, inc. decimal point
            ["用"] = new Function(x => ((double)x).ToString().Length),

            //Function "打", Unary
            //Negate the number
            ["打"] = new Function(x => -x),

            //Function "地", Unary
            //Double the number
            ["地"] = new Function(x => x * 2),

            //Function "再", Unary
            //Halve (not rounded)
            ["再"] = new Function(x => (double)x / 2),

            //Function "因", Unary
            //Increment
            ["因"] = new Function(x => x + 1),

            //Function "呢", Unary
            //Decrement
            ["呢"] = new Function(x => x - 1),

            //Function "女", Unary
            //Convert to string
            ["女"] = new Function(x => ((double)x).ToString()),

            //Function "告", Unary
            //Convert to base-10 digit array
            ["告"] = new Function(x => x.ToString().Where(y => y != '.').Select(y => new VObject(char.GetNumericValue(y))).ToList()),

            //Function "最", Unary
            //Convert to hexadecimal string
            //Note: Rounds down
            ["最"] = new Function(x => ((int)x).ToString("X")),

            //Function "手", Unary
            //Cpmvert to binary string
            //Note: Rounds down
            ["手"] = new Function(x => ((int)x).ToString()),

            //Function "前", Unary
            //1 if 0, else 0
            ["前"] = new Function(x => x == 0 ? 1 : 0),

            //Function "找", Unary
            //Absolute value
            ["找"] = new Function(x => Math.Abs(x)),

            //Function "行", Unary
            //Factorial
            //Note: Returns the caller if caller is negative
            ["行"] = new Function(x => x < 0 ? (double)x : x == 0 ? 1d : (double)Enumerable.Range(1, x).Aggregate((c, n) => c * n)),

            //Function "快", Unary
            //Square root
            ["快"] = new Function(x => Math.Sqrt(x)),

            //Function "而", Unary
            //Square
            ["而"] = new Function(x => x * x),

            //Function "死", Unary
            //Range [0..x) (exclusive)
            ["死"] = new Function(x => Enumerable.Range(0, x).Select(n => (VObject)n).ToList()),

            //Function "先", Unary
            //Range [1..x] (inclusive)
            ["先"] = new Function(x => Enumerable.Range(1, x).Select(n => (VObject)n).ToList()),

            //Function "像", Unary
            //Binary complement
            ["像"] = new Function(x => ~x),

            //Function "等", Unary
            //Number parity
            ["等"] = new Function(x => x % 2),

            //Function "被", Unary
            //Round up
            ["被"] = new Function(x => Math.Ceiling((double)x)),

            //Function "从", Unary
            //Round down
            ["从"] = new Function(x => Math.Floor((double)x)),

            //Function "明", Unary
            //Sign
            ["明"] = new Function(x => Math.Sign((double)x)),

            //Function "中", Unary
            //To character
            ["中"] = new Function(x => ((char)x).ToString()),
        });
        private static readonly ReadOnlyDictionary<string, Function> StringFunctions = new ReadOnlyDictionary<string, Function>(new Dictionary<string, Function> {
            //Function "经", Unary
            //Stores the current expression result into the assignable variable "间"
            ["经"] = new Function(x => {
                ProgramState.Variables["间"] = x;
                return x;
            }),

            //Function "妈", Unary
            //Makes the first autofill in this scope the current value
            ["妈"] = new Function(x => {
                ProgramState.Variables["HiddenAutofill"] = x;
                ProgramState.Autofill1Name = "HiddenAutofill";
                return x;
            }),

            //Function "用", Unary
            //Length
            ["用"] = new Function(x => ((string)x).Length),

            //Function "打", Unary
            //Reverse
            ["打"] = new Function(x => string.Concat(((string)x).Reverse())),

            //Function "地", Unary
            //1st char
            ["地"] = new Function(x => ((string)x)[0]),

            //Function "再", Unary
            //Last char
            ["再"] = new Function(x => ((string)x).Last()),

            //Function "因", Unary
            //
            ["因"] = new Function(x => x),

            //Function "呢", Unary
            //
            ["呢"] = new Function(x => x),

            //Function "女", Unary
            //
            ["女"] = new Function(x => x),

            //Function "告", Unary
            //
            ["告"] = new Function(x => x),

            //Function "最", Unary
            //
            ["最"] = new Function(x => x),

            //Function "手", Unary
            //
            ["手"] = new Function(x => x),

            //Function "前", Unary
            //
            ["前"] = new Function(x => x),

            //Function "找", Unary
            //
            ["找"] = new Function(x => x),

            //Function "行", Unary
            //
            ["行"] = new Function(x => x),

            //Function "快", Unary
            //
            ["快"] = new Function(x => x),

            //Function "而", Unary
            //
            ["而"] = new Function(x => x),

            //Function "死", Unary
            //
            ["死"] = new Function(x => x),

            //Function "先", Unary
            //
            ["先"] = new Function(x => x),

            //Function "像", Unary
            //
            ["像"] = new Function(x => x),

            //Function "等", Unary
            //
            ["等"] = new Function(x => x),

            //Function "被", Unary
            //
            ["被"] = new Function(x => x),

            //Function "从", Unary
            //
            ["从"] = new Function(x => x),

            //Function "明", Unary
            //
            ["明"] = new Function(x => x),

            //Function "中", Unary
            //
            ["中"] = new Function(x => x)

        });
        private static readonly ReadOnlyDictionary<string, Function> ListFunctions = new ReadOnlyDictionary<string, Function>(new Dictionary<string, Function>
        {
            //Function "经", Unary
            //Stores the current expression result into the assignable variable "间"
            ["经"] = new Function(x => {
                ProgramState.Variables["间"] = x;
                return x;
            }),

            //Function "妈", Unary
            //Makes the first autofill in this scope the current value
            ["妈"] = new Function(x => {
                ProgramState.Variables["HiddenAutofill"] = x;
                ProgramState.Autofill1Name = "HiddenAutofill";
                return x;
            }),

            //Function "用", Unary
            //
            ["用"] = new Function(x => x),

            //Function "打", Unary
            //
            ["打"] = new Function(x => x),

            //Function "地", Unary
            //
            ["地"] = new Function(x => x),

            //Function "再", Unary
            //
            ["再"] = new Function(x => x),

            //Function "因", Unary
            //
            ["因"] = new Function(x => x),

            //Function "呢", Unary
            //
            ["呢"] = new Function(x => x),

            //Function "女", Unary
            //
            ["女"] = new Function(x => x),

            //Function "告", Unary
            //
            ["告"] = new Function(x => x),

            //Function "最", Unary
            //
            ["最"] = new Function(x => x),

            //Function "手", Unary
            //
            ["手"] = new Function(x => x),

            //Function "前", Unary
            //
            ["前"] = new Function(x => x),

            //Function "找", Unary
            //
            ["找"] = new Function(x => x),

            //Function "行", Unary
            //
            ["行"] = new Function(x => x),

            //Function "快", Unary
            //
            ["快"] = new Function(x => x),

            //Function "而", Unary
            //
            ["而"] = new Function(x => x),

            //Function "死", Unary
            //
            ["死"] = new Function(x => x),

            //Function "先", Unary
            //
            ["先"] = new Function(x => x),

            //Function "像", Unary
            //
            ["像"] = new Function(x => x),

            //Function "等", Unary
            //
            ["等"] = new Function(x => x),

            //Function "被", Unary
            //
            ["被"] = new Function(x => x),

            //Function "从", Unary
            //
            ["从"] = new Function(x => x),

            //Function "明", Unary
            //
            ["明"] = new Function(x => x),

            //Function "中", Unary
            //
            ["中"] = new Function(x => x)

        });
    }
}
