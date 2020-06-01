using System;
using System.Collections.Generic;
using System.Text;
using RetroBASIC.Values;

namespace RetroBASIC.Functions
{
    public class PosToken : FunctionToken
    {
        public PosToken() : base("POS", FunctionArgumentFlags.Number | FunctionArgumentFlags.Required)
        {

        }

        public override ValueToken Evaluate(Interpreter interpreter, ValueToken[] valueTokens)
        {
            var columnPos = interpreter.Console.CursorColumn;

            return interpreter.TokensProvider.CreateIntegerValueToken(columnPos);
        }
    }
}
