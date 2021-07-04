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

namespace TerseLang {
	public abstract class Function {

		public static int GetTier(String func) {
			if (IsUnary(func)) return 0;
			if (TIER_ZERO.Contains(func)) return 0;
			if (TIER_ONE.Contains(func)) return 1;
			if (TIER_TWO.Contains(func)) return 2;
			if (TIER_UNLIMITED.Contains(func)) return -1;
			throw new ArgumentException("func");
		}

		public static bool IsUnary(String next) {
			return UNARY_FUNCTIONS.Contains(next);
		}
		public static bool IsHigherOrder(string func, ObjectType callerType) {
			if (IsUnary(func))
				return false;
				return Functions.Count(funcs => 
					funcs.name == func &&
					funcs.function is HigherOrderFunction &&
					funcs.function.GetType().GetGenericArguments().First().TypeToObjectType() == callerType
				) == 1;
		}

		public static Function Get(String func, params ObjectType[] types) {
			//We don't need to check if func is actually a function because that is handled within GetTier()
			var tier = GetTier(func);
			if (!IsUnary(func)) {
				var tierList = new[] { TIER_UNLIMITED, TIER_ZERO, TIER_ONE, TIER_TWO }[tier + 1];
				func = TIER_UNLIMITED[tierList.IndexOf(func)].ToString();
			}
			if (types.Length > 2 || types.Length < 1) throw new ArgumentException("types");
			try {
				return Functions.Single(funcs => {

					return funcs.name == func &&
					 funcs.function.GetType().GetGenericArguments().Select(x => x.TypeToObjectType()).SequenceEqual(types);
				}).function;
			}
			catch {
				ErrorHandler.Error($"No function {func} with caller type {types[0]}" + (types.Length == 2 ? "and argument type " + types[1] + "." : ""));
			}
			//This won't be called
			throw new Exception();
		}

		

