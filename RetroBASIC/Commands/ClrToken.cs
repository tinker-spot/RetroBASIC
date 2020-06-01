using System;
using System.Collections.Generic;
using System.Text;

namespace RetroBASIC.Commands
{
    public class ClrToken : CommandToken
    {
        public ClrToken() : base("CLR", CommandTokenType.Primary)
        {

        }

        public override void Execute(Interpreter interpreter, TokenMarker tokenMarker)
        {
            interpreter.VariablesEnvironment.Clear();
        }
    }
}
