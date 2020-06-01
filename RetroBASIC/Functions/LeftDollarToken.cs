using System;
using System.Collections.Generic;
using System.Text;

using RetroBASIC.Values;

namespace RetroBASIC.Functions
{
    public class LeftDollar : FunctionToken
    {
        public LeftDollar()
            : base("LEFT$", FunctionArgumentFlags.String | FunctionArgumentFlags.Required,
                  FunctionArgumentFlags.Number | FunctionArgumentFlags.Required)
        {

        }

        public override ValueToken Evaluate(Interpreter interpreter, ValueToken[] valueTokens)
        {
            var stringValueToken = (StringValueToken)(valueTokens[0]);
            var sliceAmountToken = (NumericValueToken)(valueTokens[1]);

            if (sliceAmountToken.IntValue < 0 || sliceAmountToken.IntValue > 255)
                throw new Exceptions.IllegalQuantityException();

            // Determine how much of the string to get, not to exceed the length of the string.
            var sliceAmount = (sliceAmountToken.IntValue > stringValueToken.Value.Length) ? sliceAmountToken.IntValue : stringValueToken.Value.Length;
           
            if (sliceAmount == 0)
            {
                return interpreter.TokensProvider.CreateStringValueToken(string.Empty);
            }

            var result = stringValueToken.Value.Substring(0, sliceAmountToken.IntValue);
            return interpreter.TokensProvider.CreateStringValueToken(result);
        }
    }
}
