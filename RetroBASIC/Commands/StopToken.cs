using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RetroBASIC.Commands
{
    public class StopToken : CommandToken
    {
        public StopToken() : base("STOP", CommandTokenType.Primary)
        {

        }

        public override void Execute(Interpreter interpreter, TokenMarker tokenMarker)
        {
            interpreter.BreakDetected();
        }
    }
}
