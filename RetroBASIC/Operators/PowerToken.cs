using System;
using System.Collections.Generic;
using System.Text;

using RetroBASIC.Values;

namespace RetroBASIC.Operators
{
    public class PowerToken : OperatorToken
    {
        public PowerToken() : base("^", OperatorTokenType.Power, OperatorPrecedenceLevel.Power)
        {

        }

        public override ValueToken Evaluate(Interpreter interpreter, ValueToken item1, ValueToken item2)
        {
            if (item1 is StringValueToken)
                throw new Exceptions.TypeMismatchException();

            var number1Token = (NumericValueToken)item1;
            var number2Token = (NumericValueToken)item2;

            var result = Math.Pow(number1Token.RealValue, number2Token.RealValue);

            return new RealValueToken(result);
        }
    }
}
