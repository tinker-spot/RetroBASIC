using System;
using System.Collections.Generic;
using System.Text;

using RetroBASIC.Values;
using RetroBASIC.Variables;

namespace RetroBASIC.Commands
{
    public class SaveToken : CommandToken
    {
        public SaveToken() : base("SAVE", CommandTokenType.Comment)
        {

        }

        public override void Execute(Interpreter interpreter, TokenMarker tokenMarker)
        {
            if (!(tokenMarker.Token is StringValueToken stringValueToken))
                throw new Exceptions.SyntaxErrorException();

        }
    }
}
