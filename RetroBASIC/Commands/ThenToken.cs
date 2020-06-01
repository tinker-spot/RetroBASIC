using System;
using System.Collections.Generic;
using System.Text;

namespace RetroBASIC.Commands
{
    public class ThenToken : CommandToken
    {
        public ThenToken() : base("THEN", CommandTokenType.Secondary)
        {

        }

        public override void Execute(Interpreter interpreter, TokenMarker tokenMarker)
        {
            // If used as a primary command, throw a syntax error exception.
            throw new Exceptions.SyntaxErrorException();
        }
    }
}
