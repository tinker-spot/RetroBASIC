using System;
using System.Collections.Generic;
using System.Text;

namespace RetroBASIC.Commands
{
    public class NewToken : CommandToken
    {
        public NewToken() : base("NEW", CommandTokenType.Primary)
        {

        }

        public override void Execute(Interpreter interpreter, TokenMarker tokenMarker)
        {
            if (tokenMarker.Token != null)
                throw new Exceptions.SyntaxErrorException();

            interpreter.ClearAll();
        }
    }
}
