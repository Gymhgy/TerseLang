using System;
using System.Collections.Generic;
using System.Text;

namespace GolfingLanguage1 {

    //Contains all the constants
    public static class Constants {


        public const string CHARSET = 
            "\t\n !#$%&'(*+,-./0123456789:;<=>?@" +
            "ABCDEFGHIJKLMNOPQRSTUVWXYZ\\^_`abcdefghijklmnopqrstuvwxyz{|~" +
            "【：；。，？、！￥…（—" +
            "你我的是了不们这一他么在有个好来人那要会就" +
            "什没到说吗为想能上去道她很看可知得过吧还对里以都事子生时样也和下真现做大" +
            "啊怎出点起天把开让给但谢着只些如家后儿多意别所话小自回然果发见心走定听觉" +
            "太该当" +
            // MONADIC FUNCTIONS
            "经妈用打地再因呢女告最手前找行快而死先像等被从明中" +
            //VARIABLES
            "哦情作跟面诉爱已之问错孩斯成它感干法电间" +
            //STRING STUFF
            "“”\"‘’" +
            //PUNCTUATION
            "[])}）】";

        //X is a placeholder
        public const char STRING_DELIMITER = '“';
        public const char COMPRESSED_STRING_DELIMITER = '”';
        public const char COMPRESSED_NUMBER_DELIMITER = '"';
        public const char NEWLINE_SUB = '￥';

        public const char SINGLE_CHAR_STRING = '\'';
        public const char DOUBLE_CHAR_STRING = '’';
        public const char TRIPLE_CHAR_STRING = '‘';


        public const string VARIABLES = "哦情作跟面诉爱已之问错孩斯成它感干法电间";
        public const string PUNCTUATION = "[])}）】";

        public const string BRACKETS = ")}）";
        public const char CLOSE_ALL = '】';

        public const string USAGE = "PLACEHOLDER";


    }
}
