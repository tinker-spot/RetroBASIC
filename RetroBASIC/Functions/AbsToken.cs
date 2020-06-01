using System;
using System.Collections.Generic;
using System.Text;

using RetroBASIC.Values;
using RetroBASIC.Variables;

namespace RetroBASIC.Functions
{
    public class AbsToken : FunctionToken
    {
        public AbsToken() : base("ABS", FunctionArgumentFlags.Number | FunctionArgumentFlags.Required)
        {

        }

        public override ValueToken Evaluate(Interpreter interpreter, ValueToken[] valueTokens)
        {

            var numericToken = (NumericValueToken)(valueTokens[0]);

            return interpreter.TokensProvider.CreateIntegerValueToken(Math.Abs(numericToken.IntValue));
        }
    }
}
