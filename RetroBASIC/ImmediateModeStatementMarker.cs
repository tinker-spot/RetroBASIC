using System;
using System.Collections.Generic;
using System.Text;

namespace RetroBASIC
{
    public class ImmediateModeStatementMarker
    {
        public ImmediateModeStatementMarker(List<Statement> _statements)
        {
            statements = _statements;
            MoveTo(0);
        }

        private List<Statement> statements;

        public bool Valid { get; private set; }

        public Statement Statement { get; private set; }

        public int StatementIndex { get; private set; }

        public void MoveTo(int _statementIndex)
        {
            if (_statementIndex >= 0 && _statementIndex < statements.Count)
            {
                StatementIndex = _statementIndex;
                Statement = statements[_statementIndex];
                Valid = true;
            }
            else
            {
                Statement = null;
                Valid = false;
            }
        }

        public bool MoveToNextStatement()
        {
            MoveTo(StatementIndex + 1);
            return Valid;
        }
    }
}
