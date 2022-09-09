using System;
using System.Collections.Generic;
using System.Text;

namespace TerseLang {

    //Contains all the constants
    public static class Constants {


        public const string 
            TIER_ZERO = "ABCDEFGHIJKLMNOPQRSTUVWXYZ!#$%&(*+,-/:;<=>@\\",
            TIER_ONE = "abcdefghijklmnopqrstuvwxyz^_`{|~？、！￥…（—你我的是了",
            TIER_TWO = "不们这一他么在有个好来人那要会就什没到说吗为想能上去道她很看可知得过吧还对里以都事子生时",
            TIER_UNLIMITED = "样也和下真现做大啊怎出点起天把开让给但谢着只些如家后儿多意别所话小自回然果发见心走定听觉";

        public const string UNARY_FUNCTIONS = "太该当经妈用打地再因呢女告最手前找行快而死先像等被从明中";

        public const string FUNCTIONS = TIER_ZERO + TIER_ONE + TIER_TWO + TIER_UNLIMITED + UNARY_FUNCTIONS;

        public const char STRING_DELIMITER = '"';
        public const char COMPRESSED_STRING_DELIMITER = '”';
        public const char COMPRESSED_NUMBER_DELIMITER = '“';

        public const char SINGLE_CHAR_STRING = '\'';
        public const char DOUBLE_CHAR_STRING = '’';
        public const char TRIPLE_CHAR_STRING = '‘';

        public const char STRING_LIST_SEPERATOR = '‘';

        public const string PUNCTUATION = "[])}）】.?";

        public const string BRACKETS = ")}）";
        public const char CLOSE_ALL = '】';

        public const string VARIABLES = "哦情作跟面诉爱已之问错孩斯成它感干法电间";
        public const string PARAMETER_VARIABLES = "斯成它感干法";
        public const string INPUT_VARIABLES = "哦情作跟";

        public const char IF = '?';

        public const string USAGE = "Usage: terse [-f path|-p program] [arguments...]\n" + 
                                    "       terse -i (interactive mode, enter program on first line, enter input on second)";


        public const string CHARSET =
            //FUNCTIONS
            FUNCTIONS +
            //VARIABLES
            VARIABLES +
            //STRING STUFF
            "“”\"'‘’" +
            //DIGITS
            "0123456789" +
            //PUNCTUATION
            "?)}）】." +
            //For later use
            " \n\t" +
            //For later use
            "[]【：；。，";
    }
}
