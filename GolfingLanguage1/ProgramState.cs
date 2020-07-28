using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;

using System.Text;
using System.Linq;
using static GolfingLanguage1.Constants;

namespace GolfingLanguage1 {
    public static class ProgramState {

        //Default values, which are the input variables
        //These will change when a lambda is evaluated
        //in which they will have the value of a lambda parameter variable name
        //Or it can change when a function that changes autofills is called
        public static string Autofill1Name = INPUT_VARIABLES[0].ToString();
        public static string Autofill2Name = INPUT_VARIABLES[1].ToString();

        public static VObject Autofill_1 => Variables[Autofill1Name];
        public static VObject Autofill_2 => Variables[Autofill2Name];

        public static Dictionary<string, VObject> Variables = new Dictionary<string, object>
        {
            //Inputs
            ["哦"] = 0,
            ["情"] = 0,
            ["作"] = 0,
            ["跟"] = 0,
            // Input Array
            ["面"] = 0,

            //Unused variables (for now)
            ["诉"] = new List<VObject>(),
            ["爱"] = 0,

            ["已"] = " ",
            ["之"] = "\n",
            ["问"] = "",
            ["错"] = -1,
            //Function parameters
            ["孩"] = 10,
            ["斯"] = 16,
            ["成"] = 15,
            ["它"] = 14,
            ["感"] = 13,
            ["干"] = 12,
            ["法"] = 11,
            ["电"] = 100,
            //Assign

            ["间"] = 1000,

            //Hidden Variables that cannot be changed normally
            ["HiddenAutofill"] = 0,

        }.ToDictionary(x => x.Key, x => new VObject(x.Value));

        public static void Initialize(IList<VObject> Inputs) {
            //Initialize input variables
            INPUT_VARIABLES.Zip(Inputs).Take(4).ToList().ForEach(pair => {
                var (varName, val) = pair;
                Variables[varName.ToString()] = val;
            });
            //Initialize input array

            Variables["面"] = new VObject(Inputs.ToList());
        }
    }
}
