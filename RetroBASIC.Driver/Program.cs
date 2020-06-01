using System;
using RetroBASIC;

namespace RetroBASICDriver
{
    class Program
    {
        static void Main(string[] args)
        {
            Interpreter interpreter = new Interpreter(new RetroBASIC.Console.ScreenConsole());

            if (args.Length == 1)
            {
                interpreter.LoadFromFile(args[0]);
                interpreter.Run();
                return;
            }

            bool keepRunning = true;

            Console.CancelKeyPress += (object sender, ConsoleCancelEventArgs e) => { e.Cancel = true; interpreter.BreakDetected(); };

            Console.WriteLine(Messages.initPrompt);

            while (keepRunning)
            {
                Console.WriteLine();
                Console.WriteLine(Messages.readyPrompt);
                string inputLine = Console.ReadLine();
                if (inputLine == null)
                {
                    keepRunning = false;
                    break;
                }

                if (inputLine.Length > 80)
                {
                    Console.WriteLine("? LINE TOO LONG ERROR");
                    continue;
                }

                interpreter.EnterLine(inputLine);
            }

//            interpreter.EnterLine("10 A=5");
//            interpreter.EnterLine("GOTO 10:PRINT A");
//            interpreter.Run();
        }

        private static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            throw new NotImplementedException();
        }

        public static void CancelKey (object sender, ConsoleCancelEventArgs e)
        {
            Console.WriteLine("Interrupted!");
            e.Cancel = true;
        }
    }
}
