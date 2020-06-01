using System;
using System.Collections.Generic;
using System.Text;

namespace RetroBASIC.Commands
{
    public class EndToken : CommandToken
    {
        public EndToken() : base("END", CommandTokenType.Secondary)
        {

        }

        public override void Execute(Interpreter interpreter, TokenMarker tokenMarker)
        {
            interpreter.ContinueRunning = false;
        }
    }
}
