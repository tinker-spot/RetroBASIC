using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace RetroBASIC.Console
{
    public class ScreenConsole : IConsole
    {
        public ScreenConsole()
        {
            OutputTextWriter = System.Console.Out;
            InputTextReader = System.Console.In;
        }

        public TextWriter OutputTextWriter { get; }

        public TextReader InputTextReader { get; }

        public int CursorColumn { get { return System.Console.CursorLeft; } set { SetCursorColumn(value); } }

        void SetCursorColumn(int column)
        {
            System.Console.SetCursorPosition(column, System.Console.CursorTop);
        }

        public void Tab(int column)
        {
            while (column >= System.Console.WindowWidth)
            {
                // Cause scrolling if needed
                System.Console.WriteLine();
                column -= System.Console.WindowWidth;
            }

            CursorColumn = column;
        }

        public void Spc(int spaces)
        {
            CursorColumn = CursorColumn + spaces;
        }

        public string ReadLine()
        {
            return System.Console.ReadLine();
        }

        public char? ReadChar()
        {
            if (System.Console.KeyAvailable == false)
                return null;

            var chInfo = System.Console.ReadKey(true);
            return chInfo.KeyChar;            
        }
    }
}