		//Unary Functions:
		//经妈用打地再因呢女告最手前找行快而死先像等被从明中
		//Binary Functions:
		//和下真现做大啊怎出点起天把开让给但谢着只些如家后儿多意别所话小自回然果发见心走定听觉太该当
		private static readonly ReadOnlyCollection<(String name, Function function)> Functions = new ReadOnlyCollection<(String name, Function function)>(new List<(String name, Function function)> {
			#region Number Unary
			//Function "经", Unary (Number) -> Number
			//Stores the current value into the assignable variable "间"
			("经", new UnaryFunctionWithProgramState<double>((x, p) => {
				p.Variables["间"] = x;
				return x;
			})),

			//Function "妈", Unary (Number) -> Number
			//Makes the first autofill in this scope the current value
			("妈", new UnaryFunctionWithProgramState<double>((x, p) => {
				p.Variables["HiddenAutofill"] = x;
				p.Autofill1Name = "HiddenAutofill";
				return x;
			})),

			//Function "用", Unary (Number) -> Number
			//Return length of number, inc. decimal point
			("用", new UnaryFunction<double>(x => x.ToString().Length)),

			//Function "打", Unary (Number) -> Number
			//Negate the number
			("打", new UnaryFunction<double>(x => -x)),

			//Function "地", Unary (Number) -> Number
			//Double the number
			("地", new UnaryFunction<double>(x => x * 2)),

			//Function "再", Unary (Number) -> Number
			//Halve (not rounded)
			("再", new UnaryFunction<double>(x => x / 2)),

			//Function "因", Unary (Number) -> Number
			//Increment
			("因", new UnaryFunction<double>(x => x + 1)),

			//Function "呢", Unary (Number) -> Number
			//Decrement
			("呢", new UnaryFunction<double>(x => x - 1)),

			//Function "女", Unary (Number) -> String
			//Convert to string
			("女", new UnaryFunction<double>(x => x.ToString())),

			//Function "告", Unary (Number) -> List
			//Convert to base-10 digit array
			("告", new UnaryFunction<double>(x => x.ToString().Where(y => y != '.').Select(y => new VObject(char.GetNumericValue(y))).ToList())),

			//Function "最", Unary (Number) -> Number
			//Convert to hexadecimal string
			("最", new UnaryFunction<double>(x => ((int)x).ToString("X"))),

			//Function "手", Unary (Number) -> Number
			//Convert to binary string
			("手", new UnaryFunction<double>(x => ((int)x).ToString())),

			//Function "前", Unary (Number) -> Number
			//1 if 0, else 0
			("前", new UnaryFunction<double>(x => x == 0 ? 1 : 0)),

			//Function "找", Unary (Number) -> Number
			//Absolute value
			("找", new UnaryFunction<double>(x => Math.Abs(x))),

			//Function "行", Unary (Number) -> Number
			//Factorial (Note: Returns the caller if caller is negative)
			("行", new UnaryFunction<double>(x => x < 0 ? x : x == 0 ? 1d : Enumerable.Range(1, (int)x).Aggregate((c, n) => c * n))),

			//Function "快", Unary (Number) -> Number
			//Square root
			("快", new UnaryFunction<double>(x => Math.Sqrt(x))),

			//Function "而", Unary (Number) -> Number
			//Square
			("而", new UnaryFunction<double>(x => x * x)),

			//Function "死", Unary  (Number) -> List
			//Range [0..x) (exclusive)
			("死", new UnaryFunction<double>(x => Enumerable.Range(0, (int)x).Select(n => (VObject)n).ToList())),

			//Function "先", Unary (Number) -> List
			//Range [1..x] (inclusive)
			("先", new UnaryFunction<double>(x => Enumerable.Range(1, (int)x).Select(n => (VObject)n).ToList())),

			//Function "像", Unary (Number) -> Number
			//Binary complement
			("像", new UnaryFunction<double>(x => ~(int)x)),

			//Function "等", Unary (Number) -> Number
			//Number parity
			("等", new UnaryFunction<double>(x => (int)x % 2)),

			//Function "被", Unary (Number) -> Number
			//Round up
			("被", new UnaryFunction<double>(x => Math.Ceiling(x))),

			//Function "从", Unary (Number) -> Number
			//Round down
			("从", new UnaryFunction<double>(x => Math.Floor(x))),

			//Function "明", Unary (Number) -> Number
			//Sign
			("明", new UnaryFunction<double>(x => Math.Sign(x))),

			//Function "中", Unary (Number) -> String
			//To character
			("中", new UnaryFunction<double>(x => ((char)x).ToString())),
			#endregion

			#region Number Binary
			//Function "和", Binary (Number, Number) -> Number
			//Addition
			("和", new BinaryFunction<double, double>((x, y) => x + y)),
			//Function "和", Binary (Number, String) -> String
			//Append s to N
			("和", new BinaryFunction<double, string>((x, y) => x + y)),
			//Function "和", Binary (Number, List) -> List
			//Prepend N to l
			("和", new BinaryFunction<double, List<VObject>>((x, y) => y.Prepend(new VObject(x)).ToList())),

			//Function "下", Binary (Number, Number) -> Number
			//Subtraction
			("下", new BinaryFunction<double, double>((x, y) => x - y)),

			//Function "真", Binary (Number, Number) -> Number
			//Multiplication
			("真", new BinaryFunction<double,double>((x,y) => x * y)),

			//Function "现", Binary (Number, Number) -> Number
			//Division
			("现", new BinaryFunction<double,double>((x,y) => x / y)),

			//Function "做", Binary (Number, Number) -> List
			//Range [x..y]
			("做", new BinaryFunction<double,double>((x,y) => x)),

			//Function "大", Binary (Number, Number) -> Number
			//Modulus
			("大", new BinaryFunction<double,double>((x,y) => x % y)),

			//Function "啊", Binary (Number, Number) -> Number
			//Round up to nearest multiple of n
			("啊", new BinaryFunction<double,double>((x,y) => x )),

			//Function "怎", Binary (Number, Number) -> Number
			//Round down to nearest multiple of n
			("怎", new BinaryFunction<double,double>((x,y) => x)),

			//Function "出", Binary (Number, Number) -> Number
			//To nth power of 10, multplied by N
			("出", new BinaryFunction<double,double>((x,y) => x * Math.Pow(10, y))),

			//Function "点", Binary (Number, Number) -> Number
			//Compare: N == n -> 0, N > n -> 1, N < n -> -1
			("点", new BinaryFunction<double,double>((x,y) => x > y ? 1 : x < y ? -1 : 0)),

			//Function "起", Binary (Number, Number) -> Number
			//Coprime N to n
			("起", new BinaryFunction<double,double>((x,y) => x)),

			//Function "天", Binary (Number, Number) -> Number
			//Smaller of caller and argument
			("天", new BinaryFunction<double,double>((x,y) => Math.Min(x, y))),

			//Function "把", Binary (Number, Number) -> Number
			//Larger of N and n
			("把", new BinaryFunction<double,double>((x,y) => Math.Max(x, y))),

			//Function "开", Binary (Number, Number) -> Number
			//n - N (subtraction but reversed)
			("开", new BinaryFunction<double,double>((x,y) => y - x)),

			//Function "让", Binary (Number, Number) -> Number
			//N to nth power
			("让", new BinaryFunction<double,double>((x,y) => Math.Pow(x, y))),

			//Function "给", Binary (Number, Number) -> Number
			//nth root of N
			("给", new BinaryFunction<double,double>((x,y) => Math.Pow(x, 1/y))),

			//Function "但", Binary (Number, Number) -> Number
			//Round to nearest multiple of n
			("但", new BinaryFunction<double,double>((x,y) => x * (int)((x + (int)(y/2)) / y))),

			//Function "谢", Binary (Number, Number) -> String
			//Base n string
			("谢", new BinaryFunction<double,double>((x,y) => {
				if(x % 1 > 0 || y % 1 > 0 || y < 2 || y > 36)
					return x;
				var chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
				string str = "";
				for(; x > 0; x/=y);
					str = chars[(int)x % (int)y] + str;
				return str;
			})),
			//Function "谢", Binary (Number, String) -> String
			//Base s string
			("谢", new BinaryFunction<double,string>((x,y) => {
				if(x % 1 > 0 || y.Length < 2)
					return x;
				string str = "";
				for(; x > 0; x/=y.Length);
					str = y[(int)x%y.Length] + str;
				return str;
			})),
			//Function "谢", Binary (Number, String) -> String
			//Base s string
			("谢", new BinaryFunction<double,List<VObject>>((x,y) => {
				if(x % 1 > 0 || y.Count < 2)
					return x;
				string str = "";
				for(; x > 0; x/=y.Count);
					str = y[(int)x%y.Count] + str;
				return str;
			})),

			//Function "着", Binary (Number, Number) -> List
			//DivRem
			("着", new BinaryFunction<double,double>((x,y) => new List<VObject> { (int)(x / y), x % y })),

			//Function "只", Binary (Number, Lambda[Number]) -> Number
			//First integer greater than or equal to x that fulfills the lambda
			("只", new HigherOrderFunction<double>((x,y) => { 
				for(int i = (int)Math.Ceiling(x); ; i++) {
					if (y(i).IsTruthy())
						return i;
				}
			}, 1)),

			//Function "些", Binary (Number, String) -> String
			//Get Nth character of s
			("些", new BinaryFunction<double,string>((x,y) => y[x < 0 ? y.Length - (int)x : (int)x])),
			//Function "些", Binary (Number, List) -> T
			//Get Nth character of l
			("些", new BinaryFunction<double,List<VObject>>((x,y) => y[x < 0 ? y.Count - (int)x : (int)x])),

			//Function "如", Binary (Number, Lambda[Number]) -> List
			//Create range [0..N), then map it with Lambda
			("如", new HigherOrderFunction<double>((x,y) => Enumerable.Range(0, (int)x).Select(n => y(n)).ToList(), 1)),

			//Function "家", Binary (Number, Number) -> Number
			//Greater than
			("家", new BinaryFunction<double,double>((x,y) => x > y)),

			//Function "后", Binary (Number, Number) -> Number
			//Less than
			("后", new BinaryFunction<double,double>((x,y) => x < y)),

			//Function "儿", Binary (Number, Number) -> Number
			//Greater than or equal to
			("儿", new BinaryFunction<double,double>((x,y) => x >= y)),

			//Function "多", Binary (Number, Number) -> Number
			//Less than or equal to
			("多", new BinaryFunction<double,double>((x,y) => x <= y)),

			//Function "意", Binary (Number, Number) -> Number
			//Return whether N is divisible by n 
			("意", new BinaryFunction<double,double>((x,y) => x % y == 0 ? 1 : 0)),

			//Function "别", Binary (Number, Number) -> Number
			//
			("别", new BinaryFunction<double,double>((x,y) => x)),

			//Function "所", Binary (Number, Number) -> Number
			//
			("所", new BinaryFunction<double,double>((x,y) => x)),

			//Function "话", Binary (Number, Number) -> Number
			//
			("话", new BinaryFunction<double,double>((x,y) => x)),

			//Function "小", Binary (Number, Number) -> Number
			//
			("小", new BinaryFunction<double,double>((x,y) => x)),

			//Function "自", Binary (Number, Number) -> Number
			//
			("自", new BinaryFunction<double,double>((x,y) => x)),

			//Function "回", Binary (Number, Number) -> Number
			//
			("回", new BinaryFunction<double,double>((x,y) => x)),

			//Function "然", Binary (Number, Number) -> Number
			//
			("然", new BinaryFunction<double,double>((x,y) => x)),

			//Function "果", Binary (Number, Number) -> Number
			//
			("果", new BinaryFunction<double,double>((x,y) => x)),

			//Function "发", Binary (Number, Number) -> Number
			//
			("发", new BinaryFunction<double,double>((x,y) => x)),

			//Function "见", Binary (Number, Number) -> Number
			//
			("见", new BinaryFunction<double,double>((x,y) => x)),

			//Function "心", Binary (Number, Number) -> Number
			//
			("心", new BinaryFunction<double,double>((x,y) => x)),

			//Function "走", Binary (Number, Number) -> Number
			//
			("走", new BinaryFunction<double,double>((x,y) => x)),

			//Function "定", Binary (Number, Number) -> Number
			//
			("定", new BinaryFunction<double,double>((x,y) => x)),

			//Function "听", Binary (Number, Number) -> Number
			//
			("听", new BinaryFunction<double,double>((x,y) => x)),

			//Function "觉", Binary (Number, Number) -> Number
			//
			("觉", new BinaryFunction<double,double>((x,y) => x)),

			//Function "太", Binary (Number, Number) -> Number
			//
			("太", new BinaryFunction<double,double>((x,y) => x)),

			//Function "该", Binary (Number, Number) -> List
			//Pair
			("该", new BinaryFunction<double,double>((x,y) => new List<VObject>{ x, y })),			
			//Function "该", Binary (Number, String) -> List
			//Pair
			("该", new BinaryFunction<double,string>((x,y) => new List<VObject>{ x, y })),
			//Function "该", Binary (Number, List) -> List
			//Pair
			("该", new BinaryFunction<double,List<VObject>>((x,y) => new List<VObject>{ x, y })),

			//Function "当", Binary (Number, Number) -> Number
			//Equals
			("当", new BinaryFunction<double,double>((x,y) => x == y)),
			//Function "当", Binary (Number, String) -> Number
			//Equals
			("当", new BinaryFunction<double,string>((x,y) => x.ToString() == y)),
			//Function "当", Binary (Number, List) -> Number
			//Equals
			("当", new BinaryFunction<double,List<VObject>>((x,y) => false)),
			#endregion

			#region String Unary
			//Function "经", Unary (String) -> String
			//Stores the current expression result into the assignable variable "间"
			("经", new UnaryFunctionWithProgramState<string>((x, p) => {
				p.Variables["间"] = x;
				return x;
			})),

			//Function "妈", Unary (String) -> String
			//Makes the first autofill in this scope the current value
			("妈", new UnaryFunctionWithProgramState<string>((x, p) => {
				p.Variables["HiddenAutofill"] = x;
				p.Autofill1Name = "HiddenAutofill";
				return x;
			})),

			//Function "用", Unary (String) -> Number
			//Length
			("用", new UnaryFunction<string>(x => x.Length)),

			//Function "打", Unary (String) -> String
			//Reverse
			("打", new UnaryFunction<string>(x => string.Concat(x.Reverse()))),

			//Function "地", Unary (String) -> String
			//1st char
			("地", new UnaryFunction<string>(x => x[0])),

			//Function "再", Unary (String) -> String
			//Last char
			("再", new UnaryFunction<string>(x => x.Last())),

			//Function "因", Unary (String) -> Number/String
			//To number (returns caller string if not parsable)
			("因", new UnaryFunction<string>(x => double.TryParse(x, out var r) ? (VObject)r : (VObject)x)),

			//Function "呢", Unary (String) -> List[String]
			//To char array
			("呢", new UnaryFunction<string>(x => x.Select(n => (VObject)n).ToList())),

			//Function "女", Unary (String) -> List[String]
			//Split on " "
			("女", new UnaryFunction<string>(x => x.Split(" ").Select(n => (VObject)n).ToList())),

			//Function "告", Unary (String) -> String
			//Split on newline "\n"
			("告", new UnaryFunction<string>(x => x.Split("\n").Select(n => (VObject)n).ToList())),

			//Function "最", Unary (String) -> List[Number]/Number
			//Array of char values, or just an int if length 1
			("最", new UnaryFunction<string>(x => x.Length == 1 ? new VObject((int)x[0]) : (VObject)x.Select(n => new VObject((int)n)).ToList())),

			//Function "手", Unary (String) -> List[String]
			//All permutations of the string
			("手", new UnaryFunction<string>(x => {
				List<VObject> permList = new List<VObject>();
				char[] chars = x.ToCharArray();
				int len = chars.Length;
				void Permutations(int i) {
					if (i >= len - 1)
						permList.Add(String.Concat(chars));
					else {
						Permutations(i + 1);
						for (int j = i + 1; j < len; j++) {
							(chars[i], chars[j]) = (chars[j], chars[i]);
							Permutations(i + 1);
							(chars[i], chars[j]) = (chars[j], chars[i]);
						}
					}
				}
				Permutations(0);
				return permList;
			})),

			//Function "前", Unary (String) -> String
			//To Upper
			("前", new UnaryFunction<string>(x => x.ToUpper())),

			//Function "找", Unary (String) -> String
			//To Lower
			("找", new UnaryFunction<string>(x => x.ToLower())),

			//Function "行", Unary (String) -> String
			//Repeat itself, twice
			("行", new UnaryFunction<string>(x => x + x)),

			//Function "快", Unary (String) -> String
			//Split on newlines, then transpose rows and columns, then rejoin with newlines
			("快", new UnaryFunction<string>(x => {
				var split = x.Split("\n");
				var maxLen = split.Max(n => n.Length);
				var lines = new List<VObject>();
				for(int i = 0; i < maxLen; i++) {
					string line = "";
					foreach(var l in split) {
						if(i >= l.Length)
							line += " ";
						else
							line += l[i];
					}
					lines.Add(line);
				}
				return string.Join("\n", lines);
			})),

			//Function "而", Unary (String) -> String
			//Unique chars in order they appear
			("而", new UnaryFunction<string>(x => string.Concat(x.GroupBy(n => n).Select(n => n.Key)))),

			//Function "死", Unary (String) -> List[String]
			//All subsections
			("死", new UnaryFunction<string>(x => {
				List<VObject> subsecs = new List<VObject>();
				for (int i = 0; i < x.Length; i++) {
					for (int j = x.Length - i; j > 0; j--) {
						subsecs.Add(x.Substring(i, j));
					}
				}
				return subsecs;
			})),

			//Function "先", Unary (String) -> String
			//
			("先", new UnaryFunction<string>(x => x)),

			//Function "像", Unary (String) -> String
			//
			("像", new UnaryFunction<string>(x => x)),

			//Function "等", Unary (String) -> String
			//
			("等", new UnaryFunction<string>(x => x)),

			//Function "被", Unary (String) -> String
			//
			("被", new UnaryFunction<string>(x => x)),

			//Function "从", Unary (String) -> String
			//
			("从", new UnaryFunction<string>(x => x)),

			//Function "明", Unary (String) -> String
			//
			("明", new UnaryFunction<string>(x => x)),

			//Function "中", Unary (String) -> String
			//
			("中", new UnaryFunction<string>(x => x)),
			#endregion

			#region String Binary
			//Function "和", Binary (String, String) -> String
			//Concatenation
			("和", new BinaryFunction<string,string>((x,y) => x + y)),
			//Function "和", Binary (String, Number) -> String
			//Concatenation
			("和", new BinaryFunction<string,double>((x,y) => x + y)),
			//Function "和", Binary (String, Number) -> String
			//Prepend S to l
			("和", new BinaryFunction<string,List<VObject>>((x,y) => y.Prepend((VObject)x).ToList())),

			//Function "下", Binary (String, String) -> List
			//Split S on s
			("下", new BinaryFunction<string,string>((x,y) => x.Split(y).Select(l => new VObject(l)).ToList())),
			//Function "下", Binary (String, Number) -> List
			//Split S on n
			("下", new BinaryFunction<string,double>((x,y) => x.Split(y.ToString()).Select(l => new VObject(l)).ToList())),

			//Function "真", Binary (String, String) -> String
			//Prepend S with s
			("真", new BinaryFunction<string,string>((x,y) => y + x)),

			//Function "现", Binary (String, Number) -> List
			//Split S into chunks of size n
			("现", new BinaryFunction<string,double>((x,y) => {
				if(y % 1 > 0)
					y = Math.Floor(y);
				int i = 0;
				return x.GroupBy(_ => i++ / y).Select(n => (VObject)string.Concat(n)).ToList();
			})),

			//Function "做", Binary (String, Number) -> List
			//Split S into n chunks containing consecutive elements
			("做", new BinaryFunction<string,double>((x,y) => {
				int n = (int)y;
				int i = 0;
				int chunkLen = x.Length / n;
				return x.GroupBy(_ => i++ / chunkLen).Select(l => (VObject)string.Concat(l)).ToList();
			})),

			//Function "大", Binary (String, String) -> Number
			//First index of s within S
			("大", new BinaryFunction<string,string>((x,y) => x.IndexOf(y))),

			//Function "啊", Binary (String, String) -> Number
			//Last index of s within S
			("啊", new BinaryFunction<string,string>((x,y) => x.LastIndexOf(y))),

			//Function "怎", Binary (String, String) -> List
			//All indexes of s within S
			("怎", new BinaryFunction<string,string>((x,y) => Enumerable.Range(0, x.Length).Where(n => x.IndexOf(y, n) != -1).Select(n => new VObject(n)).ToList())),
			//Function "怎", Binary (String, String) -> List
			//All indexes of s within S
			("怎", new BinaryFunction<string,double>((x,y) => Enumerable.Range(0, x.Length).Where(n => x.IndexOf(y.ToString(), n) != -1).Select(n => new VObject(n)).ToList())),

			//Function "出", Binary (String, String) -> String
			//Delete all instances of s from S
			("出", new BinaryFunction<string,string>((x,y) => x.Replace(y, ""))),
			//Function "出", Binary (String, List) -> String
			//For each pair of strings (s1, s2) in l, replace each instance of s1 in S with s2. If there are odd number of elements in l, for the last element it is replaced with empty string
			("出", new BinaryFunction<string,List<VObject>>((x,y) => {
				int i = 0;
				foreach(var pair in y.GroupBy(_ => i++/2)) {
					var (search, replacement) = (pair.ElementAt(0), pair.Count() < 2 ? (VObject)"" : pair.ElementAt(1));
					x = x.Replace(search, replacement);
				}
				return x;
			})),

			//Function "点", Binary (String, Lambda[T,Number]) -> List
			//Map
			("点", new HigherOrderFunction<string>((x,y) => x.Select((n,i) => y(n,i)).ToList(), 2)),

			//Function "起", Binary (String, Lambda[T,Number]) -> String
			//Filter
			("起", new HigherOrderFunction<string>((x,y) => string.Concat(x.Where((n, i) => y(n, i).IsTruthy())), 2)),

			//Function "天", Binary (String, Lambda[T,Number]) -> List[List[String]]
			//Group By
			("天", new HigherOrderFunction<string>((x,y) => {
				int i = 0;
				return x.GroupBy(n => y(n,i++)).Select(l=>(VObject)l.Select(o=>(VObject)o.ToString()).ToList()).ToList();
			}, 2)),

			//Function "把", Binary (String, Lambda[String,Number]) -> String
			//Filter out truthy
			("把", new HigherOrderFunction<string>((x,y) => string.Concat(x.Where((n, i) => !y(n,i).IsTruthy())), 2)),

			//Function "开", Binary (String, Lambda[String,Number]) -> List
			//Reduce
			("开", new  HigherOrderFunction<string>((x,y) => {
				int i = 0;
				return x.Aggregate<char,VObject>(x[0].ToString(), (cur, n) => y(cur, n, i++));
			}, 2)),

			//Function "让", Binary (String, Lambda[T,Number]) -> List
			//Culmulative Reduction
			("让", new HigherOrderFunction<string>((x,y) => {
				int i = 0;
				List<VObject> res = new List<VObject>();
				x.ToList().ForEach(n => res.Add(y(n,i++)));
				return res;
			}, 2)),

			//Function "给", Binary (String, String) -> Number
			//Sum with lambda
			("给", new HigherOrderFunction<string>((x,y) => {
				int i = 0;
				return x.Sum(n => {
					var ret = y(n, i++);
					if(ret.ObjectType == ObjectType.String && double.TryParse(ret, out double d))
						return d;
					if(ret.ObjectType == ObjectType.List)
						return 0;
					return ret;
				});
			}, 2)),

			//Function "但", Binary (String, Lambda[T,Number]) -> List
			//Cumulative Sum with lambda
			("但", new HigherOrderFunction<string>((x,y) => {
				int i = 0;
				double sum = 0;
				return x.Select(n => {
					var ret = y(n, i++);
					if(ret.ObjectType == ObjectType.String && double.TryParse(ret, out double d))
						return sum += d;
					if(ret.ObjectType == ObjectType.List)
						return sum;
					return sum += ret;
				}).Select(l => new VObject(l)).ToList();
			}, 2)),


			//Function "谢", Binary (String, Number) -> List
			//Take first n elements of S
			("谢", new BinaryFunction<string,double>((x,y) => x.Substring(0, (int)y))),

			//Function "着", Binary (String, Number) -> List
			//Take last n elements of S
			("着", new BinaryFunction<string,double>((x,y) => x.Substring(x.Length - (int)y))),

			//Function "只", Binary (String, Lambda[String,Number]) -> String
			//Take while lambda returns true
			("只", new HigherOrderFunction<string>((x,y) => {
				int i = 0;
				return string.Concat(x.TakeWhile(n => y(n, i++).IsTruthy()));
			}, 2)),

			//Function "些", Binary (String, Number) -> String
			//Skip first n elements of S
			("些", new BinaryFunction<string,double>((x,y) => x.Substring((int)y))),

			//Function "如", Binary (String, Number) -> String
			//Skip last n elements of S
			("如", new BinaryFunction<string,double>((x,y) => x.Substring(0, x.Length - (int)y))),

			//Function "家", Binary (String, Lambda[T,Number]) -> String
			//Skip while lambda returns true
			("家", new HigherOrderFunction<string>((x,y) => {
				int i = 0;
				return string.Concat(x.SkipWhile(n => y(n, i++).IsTruthy()));
			}, 2)),

			//Function "后", Binary (List, Lambda[T,Number]) -> List
			//Sort with lambda
			("后", new HigherOrderFunction<string>((x,y) => {
				int i = 0;
				return string.Concat(x.OrderBy(n => y(n, i++)));
			}, 2)),

			//Function "儿", Binary (List, Lambda[T,Number]) -> List
			//Sort descending with lambda
			("儿", new HigherOrderFunction<string>((x,y) => {
				int i = 0;
				return string.Concat(x.OrderByDescending(n => y(n, i++)));
			}, 2)),

			//Function "多", Binary (String, Number) -> String
			//Get nth char (negative gets from back)
			("多", new BinaryFunction<string, double>((x,y) => x[y < 0 ? x.Length - (int)y : (int)y])),
			//Function "多", Binary (String, List) -> List
			//Foreach value in l, get nth element (negative gets from back)
			("多", new BinaryFunction<string, List<VObject>>((x,y) => y.Where(n => n.ObjectType == ObjectType.Number || (n.ObjectType == ObjectType.String && int.TryParse(n, out _)))
			.Select(n => {
				if(n.ObjectType == ObjectType.String)
					if(int.TryParse(n, out int i))
						return x[i < 0 ? x.Length - i : i];
					//Saves a bit of code (acceptable in this case because the error label is very close
					else goto error;
				else if(n.ObjectType == ObjectType.Number)
					return x[n < 0 ? x.Length - (int)n : (int)n];
				else goto error;
				error: {
					ErrorHandler.InternalError("In function 多 (List, List) -> List, an object in l got past the filter:" + n.ToString());
					throw new Exception();
				}
			}).Select(n => (VObject)n).ToList())),

			//Function "意", Binary (List, Number) -> Number
			//Contains the certain VObject (in this case number)
			("意", new BinaryFunction<string,double>((x,y) => x.Contains(y.ToString()))),
			//Function "意", Binary (String, String) -> Number
			//Contains the certain VObject (in this case string)
			("意", new BinaryFunction<string,string>((x,y) => x.Contains(y))),

			//Function "别", Binary (String, String) -> String
			//Remove all characters in s from S
			("别", new BinaryFunction<string,string>((x,y) => string.Concat(x.Where(n => !y.Contains(n))))),
			//Function "别", Binary (String, Number) -> String
			//Remove all characters in n from S
			("别", new BinaryFunction<string,double>((x,y) => string.Concat(x.Where(n => !(y.ToString()).Contains(n))))),

			//Function "所", Binary (String, Number) -> String/Number
			//Convert S from base-n string to base-10 integer, else returns itself if invalid (supports base 2 - 36)
			("所", new BinaryFunction<string,double>((x,y) => {
				if(y % 1 != 0 || y < 2 || y > 36)
					return x;
				var chars = "0123456789ABCDEFGHIFJKLMNOPQRSTUVWXYZ".Substring(0, (int)y);
				int res = 0;
				for(int i = x.Length - 1; i >= 0; i--) {
					if(!chars.Contains(x[i]))
						return x;
					res += chars.IndexOf(x[i]);
					res *= (int)y;
				}
				return res;
			})),
			//Function "所", Binary (String, String) -> String/Number
			//Convert S from base-s string to base-10 integer, else returns itself if invalid
			("所", new BinaryFunction<string,string>((x,y) => {
				int res = 0;
				for(int i = x.Length - 1; i >= 0; i--) {
				   if(!y.Contains(x[i]))
						return x;
					res += y.IndexOf(x[i]);
					res *= y.Length;
				}
				return res;
			})),
			
			//Function "话", Binary (String, String) -> String
			//Repeat S enough times so that it is as long as y
			("话", new BinaryFunction<string,string>((x,y) => x == "" ? "" : string.Concat(y.Select((_, i) => y[i % x.Length])))),
			//Function "话", Binary (String, Number) -> String
			//Repeat S n times
			("话", new BinaryFunction<string,double>((x,y) => string.Concat(Enumerable.Repeat(x,Math.Max(0, (int)y))))),

			//Function "小", Binary (String, Number) -> String
			//Get every nth character of S
			("小", new BinaryFunction<string,double>((x,y) => string.Concat(x.Where((_, i) => i % y == 0)))),

			//Function "自", Binary (String, String) -> String
			//Number of occurences of s in S
			("自", new BinaryFunction<string,string>((x,y) => {
				int count = 0, n = 0;

				if(y == "") {
					return 0;
				}
				while ((n = x.IndexOf(y, n, StringComparison.InvariantCulture)) != -1) {
					n += y.Length;
					count++;
				}
				return count;
			})),
			//Function "自", Binary (String, Number) -> String
			//Number of occurences of n in S
			("自", new BinaryFunction<string,double>((x,y) => {
				int count = 0, n = 0;
				string substring = y.ToString();
				if(substring == "") {
					return 0;
				}
				while ((n = x.IndexOf(substring, n, StringComparison.InvariantCulture)) != -1) {
					n += substring.Length;
					count++;
				}
				return count;
			})),

			//Function "回", Binary (String, Lambda[String, String, Number]) -> List
			//Pass each consecutive pair of characters thru L
			("回", new HigherOrderFunction<string>((x,y) => {
				int index = 0;
				return x.Zip(x.Skip(1), (a,b) => y(a,b,index++)).ToList(); 
			}, 3, x => new List<VObject>{ x[0], x[1] })),

			//Function "然", Binary (String, String) -> String
			//
			("然", new BinaryFunction<string,string>((x,y) => x)),

			//Function "果", Binary (String, String) -> String
			//
			("果", new BinaryFunction<string,string>((x,y) => x)),

			//Function "发", Binary (String, String) -> String
			//
			("发", new BinaryFunction<string,string>((x,y) => x)),

			//Function "见", Binary (String, String) -> String
			//
			("见", new BinaryFunction<string,string>((x,y) => x)),

			//Function "心", Binary (String, String) -> String
			//
			("心", new BinaryFunction<string,string>((x,y) => x)),

			//Function "走", Binary (String, String) -> String
			//
			("走", new BinaryFunction<string,string>((x,y) => x)),

			//Function "定", Binary (String, String) -> String
			//
			("定", new BinaryFunction<string,string>((x,y) => x)),

			//Function "听", Binary (String, String) -> String
			//
			("听", new BinaryFunction<string,string>((x,y) => x)),

			//Function "觉", Binary (String, String) -> String
			//
			("觉", new BinaryFunction<string,string>((x,y) => x)),

			//Function "太", Binary (String, String) -> String
			//
			("太", new BinaryFunction<string,string>((x,y) => x)),

			//Function "该", Binary (String, String) -> List
			//Pair
			("该", new BinaryFunction<string,string>((x,y) => new List<VObject>{ x, y })),			
			//Function "该", Binary (String, Number) -> List
			//Pair
			("该", new BinaryFunction<string,double>((x,y) => new List<VObject>{ x, y })),
			//Function "该", Binary (String, List) -> List
			//Pair
			("该", new BinaryFunction<string,List<VObject>>((x,y) => new List<VObject>{ x, y })),

			//Function "当", Binary (List, List) -> Number
			//Equals
			("当", new BinaryFunction<string,string>((x,y) => x == y)),
			//Function "当", Binary (List, String) -> Number
			//Equals
			("当", new BinaryFunction<string,List<VObject>>((x,y) => false)),
			//Function "当", Binary (List, Number) -> Number
			//Equals
			("当", new BinaryFunction<string,double>((x,y) => x == y.ToString())),


			#endregion

			#region List Unary
			//Function "经", Unary (List) -> List
			//Stores the current expression result into the assignable variable "间"
			("经", new UnaryFunctionWithProgramState<List<VObject>>((x,p) => {
				p.Variables["间"] = x;
				return x;
			})),

			//Function "妈", Unary (List) -> List
			//Makes the first autofill in this scope the current value
			("妈", new UnaryFunctionWithProgramState<List<VObject>>((x,p) => {
				p.Variables["HiddenAutofill"] = x;
				p.Autofill1Name = "HiddenAutofill";
				return x;
			})),

			//Function "用", Unary (List) -> Number
			//Length
			("用", new UnaryFunction<List<VObject>>(x => x.Count)),

			//Function "打", Unary (List) -> List
			//Reverse the list
			("打", new UnaryFunction<List<VObject>>(x => { x.Reverse(); return x; })),

			//Function "地", Unary (List) -> T
			//1st element
			("地", new UnaryFunction<List<VObject>>(x => x[0])),

			//Function "再", Unary (List) -> T
			//Last element
			("再", new UnaryFunction<List<VObject>>(x => x.Last())),

			//Function "因", Unary (List) -> List
			//Sort
			("因", new UnaryFunction<List<VObject>>(x => x.OrderBy(n => n).ToList())),

			//Function "呢", Unary (List) -> List
			//Concatenate
			("呢", new UnaryFunction<List<VObject>>(x => string.Concat(x))),

			//Function "女", Unary (List) -> String
			//Join w/ " "
			("女", new UnaryFunction<List<VObject>>(x => string.Join(" ", x))),

			//Function "告", Unary (List) -> String
			//Join w/ "\n"
			("告", new UnaryFunction<List<VObject>>(x => string.Join("\n", x))),

			//Function "最", Unary (List) -> Humber
			//Sum (Map strings by parsing them or 0, lists to 0)
			("最", new UnaryFunction<List<VObject>>(x => x.Sum(n => {
				switch (n.ObjectType) {
					case ObjectType.Number:
						return n;
					case ObjectType.String:
						return double.TryParse(n, out double d) ? d : 0;
					case ObjectType.List:
						return 0;
				}
				throw new Exception("Object is not of type number, list or string in function 最");

			}))),

			//Function "手", Unary (List) -> Number
			//Product (Map strings by parsing them or 1, lists to 1)
			("手", new UnaryFunction<List<VObject>>(x => x.Aggregate((product, n) => {
				switch (n.ObjectType) {
					case ObjectType.Number:
						return n * product;
					case ObjectType.String:
						return double.TryParse(n, out double d) ? product * d : (double)product;
					case ObjectType.List:
						return product;
				}
				throw new Exception("Object is not of type number, list or string in function 手");
			}))),

			//Function "前", Unary (List) -> List
			//Flatten Completely
			("前", new UnaryFunction<List<VObject>>(x => {
				List<VObject> flattened = new List<VObject>();
				void Search(List<VObject> l) {
					foreach (var obj in l) {
						if (obj.ObjectType == ObjectType.List)
							Search(obj);
						else
							flattened.Add(obj);
					}
				}
				return flattened;
			})),

			//Function "找", Unary (List) -> List
			//Remove first
			("找", new UnaryFunction<List<VObject>>(x => x.Skip(1).ToList())),

			//Function "行", Unary (List) -> List
			//Max (Strings to parsed or NegativeInfinity, lists to NegativeInfinity)
			("行", new UnaryFunction<List<VObject>>(x => x.Max(n => {
				switch (n.ObjectType) {
					case ObjectType.Number:
						return n;
					case ObjectType.String:
						return double.TryParse(n, out double d) ? d : double.NegativeInfinity;
					case ObjectType.List:
						return double.NegativeInfinity;
				}
				throw new Exception("Object is not of type number, list or string in function 行");
			}))),

			//Function "快", Unary (List) -> Number
			//Min (Strings to parsed or PositiveInfinity, lists to PositiveInfinity)
			("快", new UnaryFunction<List<VObject>>(x => x.Min(n => {
				switch (n.ObjectType) {
					case ObjectType.Number:
						return n;
					case ObjectType.String:
						return double.TryParse(n, out double d) ? d : double.PositiveInfinity;
					case ObjectType.List:
						return double.PositiveInfinity;
				}
				throw new Exception("Object is not of type number, list or string in function 快");
			}))),

			//Function "而", Unary (List) -> Number
			//Cumulative Sum (Strings to parsed or 0, lists to 0)
			("而", new UnaryFunction<List<VObject>>(x => {
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
			})),

			//Function "死", Unary (List) -> List
			//Subsections (inc. self)
			("死", new UnaryFunction<List<VObject>>(x => {
				List<VObject> subsecs = new List<VObject>();
				for (int i = 0; i < x.Count; i++) {
					for (int j = x.Count - i; j > 0; j--) {
						subsecs.Add(x.Skip(i).Take(j).ToList());
					}
				}
				return subsecs;
			})),

			//Function "先", Unary (List) -> List
			//Permutations
			("先", new UnaryFunction<List<VObject>>(x => {
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
			})),

			//Function "像", Unary (List) -> List
			//Combinations
			("像", new UnaryFunction<List<VObject>>(x => {
				List<VObject> combos = new List<VObject>();
				/*
				 * Based off of https://stackoverflow.com/a/127856
				*/
				int len = x.Count;
				for (int i = 1; i <= len; i++) {
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
			})),

			//Function "等", Unary (List) -> List
			//Group
			("等", new UnaryFunction<List<VObject>>(x => x.GroupBy(n => n).Select(n => (VObject)n.ToList()).ToList())),

			//Function "被", Unary (List) -> List
			//
			("被", new UnaryFunction<List<VObject>>(x => x)),

			//Function "从", Unary (List) -> List
			//
			("从", new UnaryFunction<List<VObject>>(x => x)),

			//Function "明", Unary (List) -> List
			//
			("明", new UnaryFunction<List<VObject>>(x => x)),

			//Function "中", Unary (List) -> List
			//
			("中", new UnaryFunction<List<VObject>>(x => x)),
			
			#endregion            

			#region List Binary
			//Function "和", Binary (List, List) -> List
			//Concatenation
			("和", new BinaryFunction<List<VObject>,List<VObject>>((x,y) => x.Concat(y).ToList())),
			//Function "和", Binary (List, String) -> List
			//Append s to L
			("和", new BinaryFunction<List<VObject>,string>((x,y) => x.Append((VObject)y).ToList())),
			//Function "和", Binary (List, Number) -> List
			//Append n to L
			("和", new BinaryFunction<List<VObject>,double>((x,y) => x.Append((VObject)y).ToList())),

			//Function "下", Binary (List, String) -> String
			//Join L on s
			("下", new BinaryFunction<List<VObject>,string>((x,y) => string.Join(y, x))),
			//Function "下", Binary (List, Number) -> List
			//Join L on n
			("下", new BinaryFunction<List<VObject>,double>((x,y) => string.Join(y.ToString(), x))),

			//Function "真", Binary (List, List) -> List
			//Prepend l to L
			("真", new BinaryFunction<List<VObject>,List<VObject>>((x,y) =>y.Concat(x).ToList())),

			//Function "现", Binary (List, Number) -> List
			//Split S into chunks of size n
			("现", new BinaryFunction<List<VObject>,double>((x,y) => {
				int i = 0;
				return x.GroupBy(_ => i++/y).Select(l => (VObject)l.ToList()).ToList();
			})),

			//Function "做", Binary (List, List) -> List
			//Split S into n chunks
			("做", new BinaryFunction<List<VObject>,List<VObject>>((x,y) => x)),

			//Function "大", Binary (List, List) -> List
			//First index of s within S
			("大", new BinaryFunction<List<VObject>,List<VObject>>((x,y) => x.IndexOf(y))),

			//Function "啊", Binary (List, List) -> List
			//Last index of s within S
			("啊", new BinaryFunction<List<VObject>,List<VObject>>((x,y) => x.LastIndexOf(y))),

			//Function "怎", Binary (List, List) -> List
			//All indexes of s within S
			("怎", new BinaryFunction<List<VObject>,List<VObject>>((x,y) => Enumerable.Range(0, x.Count).Where(n => x[n].Equals(y)).ToVList())),
			//Function "怎", Binary (List, String) -> List
			//All indexes of s within S
			("怎", new BinaryFunction<List<VObject>,string>((x,y) => Enumerable.Range(0, x.Count).Where(n => x[n].Equals(y)).ToVList())),
			//Function "怎", Binary (List, List) -> List
			//All indexes of s within S
			("怎", new BinaryFunction<List<VObject>,double>((x,y) => Enumerable.Range(0, x.Count).Where(n => x[n].Equals(y)).ToVList())),

			//Function "出", Binary (List, String) -> List
			//Delete all instances of s from L
			("出", new BinaryFunction<List<VObject>,string>((x,y) => x.Where(n => n!=y).ToList())),
			//Function "出", Binary (List, Number) -> List
			//Delete all instances of s from L
			("出", new BinaryFunction<List<VObject>,double>((x,y) => x.Where(n => n!=y).ToList())),

			//Function "点", Binary (List, Lambda[T,Number]) -> List
			//Map
			("点", new HigherOrderFunction<List<VObject>>((x,y) => x.Select((n,i) => y(n,i)).ToList(), 2)),

			//Function "起", Binary (List, Lambda[T,Number]) -> List
			//Filter
			("起", new HigherOrderFunction<List<VObject>>((x,y) => x.Where((n, i) => y(n, i).IsTruthy()).ToList(), 2)),

			//Function "天", Binary (List, Lambda[T,Number]) -> List
			//Group By
			("天", new HigherOrderFunction<List<VObject>>((x,y) => {
				int i = 0;
				return x.GroupBy(n => y(n,i++)).Select(l=>(VObject)l.ToList()).ToList();
			}, 2)),

			//Function "把", Binary (List, Lambda[T,Number]) -> List
			//Filter out truthy
			("把", new HigherOrderFunction<List<VObject>>((x,y) => x.Where((n, i) => !y(n,i).IsTruthy()).ToList(), 2)),

			//Function "开", Binary (List, Lambda[T,Number]) -> List
			//Reduce
			("开", new  HigherOrderFunction<List<VObject>>((x,y) => {
				int i = 0;
				return x.Aggregate((cur, n) => y(cur, n, i++));
			}, 2)),

			//Function "让", Binary (List, Lambda[T,Number]) -> List
			//Culmulative Reduction
			("让", new HigherOrderFunction<List<VObject>>((x,y) => {
				int i = 0;
				List<VObject> res = new List<VObject>();
				x.ForEach(n => res.Add(y(n,i++)));
				return res;
			}, 2)),

			//Function "给", Binary (List, Lambda[T,Number]) -> Number
			//Sum with lambda
			("给", new HigherOrderFunction<List<VObject>>((x,y) => {
				int i = 0;
				return x.Sum(n => {
					var ret = y(n, i++);
					if(ret.ObjectType == ObjectType.String && double.TryParse(ret, out double d))
						return d;
					if(ret.ObjectType == ObjectType.List)
						return 0;
					return ret;
				});
			}, 2)),

			//Function "但", Binary (List, Lambda[T,Number]) -> List
			//Cumulative Sum with lambda
			("但", new HigherOrderFunction<List<VObject>>((x,y) => {
				int i = 0;
				double sum = 0;
				return x.Select(n => {
					var ret = y(n, i++);
					if(ret.ObjectType == ObjectType.String && double.TryParse(ret, out double d))
						return sum += d;
					if(ret.ObjectType == ObjectType.List)
						return sum;
					return sum += ret;
				}).Select(l => new VObject(l)).ToList();
			}, 2)),

			//Function "谢", Binary (List, Number) -> List
			//Take first n elements of L
			("谢", new BinaryFunction<List<VObject>,double>((x,y) => x.Take((int)y).ToList())),

			//Function "着", Binary (List, Number) -> List
			//Take last n elements of L
			("着", new BinaryFunction<List<VObject>,double>((x,y) => x.TakeLast((int)y).ToList())),

			//Function "只", Binary (List, Lambda[T,Number]) -> List
			//Take while lambda returns true
			("只", new HigherOrderFunction<List<VObject>>((x,y) => {
				int i = 0;
				return x.TakeWhile(n => y(n, i++).IsTruthy()).ToList();
			}, 2)),

			//Function "些", Binary (List, Number) -> List
			//Skip first n elements of L
			("些", new BinaryFunction<List<VObject>,double>((x,y) => x.Skip((int)y).ToList())),

			//Function "如", Binary (List, Number) -> List
			//Skip last n elements of L
			("如", new BinaryFunction<List<VObject>,double>((x,y) => x.SkipLast((int)y).ToList())),

			//Function "家", Binary (List, Lambda[T,Number]) -> List
			//Skip while lambda returns true
			("家", new HigherOrderFunction<List<VObject>>((x,y) => {
				int i = 0;
				return x.SkipWhile(n => y(n, i++).IsTruthy()).ToList();
			}, 2)),

			//Function "后", Binary (List, Lambda[T,Number]) -> List
			//Sort with lambda
			("后", new HigherOrderFunction<List<VObject>>((x,y) => {
				int i = 0;
				return x.OrderBy(n => y(n, i++)).ToList();
			}, 2)),

			//Function "儿", Binary (List, Lambda[T,Number]) -> List
			//Sort descending with lambda
			("儿", new HigherOrderFunction<List<VObject>>((x,y) => {
				int i = 0;
				return x.OrderByDescending(n => y(n, i++)).ToList();
			}, 2)),

			//Function "多", Binary (List, Number) -> T
			//Get nth element (negative gets from back)
			("多", new BinaryFunction<List<VObject>, double>((x,y) => x[y < 0 ? x.Count - (int)y : (int)y])),
			//Function "多", Binary (List, List) -> List
			//Foreach value in l, get nth element (negative gets from back)
			("多", new BinaryFunction<List<VObject>, List<VObject>>((x,y) => y.Where(n => n.ObjectType == ObjectType.Number || (n.ObjectType == ObjectType.String && int.TryParse(n, out _)))
			.Select(n => {
				if(n.ObjectType == ObjectType.String)
					if(int.TryParse(n, out int i))
						return x[i < 0 ? x.Count - i : i];
					//Saves a bit of code (acceptable in this case because the error label is very close
					else goto error;
				else if(n.ObjectType == ObjectType.Number)
					return x[n < 0 ? x.Count - (int)n : (int)n];
				else goto error;
				error: {
					ErrorHandler.InternalError("In function 多 (List, List) -> List, an object in l got past the filter:" + n.ToString());
					throw new Exception();
				}
			}).ToList())),

			//Function "意", Binary (List, List) -> Number
			//Contains the certain VObject (in this case list)
			("意", new BinaryFunction<List<VObject>,List<VObject>>((x,y) =>  x.Contains(y))),
			//Function "意", Binary (List, Number) -> Number
			//Contains the certain VObject (in this case number)
			("意", new BinaryFunction<List<VObject>,double>((x,y) => x.Contains(y))),
			//Function "意", Binary (List, String) -> Number
			//Contains the certain VObject (in this case string)
			("意", new BinaryFunction<List<VObject>,string>((x,y) => x.Contains(y))),

			//Function "别", Binary (List, Lambda[T,Number]) -> T
			//Given that L is of the structure [l, n] or [n, l], repeatedly pass the last element into the lambda and append it onto l until l is of n length, then return the nth element of l
			("别", new HigherOrderFunction<List<VObject>>((x,y) => {
				x = x.OrderByDescending(n => n.ObjectType).ToList();
				if(x.Count != 2 || x[0].ObjectType != ObjectType.List || x[1].ObjectType != ObjectType.Number || x[1] < 0) {
					ErrorHandler.Error("The caller list was not of the structure [l, n]");
				}
				//Call ToList to prevent any reference shenanigans
				List<VObject> list = ((List<VObject>)x[0]).ToList();
				int target = (int)x[1];
				while(list.Count < target) {
					list.Add(y(x.Last(), list.Count, list));
				}
				return list[target];
			}, 3)),

			//Function "所", Binary (List, Lambda[T,Number]) -> T
			//Given that L is of the structure [l, n] or [n, l], repeatedly pass the last element into the lambda and append it onto l until l is of n length, then return the first n elements of l
			("所", new HigherOrderFunction<List<VObject>>((x,y) => {
				x = x.OrderByDescending(n => n.ObjectType).ToList();
				if(x.Count != 2 || x[0].ObjectType != ObjectType.List || x[1].ObjectType != ObjectType.Number || x[1] < 0) {
					ErrorHandler.Error("The caller list was not of the structure [l, n]");
				}
				//Call ToList to prevent any reference shenanigans
				List<VObject> list = ((List<VObject>)x[0]).ToList();
				int target = (int)x[1];
				while(list.Count < target) {
					list.Add(y(x.Last(), list.Count, list));
				}
				//In case the list provided was longer than the target to begin with
				return list.Take(target).ToList();
			}, 3)),

			//Function "话", Binary (List, List) -> List
			//
			("话", new BinaryFunction<List<VObject>,List<VObject>>((x,y) => x)),

			//Function "小", Binary (List, List) -> List
			//
			("小", new BinaryFunction<List<VObject>,List<VObject>>((x,y) => x)),

			//Function "自", Binary (List, List) -> List
			//
			("自", new BinaryFunction<List<VObject>,List<VObject>>((x,y) => x)),

			//Function "回", Binary (List, List) -> List
			//
			("回", new BinaryFunction<List<VObject>,List<VObject>>((x,y) => x)),

			//Function "然", Binary (List, List) -> List
			//
			("然", new BinaryFunction<List<VObject>,List<VObject>>((x,y) => x)),

			//Function "果", Binary (List, List) -> List
			//
			("果", new BinaryFunction<List<VObject>,List<VObject>>((x,y) => x)),

			//Function "发", Binary (List, List) -> List
			//
			("发", new BinaryFunction<List<VObject>,List<VObject>>((x,y) => x)),

			//Function "见", Binary (List, List) -> List
			//
			("见", new BinaryFunction<List<VObject>,List<VObject>>((x,y) => x)),

			//Function "心", Binary (List, List) -> List
			//
			("心", new BinaryFunction<List<VObject>,List<VObject>>((x,y) => x)),

			//Function "走", Binary (List, List) -> List
			//
			("走", new BinaryFunction<List<VObject>,List<VObject>>((x,y) => x)),

			//Function "定", Binary (List, List) -> List
			//
			("定", new BinaryFunction<List<VObject>,List<VObject>>((x,y) => x)),

			//Function "听", Binary (List, List) -> List
			//
			("听", new BinaryFunction<List<VObject>,List<VObject>>((x,y) => x)),

			//Function "觉", Binary (List, List) -> List
			//
			("觉", new BinaryFunction<List<VObject>,List<VObject>>((x,y) => x)),

			//Function "太", Binary (List, List) -> List
			//
			("太", new BinaryFunction<List<VObject>,List<VObject>>((x,y) => x)),

			//Function "该", Binary (List, List) -> List
			//Pair
			("该", new BinaryFunction<List<VObject>,List<VObject>>((x,y) => new List<VObject>{ x, y })),			
			//Function "该", Binary (List, Number) -> List
			//Pair
			("该", new BinaryFunction<List<VObject>,double>((x,y) => new List<VObject>{ x, y })),
			//Function "该", Binary (List, String) -> List
			//Pair
			("该", new BinaryFunction<List<VObject>,string>((x,y) => new List<VObject>{ x, y })),

			//Function "当", Binary (List, List) -> Number
			//Equals
			("当", new BinaryFunction<List<VObject>,List<VObject>>((x,y) => new VObject(x).Equals(new VObject(y)))),
			//Function "当", Binary (List, String) -> Number
			//Equals
			("当", new BinaryFunction<List<VObject>,string>((x,y) => false)),
			//Function "当", Binary (List, Number) -> Number
			//Equals
			("当", new BinaryFunction<List<VObject>,double>((x,y) => false)),
			#endregion
		});
	}

