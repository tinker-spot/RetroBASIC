using System;
using System.Collections.Generic;
using System.Text;

using RetroBASIC.Values;

namespace RetroBASIC.Operators
{
    public class DivToken : OperatorToken
    {
        public DivToken() : base("/", OperatorTokenType.Divide, OperatorPrecedenceLevel.MulDiv)
        {

        }

        public override ValueToken Evaluate(Interpreter interpreter, ValueToken item1, ValueToken item2)
        {
            if (item1 is StringValueToken)
                throw new Exceptions.TypeMismatchException();

            var number1Token = (NumericValueToken)item1;
            var number2Token = (NumericValueToken)item2;

            if (number2Token.RealValue == 0)
                throw new Exceptions.DivisionByZeroException();

            return interpreter.TokensProvider.CreateRealValueToken(number1Token.RealValue / number2Token.RealValue);
        }
    }
}
