using System;
using System.Collections.Generic;
using System.Text;

using RetroBASIC.Variables;
using RetroBASIC.Values;

namespace RetroBASIC.Commands
{
    public class RunToken : CommandToken
    {
        public RunToken() : base("RUN", CommandTokenType.Primary)
        {

        }

        public override void Execute(Interpreter interpreter, TokenMarker tokenMarker)
        {
            if (tokenMarker.Token == null)
            {
                interpreter.Run();
                return;
            }

            if (!(tokenMarker.Token is NumericValueToken numericValueToken))
                throw new Exceptions.SyntaxErrorException();

            if (tokenMarker.PeekNext() != null)
                throw new Exceptions.SyntaxErrorException();

            interpreter.Run(numericValueToken.IntValue);
        }
    }
}
