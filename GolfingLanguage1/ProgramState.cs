using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;

using System.Text;
using System.Linq;
using static TerseLang.Constants;
using TerseLang.Expressions;

namespace TerseLang {
    public class ProgramState {

        //Default values, which are the input variables
        //These will change when a lambda is evaluated
        //in which they will have the value of a lambda parameter variable name
        //Or it can change when a function that changes autofills is called
        public string Autofill1Name = INPUT_VARIABLES[0].ToString();
        public string Autofill2Name = INPUT_VARIABLES[1].ToString();

        public dynamic Autofill_1 => Variables[Autofill1Name];
        public dynamic Autofill_2 => Variables[Autofill2Name];

        public Dictionary<string, dynamic> Variables = new Dictionary<string, object>
        {
            //Inputs
            ["哦"] = 0d,
            ["情"] = 0d,
            ["作"] = 0d,
            ["跟"] = 0d,
            // Input Array
            ["面"] = 0d,

            //Assignable Autofill
            ["爱"] = 0d,

            ["已"] = " ",
            ["之"] = "\n",
            ["问"] = "",
            ["错"] = -1d,
            ["孩"] = 10d,
            //Function parameters
            ["斯"] = 16d,
            ["成"] = 15d,
            ["它"] = 14d,
            ["感"] = 13d,
            ["干"] = 12d,
            ["法"] = 11d,

            ["电"] = 100d,

            ["间"] = 1000d,

        };

        private readonly Interpreter Interpreter;

        public ProgramState(IList<dynamic> Inputs,Interpreter interpreter) {
            Interpreter = interpreter;
            
            //Initialize input variables
            INPUT_VARIABLES.Zip(Inputs).Take(4).ToList().ForEach(pair => {
                var (varName, val) = pair;
                Variables[varName.ToString()] = val;
            });
            //Initialize input array
            Variables["面"] = Inputs;
            if (Inputs.Count == 1) Autofill2Name = INPUT_VARIABLES[0].ToString();
        }

        public dynamic ExecuteNth(int n, params dynamic[] inputs) {
            dynamic[] oldValues = new dynamic[inputs.Length];
            int i = 0;
            var inputVarPair = INPUT_VARIABLES.Zip(inputs).Take(4).ToList();
            inputVarPair.ForEach(pair => {
                var (varName, val) = pair;
                oldValues[i++] = Variables[varName.ToString()];
                Variables[varName.ToString()] = val;
            });
            string oldAutofill1Name = Autofill1Name;
            string oldAutofill2Name = Autofill2Name;
            Autofill1Name = INPUT_VARIABLES[0].ToString();
            Autofill2Name = inputs.Length == 1 ? INPUT_VARIABLES[0].ToString() : INPUT_VARIABLES[1].ToString();

            var res = Interpreter.ExecuteNth(n);

            Autofill1Name = oldAutofill1Name;
            Autofill2Name = oldAutofill2Name;

            i = 0;
            inputVarPair.ForEach((inputVar) => {
                var (inputName, _) = inputVar;
                Variables[inputName.ToString()] = oldValues[i++];
            });


            return res;
        }

        public dynamic ExecutePrevious(params dynamic[] inputs) => ExecuteNth(Interpreter.CurrentExpression - 1, inputs);
        private void Prep(IList<dynamic> inputs) {
            INPUT_VARIABLES.Zip(inputs).Take(4).ToList().ForEach(pair => {
                var (varName, val) = pair;
                Variables[varName.ToString()] = val;
            });
        }
    }
}
