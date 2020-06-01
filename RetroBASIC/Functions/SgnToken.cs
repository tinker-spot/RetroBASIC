using System;
using System.Collections.Generic;
using System.Text;
using RetroBASIC.Values;

namespace RetroBASIC.Functions
{
    public class SgnToken : FunctionToken
    {
        public SgnToken() : base("SGN", FunctionArgumentFlags.Number | FunctionArgumentFlags.Required)
        {

        }

        public override ValueToken Evaluate(Interpreter interpreter, ValueToken[] valueTokens)
        {
            var number = (((NumericValueToken)valueTokens[0])).RealValue;

            int result = Math.Sign(number);

            return interpreter.TokensProvider.CreateIntegerValueToken(result);
        }
    }
}
