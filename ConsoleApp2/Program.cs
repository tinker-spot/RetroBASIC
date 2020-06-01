using System;
using System.Reflection;
using System.Configuration;
using System.IO;

using RetroBASIC;

namespace RetroBASICDriver
{
    class Program
    {
        static void Main(string[] args)
        {
            Interpreter interpreter = new Interpreter();
            interpreter.Console = new RetroBASIC.Console.ScreenConsole();
            interpreter.GetProgramDirectory = () => GetDefaultProgramDirectory();

            var consoleCommands = new ConsoleCommands(interpreter);

            Assembly retroBasicLib = typeof(Interpreter).Assembly;
            AssemblyName retroBasicLibName = retroBasicLib.GetName();
            Version retroBasicVersion = retroBasicLibName.Version;
            string retroBasicVersionString = retroBasicVersion.ToString();

            Console.WriteLine(string.Format(Messages.initPrompt, retroBasicVersionString));

            if (args.Length == 1)
            {
                Console.WriteLine($"Loading \"{args[0]}\"");
                Console.WriteLine("Running...");
                interpreter.LoadFromFile(args[0]);
                interpreter.Run();
            }

            bool keepRunning = true;

            Console.CancelKeyPress += (object sender, ConsoleCancelEventArgs e) => { e.Cancel = true;  interpreter.BreakDetected(); };

            while (keepRunning)
            {
                Console.WriteLine();
                Console.WriteLine(Messages.readyPrompt);

                string inputLine = Console.ReadLine();
                if (inputLine == null || inputLine.Length == 0)
                {
                    keepRunning = false;
                    break;
                }

                if (inputLine[0] == ConsoleCommands.CommandCharPrefix)
                {
                    consoleCommands.Execute(inputLine);
                    continue;
                }

                if (inputLine.Length > 255)
                {
                    Console.WriteLine("? LINE TOO LONG ERROR");
                    continue;
                }

                interpreter.EnterLine(inputLine);
            }
        }

        static public string GetDefaultProgramDirectory()
        {
            return RetroBASICConsoleSettings.Default.DefaultBASICProgramDirectory;
        }

        static public bool SetDefaultProgramDirectory(string newDirectory)
        {
            if (Directory.Exists(newDirectory))
            {
                RetroBASICConsoleSettings.Default.DefaultBASICProgramDirectory = newDirectory;
                RetroBASICConsoleSettings.Default.Save();
                return true;
            }

            return false;
        }
    }
}
