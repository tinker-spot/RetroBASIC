using System;
using System.Collections.Generic;
using System.Text;

namespace RetroBASIC.Commands
{
    public class FnToken : CommandToken
    {
        public FnToken() : base("FN", CommandTokenType.Secondary)
        {

        }

        public override void Execute(Interpreter interpreter, TokenMarker tokenMarker)
        {
            throw new Exceptions.SyntaxErrorException();
        }
    }
}
