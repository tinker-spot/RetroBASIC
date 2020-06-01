using System;
using System.Collections.Generic;
using System.Text;

using RetroBASIC.Values;

namespace RetroBASIC.Commands
{
    public class GotoToken : CommandToken
    {
        public GotoToken() : base("GOTO", CommandTokenType.Primary)
        {

        }

        public override void Execute(Interpreter interpreter, TokenMarker tokenMarker)
        {
            if (tokenMarker.Token == null || !(tokenMarker.Token is NumericValueToken numberToken))
                throw new Exceptions.UndefinedStatementException();

            if (interpreter.InterpreterMode == InterpreterMode.Immmediate)
            {
                interpreter.WarmRun(numberToken.IntValue);
                return;
            }

            var gotoMarker = interpreter.CreateStatementMarker();
            if (gotoMarker.MoveToLine(numberToken.IntValue) == false)
                throw new Exceptions.UndefinedStatementException();

            interpreter.NextStatementMarker = gotoMarker;
        }
    }
}
