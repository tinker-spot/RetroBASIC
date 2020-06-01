using System;
using System.Collections.Generic;
using System.Text;

namespace RetroBASIC
{
    public class Statements
    {
        private List<Statement> statements;

        public Statements(List<Statement> statements = null)
        {
            this.statements = new List<Statement>();
            if (statements != null)
                this.statements.AddRange(statements);
        }

        public void Add(Statement statement)
        {
            this.statements.Add(statement);
        }

        public void Add(List<Token> tokens)
        {
            this.statements.Add(new Statement(tokens));
        }

        public void AddTokens(List<Token> tokens)
        {
            this.statements.Add(new Statement(tokens));
        }
        public IReadOnlyList<Statement> ProgramStatements { get { return statements; } }
    }
}
