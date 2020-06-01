using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RetroBASIC.Commands
{
    public class PeekToken : CommandToken
    {
        public PeekToken() : base("PEEK", CommandTokenType.Primary)
        {

        }

        public override void Execute(Interpreter interpreter, TokenMarker tokenMarker)
        {
            throw new Exceptions.CommandNotSupportedException();
        }
    }
}