	public abstract class UnaryFunction : Function {
		public abstract VObject Invoke(VObject caller);
	}


	public class UnaryFunction<T> : UnaryFunction {

		//For regular functions
		private Func<T, VObject> Behavior { get; }

		//For Unary Functions
		public UnaryFunction(Func<T, VObject> behavior) {
			Behavior = behavior;
		}

		public override VObject Invoke(VObject caller) {
			return Behavior(caller.ConvertTo<T>());
		}
	}

	public abstract class UnaryFunctionWithProgramState : UnaryFunction {
		public abstract VObject Invoke(VObject caller, ProgramState programState);
    }

	public class UnaryFunctionWithProgramState<T> : UnaryFunctionWithProgramState {

		private Func<T, ProgramState, VObject> Behavior { get; }
		public UnaryFunctionWithProgramState(Func<T, ProgramState, VObject> behavior) {
			Behavior = behavior;
        }
		public override VObject Invoke(VObject caller, ProgramState programState) {
			return Behavior(caller.ConvertTo<T>(), programState);
        }
		
		//For abstract
		public override VObject Invoke(VObject caller) {
			return null;
		}
	}

	public abstract class BinaryFunction : Function {
		public abstract VObject Invoke(VObject caller, VObject arg);
	}

	public class BinaryFunction<T1, T2> : BinaryFunction {

