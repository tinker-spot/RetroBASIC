using System;
using System.Collections.Generic;
using System.Text;

namespace RetroBASIC.Commands
{
    public class ReturnToken : CommandToken
    {
        public ReturnToken() : base("RETURN", CommandTokenType.Primary)
        {

        }

        public override void Execute(Interpreter interpreter, TokenMarker tokenMarker)
        {
            try
            {
                var returnMarker = interpreter.GosubMarkers.Pop();
//                returnMarker.MoveToNextStatement();
                interpreter.NextStatementMarker = returnMarker;
            }
            catch (InvalidOperationException)
            {
                throw new Exceptions.ReturnWithoutGosubException();
            }
        }
    }
}
