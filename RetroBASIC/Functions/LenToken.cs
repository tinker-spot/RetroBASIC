using System;
using System.Collections.Generic;
using System.Text;

using RetroBASIC.Values;
using RetroBASIC.Variables;

namespace RetroBASIC.Functions
{
    public class LenToken : FunctionToken
    {
        public LenToken() : base("LEN", FunctionArgumentFlags.String | FunctionArgumentFlags.Required)
        {

        }

        public override ValueToken Evaluate(Interpreter interpreter, ValueToken[] valueTokens)
        {
            var stringValueToken = (StringValueToken)(valueTokens[0]);

            return interpreter.TokensProvider.CreateIntegerValueToken(stringValueToken.Value.Length);
        }
    }
}
