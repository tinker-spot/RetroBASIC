using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RetroBASIC.Commands
{
    public class DataToken : CommandToken
    {
        public DataToken() : base("DATA", CommandTokenType.Primary)
        {

        }

        public override void Execute(Interpreter interpreter, TokenMarker tokenMarker)
        {
            // Nothing to see here. Move along.
        }

    }
}
