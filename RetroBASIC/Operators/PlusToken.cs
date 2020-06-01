using System;
using System.Collections.Generic;
using System.Text;

using RetroBASIC.Values;

namespace RetroBASIC.Operators
{
    public class PlusToken : OperatorToken
    {
        public PlusToken() : base("+", OperatorTokenType.Plus, OperatorPrecedenceLevel.AddSub)
        {

        }


        public override ValueToken Evaluate(RetroBASIC.Interpreter interpreter, ValueToken item1, ValueToken item2)
        {
            StringValueToken string1Token = item1 as StringValueToken;
            StringValueToken string2Token = item2 as StringValueToken;

            if (string1Token != null && string2Token != null)
            {
                var newString = string1Token.Value + string2Token.Value;
                if (newString.Length >= 256)
                {
                    throw new Exceptions.StringTooLongException();
                }

                return interpreter.TokensProvider.CreateStringValueToken(newString);
            }

            var number1Token = item1 as NumericValueToken;
            var number2Token = item2 as NumericValueToken;

            if (number1Token == null || number2Token == null)
                throw new Exceptions.TypeMismatchException();

            var newNumber = number1Token.RealValue + number2Token.RealValue;
            return interpreter.TokensProvider.CreateRealValueToken(newNumber);
        }

    }
}
