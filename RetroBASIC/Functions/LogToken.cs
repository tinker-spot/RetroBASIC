using System;
using System.Collections.Generic;
using System.Text;
using RetroBASIC.Values;

namespace RetroBASIC.Functions
{
    public class LogToken : FunctionToken
    {
        public LogToken() : base("LOG", FunctionArgumentFlags.Number | FunctionArgumentFlags.Required)
        {

        }

        public override ValueToken Evaluate(Interpreter interpreter, ValueToken[] valueTokens)
        {
            var numberValueToken = (NumericValueToken)(valueTokens[0]);

            var result = Math.Log(numberValueToken.RealValue);

            return interpreter.TokensProvider.CreateRealValueToken((float)result);
        }
    }
}
