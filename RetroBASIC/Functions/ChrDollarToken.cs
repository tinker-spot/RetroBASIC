using System;
using System.Collections.Generic;
using System.Text;

using RetroBASIC.Values;

namespace RetroBASIC.Functions
{
    public class ChrDollar : FunctionToken
    {
        public ChrDollar() : base("CHR$", FunctionArgumentFlags.Number | FunctionArgumentFlags.Required)
        {

        }

        public override ValueToken Evaluate(Interpreter interpreter, ValueToken[] valueTokens)
        {
            var asciiValueToken = (NumericValueToken)(valueTokens[0]);

            // Accept only values that can be represented by ASCII
            if (asciiValueToken.IntValue < 0 || asciiValueToken.IntValue > 255)
                throw new Exceptions.IllegalQuantityException();

            char ch = (char)asciiValueToken.IntValue;
            return interpreter.TokensProvider.CreateStringValueToken(ch.ToString());
        }
    }
}
