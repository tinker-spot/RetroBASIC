using System;
using System.Collections.Generic;
using System.Text;

using RetroBASIC.Values;

namespace RetroBASIC.Commands
{
    public class GosubToken : CommandToken
    {
        public GosubToken() : base("GOSUB", CommandTokenType.Primary)
        {

        }

        public override void Execute(Interpreter interpreter, TokenMarker tokenMarker)
        {
            if (!(tokenMarker.Token is NumericValueToken lineNumberToken))
                throw new Exceptions.UndefinedStatementException();

            var currentMarker = interpreter.CopyCurrentStatementMarker();
            currentMarker.MoveToNextStatement();

            interpreter.GosubMarkers.Push(currentMarker);

            var gotoMarker = interpreter.CreateStatementMarker();
            if (gotoMarker.MoveToLine(lineNumberToken.IntValue) == false)
                throw new Exceptions.UndefinedStatementException();

            interpreter.NextStatementMarker = gotoMarker;
        }
    }
}
