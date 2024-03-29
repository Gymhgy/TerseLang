using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static TerseLang.Constants;

namespace TerseLang {

    public class Program {
        static void Main(string[] args) {
            if (args.Length == 0 || args[0] == "help") {
                Console.WriteLine(USAGE);
                return;
            }
            if (args.Length == 1 && args[0] != "-i") {
                Console.WriteLine("Invalid Usage");
                Console.WriteLine(USAGE);
            }
            string program = "";

            if (args[0] == "-f") {
                var bytes = File.ReadAllBytes(args[1]);
                foreach(byte b in bytes) {
                  program += CHARSET[b];
                }
            }
            if (args[0] == "-p") {
                program = args[1];
            }

            List<dynamic> inputs = new List<dynamic>();

            //Interactive Mode
            if(args[0] == "-i") {

              Console.Write(">>> ");
              program = Console.ReadLine();
              Console.Write(">>> ");
              foreach(var input in Console.ReadLine().Split())
                inputs.Add(ParseInput(input));
            }
            //2 is for flag + program
            else if (args.Length > 2) {
                foreach(var input in args.Skip(2)) {
                    inputs.Add(ParseInput(input));
                }
            }

            var interpreter = new Interpreter(program, inputs.ToArray());

            var result = interpreter.Interpret();

            Console.WriteLine(result.ToString());
        }

        public static dynamic ParseInput(string input) {
            input = input.Trim();
            if (double.TryParse(input, out double d))
                return d;
            if(input[0] == '"' && input.Last() == '"' || input[0] == '\'' && input.Last() == '\'') {
                return input.Substring(1, input.Length - 2);
            }
            if (input[0] == '[' && input.Last() == ']') {
                var list = new List<dynamic>();
                var contents = input.Substring(1, input.Length - 2);
                for (int i = 0; i < contents.Length; i++) {
                    bool inStr = false;
                    char strClose = '\0';
                    int depth = 0;
                    int j = 0;
                    for (; j + i < contents.Length && (depth > 0 || inStr || contents[j + i] != ','); j++) {
                        if (contents[j + i] == '"') {
                            if (!inStr) {
                                strClose = '"';
                                inStr = true;
                            }
                            else if (strClose == '"') {
                                inStr = false;
                            }
                        }
                        if (contents[j + i] == '\'') {
                            if (!inStr) {
                                strClose = '\'';
                                inStr = true;
                            }
                            else if (strClose == '\'') {
                                inStr = false;
                            }
                        }
                        if (!inStr && contents[j + i] == '[') {
                            depth++;
                        }
                        if (!inStr && contents[j + i] == ']') {
                            depth--;
                        }
                    }
                    list.Add(ParseInput(contents.Substring(i, j)));
                    i += j;
                }
                return list;
            }
            else {
                ErrorHandler.Error("Unable to parse input:" + input);
                throw new Exception();
            }
        }
    }
}