		private Func<T1, T2, VObject> Behavior { get; }

		public BinaryFunction(Func<T1, T2, VObject> behavior) {
			Behavior = behavior;
		}

		public override VObject Invoke(VObject caller, VObject arg) {
			return Behavior(caller.ConvertTo<T1>(), arg.ConvertTo<T2>());
		}
	}
	public delegate VObject Lambda(params VObject[] args);

	public abstract class HigherOrderFunction : Function {

		//Designates how much parameters a lambda that is passed into this function takes
		public int LambdaParameters { get; }
		//If user does not pass in a lambda, use this
		public Lambda DefaultLambda { get; }

		protected HigherOrderFunction(int lambdaParameters, Lambda defaultLambda) {
			LambdaParameters = lambdaParameters;
			DefaultLambda = defaultLambda;
		}

		protected HigherOrderFunction(int lambdaParameters) {
			LambdaParameters = lambdaParameters;
			//Filled in interpreter
			DefaultLambda = null;
		}

		public abstract VObject Invoke(VObject caller, Lambda lambda);
	}

	//For higher-order functions
	public class HigherOrderFunction<T> : HigherOrderFunction {

		private Func<T, Lambda, VObject> Behavior { get; }

		public HigherOrderFunction(Func<T, Lambda, VObject> behavior, int lambdaParameters) : base(lambdaParameters) {
			Behavior = behavior;
		}

		public HigherOrderFunction(Func<T, Lambda, VObject> behavior, int lambdaParameters, Lambda defaultLambda) : base(lambdaParameters, defaultLambda) {
			Behavior = behavior;
		}

		//This overload is only used for HigherOrderFunctions
		public override VObject Invoke(VObject caller, Lambda lambda) {
			return Behavior(caller.ConvertTo<T>(), lambda);
		}
	}
}
