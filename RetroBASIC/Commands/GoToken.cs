using System;
using System.Collections.Generic;
using System.Text;

using RetroBASIC.Values;

namespace RetroBASIC.Commands
{
    public class GoToken : CommandToken
    {
        public GoToken() : base("GO", CommandTokenType.Primary)
        {

        }

        public override void Execute(Interpreter interpreter, TokenMarker tokenMarker)
        {
            if (!(tokenMarker.Token is ToToken))
                throw new Exceptions.SyntaxErrorException();

            if (!(tokenMarker.GetNextToken() is NumericValueToken numberToken))
                throw new Exceptions.SyntaxErrorException();

            var gotoMarker = interpreter.CreateStatementMarker();
            if (gotoMarker.MoveToLine(numberToken.IntValue) == false)
                throw new Exceptions.UndefinedStatementException();

            interpreter.NextStatementMarker = gotoMarker;

        }
    }
}
