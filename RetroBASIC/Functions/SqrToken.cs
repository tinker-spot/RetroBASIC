using System;
using System.Collections.Generic;
using System.Text;
using RetroBASIC.Values;

namespace RetroBASIC.Functions
{
    public class SqrToken : FunctionToken
    {
        public SqrToken() : base("SQR", FunctionArgumentFlags.Number | FunctionArgumentFlags.Required)
        {

        }

        public override ValueToken Evaluate(Interpreter interpreter, ValueToken[] valueTokens)
        {
            var value1Token = (NumericValueToken)(valueTokens[0]);
            var result = (float)Math.Sqrt(value1Token.RealValue);
            return interpreter.TokensProvider.CreateRealValueToken(result);
        }
    }
}
