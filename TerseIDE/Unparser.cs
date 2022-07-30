using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TerseLang;
using TerseLang.Expressions;
using static TerseLang.Constants;


namespace TerseIDE {
    public class Unparser {


        private Queue<string> ParamVars { get; }

        private IReadOnlyCollection<Expression> AST { get; }

        private ProgramState programState { get; }

        public Unparser(string program, VObject[] Input) {
            //Pass the program into the parser
            AST = Parser.Parse(program);
            programState = new ProgramState(Input);
            //Initialize parameter variable queue
            //Used for setting up lambdas
            ParamVars = new Queue<string>();
            foreach (var paramVar in PARAMETER_VARIABLES) {
                ParamVars.Enqueue(paramVar.ToString());
            }
        }

        public string Unparse() {
            //For each expression in the pareser, unparse them
            List<string> results = new List<string>();
            foreach (var expr in AST) {
                results.Add(Unparse(expr).unparsed);
            }
            return string.Join("\n", results);
        }

        (string unparsed, VObject val) Unparse(Expression ast) {
            switch (ast) {
                //Self-explanatory
                case NumericLiteralExpression number:
                    return (number.Value + " ", new VObject(number.Value));
                case StringLiteralExpression str:
                    return ('"' + str.Value + '"', new VObject(str.Value));
                case VariableReferenceExpression variable:
                    return (variable.Name, programState.Variables[variable.Name]);

                case ConditionalExpression conditional:
                    var (unparsedCondition, conditionVal) = Unparse(conditional.Condition);
                    var (unparsedTrueStatement, trueVal) = Unparse(conditional.TrueExpression);
                    var (unparsedFalseStatement, falseVal) = Unparse(conditional.FalseExpression);
                    string unparsed = unparsedCondition + " ? " + unparsedTrueStatement + " : " + unparsedFalseStatement;
                    VObject val = conditionVal.IsTruthy() ? trueVal : falseVal;
                    return (unparsed, val);
                //Use the first input as autofill by default
                case AutoExpression _:
                    return (programState.Autofill1Name, programState.Autofill_1);

                case FunctionInvocationExpression funcExpr:
                    VObject caller;
                    string unparsedCaller = "";
                    if (funcExpr.Caller is AutoExpression) {
                        caller = programState.Autofill_1;
                        unparsedCaller = programState.Autofill1Name;
                    }
                    else {
                        (unparsedCaller, caller) = Unparse(funcExpr.Caller);
                    }

                    if (Function.IsHigherOrder(funcExpr.Function, caller.ObjectType)) {
                        HigherOrderFunction func = (HigherOrderFunction)Function.Get(funcExpr.Function, caller.ObjectType);
                        Lambda lambda;
                        List<string> parameterList = new List<string>();
                        bool createdLambda = false;
                        VObject res;
                        string unparsedLambda;
                        // If an autoexpression is submitted as a lambda, then either use the default lambda or if not available, a lambda that returns the first autofill (the first input)
                        if (funcExpr.Argument is AutoExpression) {
                            lambda = func.DefaultLambda ?? (_ => programState.Autofill_1);
                            res = func.Invoke(caller, lambda);
                            unparsedLambda = Unparse(funcExpr.Argument).unparsed;
                        }
                        else {
                            (lambda, parameterList) = CreateLambda(funcExpr.Argument, func.LambdaParameters);
                            createdLambda = true;
                            //Set the autofill variable names to the names of the parameter variables
                            //And store the old values into temp variables
                            string oldAutofill1Name = programState.Autofill1Name;
                            string oldAutofill2Name = programState.Autofill2Name;
                            programState.Autofill1Name = parameterList[0];
                            if (func.LambdaParameters > 1)
                                programState.Autofill2Name = parameterList[1];

                            //Now evaluate the lambda with the parameter variables and autofills properly set
                            res = func.Invoke(caller, lambda);
                            unparsedLambda = Unparse(funcExpr.Argument).unparsed;
                            //Reset the autofill variable names to what they were before
                            programState.Autofill1Name = oldAutofill1Name;
                            programState.Autofill2Name = oldAutofill2Name;
                        }

                        //Re-rotate the parameter variables back
                        //We rotated them in the CreateLambda method
                        //Note this only triggers if CreateLambda was called
                        if (createdLambda) {
                            for (int i = 0; i < PARAMETER_VARIABLES.Length - func.LambdaParameters; i++) {
                                ParamVars.Enqueue(ParamVars.Dequeue());
                            }
                        }

                        var paramDeclare = "(" + string.Join(",", parameterList) + ")";
                        string unparsedFunc = $"{unparsedCaller}.{funcExpr.Function}(default)";
                        if (createdLambda)
                            unparsedFunc = $"{unparsedCaller}.{funcExpr.Function}({paramDeclare}=>{unparsedLambda})";
                        return (unparsedFunc, res);
                    }

                    if (Function.IsUnary(funcExpr.Function)) {
                        try {
                            UnaryFunction func = (UnaryFunction)Function.Get(funcExpr.Function, caller.ObjectType);
                            VObject res;
                            if (func is UnaryFunctionWithProgramState funcWithProgState) res = funcWithProgState.Invoke(caller, programState);
                            else res = func.Invoke(caller);
                            return ($"{unparsedCaller}.{funcExpr.Function}()", res);
                        }
                        catch (TerseLang.ProgramErrorException) {
                            return ($"{unparsedCaller}.{funcExpr.Function}()", caller);
                        }
                    }
                    else {
                        try {
                            VObject arg = funcExpr.Argument is AutoExpression ? programState.Autofill_2 : Unparse(funcExpr.Argument).val;
                            string unparsedArg = funcExpr.Argument is AutoExpression ? programState.Autofill2Name : Unparse(funcExpr.Argument).unparsed;
                            BinaryFunction func = (BinaryFunction)Function.Get(funcExpr.Function, caller.ObjectType, arg.ObjectType);
                            return ($"{unparsedCaller}.{funcExpr.Function}({unparsedArg})", func.Invoke(caller, arg));
                        }
                        catch (TerseLang.ProgramErrorException) {
                            string unparsedArg = funcExpr.Argument is AutoExpression ? programState.Autofill2Name : Unparse(funcExpr.Argument).unparsed;
                            return ($"{unparsedCaller}.{funcExpr.Function}({unparsedArg})", caller);
                        }
                    }
                    throw new Exception();
                default:
                    ErrorHandler.InternalError("Error with unparser: " + ast.ToString());
                    throw new Exception();
            }
        }

