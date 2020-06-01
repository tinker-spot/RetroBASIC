using System;
using System.Collections.Generic;
using System.Text;

namespace RetroBASIC.Commands
{
    public class RestoreToken : CommandToken
    {
        public RestoreToken() : base("RESTORE", CommandTokenType.Primary)
        {

        }

        public override void Execute(Interpreter interpreter, TokenMarker tokenMarker)
        {
            interpreter.DataStatementMarker.MoveTo(0);
        }
    }
}
