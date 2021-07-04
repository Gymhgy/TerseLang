using System;
using System.Collections.Generic;
using System.Text;
using TerseLang.Expressions;
using System.Linq;
using static TerseLang.Constants;

namespace TerseLang {
    public class Interpreter {


        private Queue<string> ParamVars { get; }

        private IReadOnlyCollection<Expression> AST { get; }

        private ProgramState programState { get; }

        public Interpreter(string program, VObject[] Input) {
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

        public IList<VObject> Interpret() {
            //For each expression in the pareser, evaluate them and add them to the list or results
            List<VObject> results = new List<VObject>();
            foreach (var expr in AST) {
                results.Add(Evaluate(expr));
            }
            return results;
        }


        //How the value of an AutoExpression is decided
        //If the AutoExpression is the parameter to a higher order function
        // - Return the input
        //If we are inside a higher-order function
        // - The 'parameter variables' are 斯,成,它,感,干,法
        // - They are placed upon a stack
        // - Based on how many is needed, that number is popped from the stack
        // - E.g some higher-order functions require 3 parameters, some only need 2 or 1
        // - If there are at least two parameter variables, the first one will be designated as the first autofill
        // - and the second will be designated as the second autofill
        //Else if we are not inside a higher-order function
        // - The first autofill will be the first input passed to the program
        // - The second autofill will be the second input passed to the program
        //Now we have our first and second autofill
        // - If there are zero autofill, just use 0
        // - If there is only one autofill, use that one autofill
        // - If there are at least two autofills defined:
        //   - If the expression is an argument to a function, use the first autofill
        //   - Use the second autofill
        //   - Else just use the first

        VObject Evaluate(Expression ast) {
            switch (ast) {
                //Self-explanatory
                case NumericLiteralExpression number:
                    return new VObject(number.Value);
                case StringLiteralExpression str:
                    return new VObject(str.Value);
                case ListExpression array:
                    return new VObject(array.Contents.Select(Evaluate).ToList());

                case VariableReferenceExpression variable:
                    return programState.Variables[variable.Name];

                case ConditionalExpression conditional:
                    if (Evaluate(conditional.Condition).IsTruthy()) {
                        return Evaluate(conditional.TrueExpression);
                    }
                    else
                        return Evaluate(conditional.FalseExpression);
                //Use the first input as autofill by default
                case AutoExpression _:
                    return programState.Autofill_1;

                case FunctionInvocationExpression funcExpr:
                    VObject caller;
                    if (funcExpr.Caller is AutoExpression)
                        caller = programState.Autofill_1;
                    else
                        caller = Evaluate(funcExpr.Caller);

                    if (Function.IsHigherOrder(funcExpr.Function, caller.ObjectType)) {
                        HigherOrderFunction func = (HigherOrderFunction)Function.Get(funcExpr.Function, caller.ObjectType);
                        Lambda lambda;
                        // If an autoexpression is submitted as a lambda, then either use the default lambda or if not available, a lambda that returns the first autofill (the first input)
                        if (funcExpr.Arguments[0] is AutoExpression) {
                            lambda = func.DefaultLambda ??  (_ => programState.Autofill_1);
                        }
                        else
                            lambda = CreateLambda(funcExpr.Arguments[0], func.LambdaParameters);


                        //We pass the lambda that we created into the function
                        var res = func.Invoke(caller, lambda);
                        //Re-rotate the parameter variables back
                        //We rotated them in the CreateLambda method
                        for (int i = 0; i < PARAMETER_VARIABLES.Length - func.LambdaParameters; i++) {
                            ParamVars.Enqueue(ParamVars.Dequeue());
                        }

                        return res;
                    }

                    if (Function.IsUnary(funcExpr.Function)) {
                        UnaryFunction func = (UnaryFunction)Function.Get(funcExpr.Function, caller.ObjectType);
                        if (func is UnaryFunctionWithProgramState funcWithProgState) return funcWithProgState.Invoke(caller, programState);
                        else return func.Invoke(caller);
                    }
                    else {
                        VObject arg = funcExpr.Arguments[0] is AutoExpression ? programState.Autofill_2 : Evaluate(funcExpr.Arguments[0]);
                        BinaryFunction func = (BinaryFunction)Function.Get(funcExpr.Function, caller.ObjectType, arg.ObjectType);
                        return func.Invoke(caller, arg);
                    }
                    throw new Exception();
                default:
                    ErrorHandler.InternalError("This shouldn't happen.");
                    throw new Exception();
            }
        }

        // When a function takes in a lambda expression
        // This creates it
        Lambda CreateLambda(Expression lambda, int lambdaParams) {
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

                //Set the autofill variable names to the names of the parameter variables
                //And store the old values into temp variables
                string oldAutofill1Name = programState.Autofill1Name;
                string oldAutofill2Name = programState.Autofill2Name;
                programState.Autofill1Name = parameterNames[0];
                programState.Autofill2Name = parameterNames[1];

                //Now evaluate the lambda with the parameter variables and autofills properly set
                var result = Evaluate(lambda);

                //Reset the autofill variable names to what they were before
                programState.Autofill1Name = oldAutofill1Name;
                programState.Autofill2Name = oldAutofill2Name;

                //Reset the parameter variables to their old values
                i = 0;
                paramArgPairs.ForEach((paramArgPair) => {
                    var (paramName, _) = paramArgPair;
                    programState.Variables[paramName] = oldValues[i];
                });

                return result;
            }
            return func;
        }



    }

}
