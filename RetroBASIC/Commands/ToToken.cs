using System;
using System.Collections.Generic;
using System.Text;

namespace RetroBASIC.Commands
{
    public class ToToken : CommandToken
    {
        public ToToken() : base("TO", CommandTokenType.Secondary)
        {

        }

        public override void Execute(Interpreter interpreter, TokenMarker tokenMarker)
        {
            // Throw a syntax error if used as a primary command
            throw new Exceptions.SyntaxErrorException();
        }
    }
}
