using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;

using System.Text;
using System.Linq;
using static TerseLang.Constants;

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

        public ProgramState(IList<dynamic> Inputs) {
            //Initialize input variables
            INPUT_VARIABLES.Zip(Inputs).Take(4).ToList().ForEach(pair => {
                var (varName, val) = pair;
                Variables[varName.ToString()] = val;
            });
            //Initialize input array
            Variables["面"] = Inputs;
            if (Inputs.Count == 1) Autofill2Name = INPUT_VARIABLES[0].ToString();
        }
    }
}
