using System;
using System.Collections.Generic;
using System.Text;
using RetroBASIC.Values;

namespace RetroBASIC.Functions
{
    public class CosToken : FunctionToken
    {
        public CosToken() : base("COS", FunctionArgumentFlags.Number | FunctionArgumentFlags.Required)
        {

        }

        public override ValueToken Evaluate(Interpreter interpreter, ValueToken[] valueTokens)
        {
            var realValueToken = (NumericValueToken)(valueTokens[0]);
            var result = Math.Cos(realValueToken.RealValue);

            return interpreter.TokensProvider.CreateRealValueToken((float)result);
        }
    }
}
