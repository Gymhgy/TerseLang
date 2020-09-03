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
            if (args.Length == 1) {
                Console.WriteLine("Invalid Usage");
                Console.WriteLine(USAGE);
            }
            string program = "";

            if (args[0] == "-f") {
                program = File.ReadAllText(args[1]);
            }
            if (args[0] == "-p") {
                program = args[1];
            }
            List<VObject> inputs = new List<VObject>();
            //2 is for flag + program
            if (args.Length > 2) {
                foreach(var input in args.Skip(2)) {
                    inputs.Add(ParseInput(input));
                }
            }

            if (program.Contains("\n"))
                ErrorHandler.Error("Sorry! Programs cannot contain newline characters just yet. Hopefully this will be fixed soon");

            var interpreter = new Interpreter(program, inputs.ToArray());

            var result = interpreter.Interpret().Last();

            Console.WriteLine(result.ToString());
        }

        public static VObject ParseInput(string input) {
            input = input.Trim();
            if (double.TryParse(input, out double d))
                return d;
            if(input[0] == '"' && input.Last() == '"' || input[0] == '\'' && input.Last() == '\'') {
                return input.Substring(1, input.Length - 2);
            }
            if (input[0] == '[' && input.Last() == ']') {
                var list = new List<VObject>();
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
