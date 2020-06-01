using System;
using System.Collections.Generic;
using System.Text;

namespace RetroBASIC.Commands
{
    public class RemToken : CommandToken
    {
        public RemToken() : base("REM", CommandTokenType.Comment)
        {

        }

        public override void Execute(Interpreter interpreter, TokenMarker tokenMarker)
        {
            /* The REM command is just for comments. */
        }
    }
}
