using System;
using System.Collections.Generic;
using System.Text;
using RetroBASIC.Values;

namespace RetroBASIC.Functions
{
    public class AtnToken : FunctionToken
    {
        public AtnToken() : base("ATN", FunctionArgumentFlags.Number | FunctionArgumentFlags.Required)
        {

        }

        public override ValueToken Evaluate(Interpreter interpreter, ValueToken[] valueTokens)
        {
            var numericValueToken = (NumericValueToken)(valueTokens[0]);

            var result = Math.Atan(numericValueToken.RealValue);
            return interpreter.TokensProvider.CreateRealValueToken((float)result);
        }
    }
}
