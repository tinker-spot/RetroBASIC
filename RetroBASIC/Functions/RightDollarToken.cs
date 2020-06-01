using System;
using System.Collections.Generic;
using System.Text;
using RetroBASIC.Values;

namespace RetroBASIC.Functions
{
    public class RightDollarToken : FunctionToken
    {
        public RightDollarToken() : 
            base("RIGHT$", FunctionArgumentFlags.String | FunctionArgumentFlags.Required,
                FunctionArgumentFlags.Number | FunctionArgumentFlags.Required)
        {

        }

        public override ValueToken Evaluate(Interpreter interpreter, ValueToken[] valueTokens)
        {
            var stringValue = ((StringValueToken)(valueTokens[0])).Value;
            var amount = ((NumericValueToken)(valueTokens[1])).IntValue;

            // Strings can't be over 255 characters long. Anything else is rejected.
            if (amount < 0 || amount > 255)
                throw new Exceptions.IllegalQuantityException();

            // Short circuit in case of a zero length amount
            if (amount == 0)
                return interpreter.TokensProvider.CreateStringValueToken(string.Empty);

            // Possibly trim so Substring doesn't throw an exception.
            if (amount > stringValue.Length)
                amount = stringValue.Length;

            return interpreter.TokensProvider.CreateStringValueToken(stringValue.Substring(stringValue.Length - amount, amount));
        }
    }
}
