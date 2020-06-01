using System;
using System.Collections.Generic;
using System.Text;

namespace RetroBASIC
{
    public class StatementMarker
    {
        public StatementMarker(SortedList<int, Line> _lines, int lineIndex = -1, int statementIndex = 0)
        {
            lines = _lines;
            if (lineIndex == -1)
            {
                Valid = false;
                return;
            }
            MoveTo(lineIndex, statementIndex);
        }

        public StatementMarker(StatementMarker other)
        {
            this.MoveTo(other);
        }

        private SortedList<int, Line> lines;

        public int LineIndex { get; private set; }

        public Line Line { get; private set; }

        public int LineNumber { get; private set; }

        public int StatementIndex { get; private set; }

        public Statement Statement { get; private set; }

        public bool Valid { get; private set; }

        bool SetFromIndexes()
        {
            Valid = false;
            if (LineIndex >= lines.Count)
                return false;

            IList<Line> listValues = (IList<Line>)(lines.Values);

            Line = listValues[LineIndex];
            LineNumber = Line.LineNumber;

            if (StatementIndex >= Line.Statements.Count)
                return false;

            Statement = Line.Statements[StatementIndex];
            Valid = true;
            return true;
        }
        public bool MoveTo(int lineIndex = 0, int statementIndex = 0)
        {
            LineIndex = lineIndex;
            StatementIndex = statementIndex;
            return SetFromIndexes();
        }

        public void MoveTo(StatementMarker other)
        {
            this.lines = other.lines;
            this.LineIndex = other.LineIndex;
            this.Line = other.Line;
            this.LineNumber = other.LineNumber;
            this.StatementIndex = other.StatementIndex;
            this.Statement = other.Statement;
            this.Valid = other.Valid;
        }

        public bool MoveToNextStatement()
        {
            var statementIndex = StatementIndex + 1;
            if (statementIndex < Line.Statements.Count)
            {
                StatementIndex = statementIndex;
                return SetFromIndexes();
            }

            StatementIndex = 0;
            LineIndex += 1;

            if (LineIndex < lines.Count)
            {
                return SetFromIndexes();
            }

            Valid = false;
            return false;
        }

        public bool MoveToNextLine()
        {
            LineIndex += 1;
            StatementIndex = 0;

            return SetFromIndexes();
        }

        public bool MoveToLine(int lineNumber)
        {
            var lineIndex = lines.IndexOfKey(lineNumber);
            if (lineIndex == -1)
            {
                Valid = false;
                return false;
            }

            return MoveTo(lineIndex);
        }
    }
}
