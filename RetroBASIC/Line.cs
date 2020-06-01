using System;
using System.Collections.Generic;
using System.Text;

namespace RetroBASIC
{
//    using Statement = List<Token>;

    public class Line
    {
        public Line(int line, List<Statement> statements)
        {
            LineNumber = line;
            Statements = statements;
        }
        public int LineNumber { get; }
        public IReadOnlyList<Statement> Statements { get; }
    }
}
