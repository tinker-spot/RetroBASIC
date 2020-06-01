using System;
using System.Collections.Generic;
using System.Text;
using RetroBASIC.Values;

namespace RetroBASIC.Functions
{
    public class IntToken : FunctionToken
    {
        public IntToken() : base("INT", FunctionArgumentFlags.Number | FunctionArgumentFlags.Required)
        {

        }

        public override ValueToken Evaluate(Interpreter interpreter, ValueToken[] valueTokens)
        {
            var numericValue = ((NumericValueToken)(valueTokens[0])).RealValue;

            // Remove the decimals
            var result = Math.Floor(numericValue);
            return interpreter.TokensProvider.CreateRealValueToken((float)result);
        }
    }
}
