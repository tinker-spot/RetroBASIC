using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using RetroBASIC;

namespace RetroBASICDriver
{
    public class ConsoleCommands
    {
        static public char CommandCharPrefix = '@';

        string inputLine;
        Interpreter interpreter;

        public ConsoleCommands(Interpreter _interpreter)
        {
            interpreter = _interpreter;
        }

        public void Execute(string _inputLine)
        {
            inputLine = _inputLine;
            var (command, parameters) = ParseCommandLine();
            switch (command)
            {
                case "CD":
                    ExecuteChangeDirectory(parameters);
                    break;

                case "DIR":
                    ExecuteDirectory(parameters);
                    break;

                case "LOAD":
                    ExecuteLoad(parameters);
                    break;

                case "RUN":
                    ExecuteRun(parameters);
                    break;

                case "EXIT":
                    Environment.Exit(0);
                    break;

                default:
                    Console.WriteLine("Unknown command!");
                    break;
            }
        }

        (string, string[]) ParseCommandLine()
        {
            if (inputLine[0] != CommandCharPrefix)
                throw new InvalidOperationException();

            var parameters = new List<string>();
//            var inputLineSpan = inputLine.AsSpan();

            int endCommandIndex = inputLine.IndexOf(' ');
            if (endCommandIndex == -1)
                endCommandIndex = inputLine.Length;

            string command = string.Create(endCommandIndex - 1, inputLine,
                (newString, copyString) => { for (int i = 0; i < newString.Length; i += 1) { newString[i] = Char.ToUpper(copyString[i + 1]); }
                });

//            parameters.Add(command);

            int index = endCommandIndex + 1;
            while (index < inputLine.Length)
            {
                index = SkipWhitespace(index);
                if (index == inputLine.Length)
                    break;

                if (inputLine[index] == '\"')
                {
                    int endQuoteIndex = inputLine.IndexOf('\"', index + 1);
                    if (endQuoteIndex == -1)
                        endQuoteIndex = inputLine.Length;

                    int start = index + 1;
                    int end = endQuoteIndex;
                    parameters.Add(inputLine[start..end]);
                    index = SkipWhitespace(endQuoteIndex + 1);
                    if (index < inputLine.Length && inputLine[index] == ',')
                        index += 1;
                }
                else
                {
                    int commaIndex = inputLine.IndexOf(',', index);
                    if (commaIndex == -1)
                        commaIndex = inputLine.Length;

                    parameters.Add(inputLine[index..commaIndex]);
                    index = commaIndex + 1;
                }
            }

            return (command, parameters.ToArray());
        }
        int SkipWhitespace(int index)
        {
            try
            {
                while (inputLine[index] == ' ')
                    index += 1;
            }
            catch (IndexOutOfRangeException)
            {

            }

            return index;
        }

        void ExecuteDirectory(string[] parameters)
        {
            string defaultProgramDirectory = Program.GetDefaultProgramDirectory();
            if (defaultProgramDirectory.Length == 0)
            {
                Console.WriteLine("No directory set!");
                return;
            }

            string searchPattern = parameters.Length > 0 ? parameters[0] : "*";

            var basicFiles = Directory.EnumerateFiles(defaultProgramDirectory, searchPattern);

            foreach (var basicFile in basicFiles)
                Console.WriteLine(basicFile);

            Console.WriteLine();
        }

        void ExecuteChangeDirectory(string[] parameters)
        {
            if (parameters.Length == 0)
            {
                Console.WriteLine(Program.GetDefaultProgramDirectory());
                return;
            }

            bool success = Program.SetDefaultProgramDirectory(parameters[0]);

            if (success)
                Console.WriteLine("Success!");
            else
                Console.WriteLine("Failed to change directory");
        }

        void ExecuteLoad(string[] parameters)
        {
            if (parameters.Length == 0)
            {
                Console.WriteLine("No program to load!");
                return;
            }

            interpreter.LoadFromFile(parameters[0]);
        }

        void ExecuteRun(string[] parameters)
        {
            int curArg = 0;

            if (parameters.Length > 0)
            {
                interpreter.LoadFromFile(parameters[0]);
                curArg += 1;
            }

            // Look for a line number to start interpreting.
            if (parameters.Length > curArg)
            {
                int lineNumber = int.Parse(parameters[curArg]);
                interpreter.Run(lineNumber);
                return;
            }

            interpreter.Run();
        }
    }
}
