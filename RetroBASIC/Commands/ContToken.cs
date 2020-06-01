using System;
using System.Collections.Generic;
using System.Text;

namespace RetroBASIC.Commands
{
    public class ContToken : CommandToken
    {
        public ContToken() : base("CONT", CommandTokenType.Primary)
        {

        }

        public override void Execute(Interpreter interpreter, TokenMarker tokenMarker)
        {
            if (tokenMarker.Token != null)
                throw new Exceptions.SyntaxErrorException();

            if (interpreter.CurrentStatementMarker != null && interpreter.CurrentStatementMarker.Valid == false)
                throw new Exceptions.CantContinueException();

            interpreter.ContinueRun();
        }
    }
}
