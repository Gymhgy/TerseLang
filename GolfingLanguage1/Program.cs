using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static TerseLang.Constants;

namespace TerseLang {

    class Program {
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

            List<VObject> inputs = new List<VObject>();

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

            if (program.Contains("\n"))
                ErrorHandler.Error("Sorry! Programs cannot contain newline characters just yet. Hopefully this will be fixed soon");

            var interpreter = new Interpreter(program, inputs.ToArray());

            var result = interpreter.Interpret().Last();

            Console.WriteLine(result.ToString());
        }

        static VObject ParseInput(string input) {
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
                    for (; (depth > 0 || inStr || input[j + i] != ',') && j + i < contents.Length; j++) {
                        if (input[j + i] == '"') {
                            if (!inStr) {
                                strClose = '"';
                                inStr = true;
                            }
                            else if (strClose == '"') {
                                inStr = false;
                            }
                        }
                        if (input[j + i] == '\'') {
                            if (!inStr) {
                                strClose = '\'';
                                inStr = true;
                            }
                            else if (strClose == '\'') {
                                inStr = false;
                            }
                        }
                        if (!inStr && input[j + i] == '[') {
                            depth++;
                        }
                        if (!inStr && input[j + i] == ']') {
                            depth--;
                        }
                    }
                    i += j;
                    list.Add(contents.Substring(i, j));
                }
                return list;
            }
            else {
                ErrorHandler.Error("Unable to parse input");
                throw new Exception();
            }
        }
    }
}
