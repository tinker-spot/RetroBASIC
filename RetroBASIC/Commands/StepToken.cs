using System;
using System.Collections.Generic;
using System.Text;

namespace RetroBASIC.Commands
{
    public class StepToken : CommandToken
    {
        public StepToken() : base("STEP", CommandTokenType.Secondary)
        {

        }

        public override void Execute(Interpreter interpreter, TokenMarker tokenMarker)
        {
            // If used as a primary command, throw a syntax error.
            throw new Exceptions.SyntaxErrorException();
        }
    }
}