        // When a function takes in a lambda expression
        // This creates it
        (Lambda lambda, List<string> parameterNames) CreateLambda(Expression lambda, int lambdaParams) {
            //Set up the list of parameters that this lambda will use
            List<string> parameterNames = new List<string>(lambdaParams);
            for (int i = 0; i < lambdaParams; i++) {
                parameterNames.Add(ParamVars.Peek());
                //Rotate the parameter variable list
                ParamVars.Enqueue(ParamVars.Dequeue());
            }
            //This is the function that will be returned
            //Everytime the lambda is used, this is what is being executed
            VObject func(VObject[] args) {
                var paramArgPairs = parameterNames.Zip(args).ToList();
                var oldValues = new VObject[lambdaParams];
                int i = 0;

                //Set the values of the parameter variables to whatever arguments were passed
                paramArgPairs.ForEach((paramArgPair) => {
                    var (paramName, argVal) = paramArgPair;
                    oldValues[i++] = programState.Variables[paramName];
                    programState.Variables[paramName] = argVal;
                });


                //Now evaluate the lambda with the parameter variables and autofills properly set
                var result = Unparse(lambda);

                //Reset the parameter variables to their old values
                i = 0;
                paramArgPairs.ForEach((paramArgPair) => {
                    var (paramName, _) = paramArgPair;
                    programState.Variables[paramName] = oldValues[i];
                });

                return result.val;
            }
            return (func, parameterNames);
        }



    }
}