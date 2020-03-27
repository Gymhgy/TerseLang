using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;

using System.Text;
using System.Linq;

namespace GolfingLanguage1 {
    public static class ProgramState {
        public static ReadOnlyDictionary<string, VObject> Variables = new ReadOnlyDictionary<string, VObject>(new Dictionary<string, object>
        {



        }.ToDictionary(x => x.Key, x => new VObject(x.Value)));

        public static VObject[] Input;
    }
}
