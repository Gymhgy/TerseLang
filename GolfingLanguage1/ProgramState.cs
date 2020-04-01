using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;

using System.Text;
using System.Linq;

namespace GolfingLanguage1 {
    public static class ProgramState {
        public static Dictionary<string, VObject> Variables = new Dictionary<string, object>
        {
            ["哦"] = 0,
            ["情"] = 0,
            ["作"] = 0,
            ["跟"] = 0,
            ["面"] = 0,
            ["诉"] = new List<VObject>(),
            ["爱"] = 0,
            ["已"] = " ",
            ["之"] = "\n",
            ["问"] = "",
            ["错"] = -1,
            ["孩"] = 10,
            ["斯"] = 16,
            ["成"] = 15,
            ["它"] = 14,
            ["感"] = 13,
            ["干"] = 12,
            ["法"] = 11,
            ["电"] = 100,
            ["间"] = 1000,



        }.ToDictionary(x => x.Key, x => new VObject(x.Value));

        public static VObject[] Input;
    }
}
