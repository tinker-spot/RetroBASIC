using System;
using System.Collections.Generic;
using System.Text;
using RetroBASIC.Values;

namespace RetroBASIC.Functions
{
    public class StrDollarToken : FunctionToken
    {
        public StrDollarToken() : base("STR$", FunctionArgumentFlags.Number | FunctionArgumentFlags.Required)
        {

        }

        public override ValueToken Evaluate(Interpreter interpreter, ValueToken[] valueTokens)
        {
            var number = ((NumericValueToken)(valueTokens[0])).RealValue;

            StringBuilder sb = new StringBuilder();

            // Add a space when number number is zero or positive 
            if (number >= 0)
                sb.Append(" ");
            sb.Append(number);

            var result = sb.ToString();
            return interpreter.TokensProvider.CreateStringValueToken(result);
        }
    }
}
