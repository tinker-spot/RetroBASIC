using System;
using System.Collections.Generic;
using System.Text;
using RetroBASIC.Values;

namespace RetroBASIC.Operators
{
    public class MinusToken : OperatorToken
    {
        public MinusToken() : base("-", OperatorTokenType.Minus, OperatorPrecedenceLevel.AddSub)
        {

        }

        public override ValueToken Evaluate(Interpreter interpreter, ValueToken item1, ValueToken item2)
        {
            if (item1 is StringValueToken)
                throw new Exceptions.TypeMismatchException();

            var number1Token = (NumericValueToken)item1;
            var number2Token = (NumericValueToken)item2;

            return interpreter.TokensProvider.CreateRealValueToken(number1Token.RealValue - number2Token.RealValue);
        }

    }
}
