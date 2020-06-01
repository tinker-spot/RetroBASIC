using System;
using System.Collections.Generic;
using System.Text;
using RetroBASIC.Values;

namespace RetroBASIC.Functions
{
    public class MidDollarToken : FunctionToken
    {
        public MidDollarToken()
            : base("MID$", FunctionArgumentFlags.String | FunctionArgumentFlags.Required,
                  FunctionArgumentFlags.Number | FunctionArgumentFlags.Required,
                  FunctionArgumentFlags.Number | FunctionArgumentFlags.Optional)
        {

        }

        public override ValueToken Evaluate(Interpreter interpreter, ValueToken[] valueTokens)
        {
            var stringValueToken = (StringValueToken)(valueTokens[0]);
            var startValue = ((NumericValueToken)(valueTokens[1])).IntValue;
            int lengthValue = (valueTokens.Length == 3) ? ((NumericValueToken)(valueTokens[2])).IntValue : stringValueToken.Value.Length;

            // Strings can't be over 255 characters long so any values outside of that range are rejected.
            if (startValue <= 0 || startValue > 255)
                throw new Exceptions.IllegalQuantityException();

            if (lengthValue < 0 || lengthValue > 255)
                throw new Exceptions.IllegalQuantityException();

            // Short circuit if the length is zero or the start value is too big.
            if (lengthValue == 0 || startValue > stringValueToken.Value.Length)
                return interpreter.TokensProvider.CreateStringValueToken(string.Empty);

            startValue -= 1;

            // Possibly adjust the length so Substring doesn't throw an exception.
            if (lengthValue > (stringValueToken.Value.Length - startValue))
                lengthValue = (stringValueToken.Value.Length - startValue);

            var result = stringValueToken.Value.Substring(startValue, lengthValue);
            return interpreter.TokensProvider.CreateStringValueToken(result);
        }
    }
}
