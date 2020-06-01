using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using RetroBASIC.Values;

namespace RetroBASIC.Functions
{
    public class SinToken : FunctionToken
    {
        public SinToken() : base("SIN", FunctionArgumentFlags.Number | FunctionArgumentFlags.Required)
        {

        }

        public override ValueToken Evaluate(Interpreter interpreter, ValueToken[] valueTokens)
        {
            var inputValue = ((NumericValueToken)(valueTokens[0])).RealValue;

            return interpreter.TokensProvider.CreateRealValueToken((float)Math.Sin(inputValue));
        }
    }
}
