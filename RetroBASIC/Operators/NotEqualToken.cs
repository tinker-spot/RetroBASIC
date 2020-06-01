using System;
using System.Collections.Generic;
using System.Text;
using RetroBASIC.Values;

namespace RetroBASIC.Operators
{
    public class NotEqualToken : OperatorToken
    {
        public NotEqualToken() : base("<>", OperatorTokenType.NotEqual, OperatorPrecedenceLevel.Comparison)
        {

        }

        public override ValueToken Evaluate(Interpreter interpreter, ValueToken item1, ValueToken item2)
        {
            int result;

            if (item1 is StringValueToken stringToken1 && item2 is StringValueToken stringToken2)
            {
                int compareResult = StringComparer.OrdinalIgnoreCase.Compare(stringToken1.Value, stringToken2.Value);

                result = (compareResult != 0) ? Constants.TRUE : Constants.FALSE;
            }
            else
            {
                var numberToken1 = (NumericValueToken)item1;
                var numberToken2 = (NumericValueToken)item2;

                result = (numberToken1.RealValue != numberToken2.RealValue) ? Constants.TRUE : Constants.FALSE;
            }

            return new RealValueToken(result);
        }
    }
}
