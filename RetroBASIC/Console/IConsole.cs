using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace RetroBASIC.Console
{
    public interface IConsole
    {
        TextWriter OutputTextWriter { get; }
        TextReader InputTextReader { get; }

        string ReadLine();
        char? ReadChar();

        int CursorColumn { get; set; }

        void Tab(int tab);
        void Spc(int spaces);

    }
}
