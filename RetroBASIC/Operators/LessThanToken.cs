using System;
using System.Collections.Generic;
using System.Text;

using RetroBASIC.Values;

namespace RetroBASIC.Operators
{
    public class LessThanToken : OperatorToken
    {
        public LessThanToken() : base("<", OperatorTokenType.LesserThan, OperatorPrecedenceLevel.Comparison)
        {

        }

        public override ValueToken Evaluate(Interpreter interpreter, ValueToken item1, ValueToken item2)
        {
            int result;

            if (item1 is NumericValueToken numericToken1 && item2 is NumericValueToken numericToken2)
            {
                result = (numericToken1.RealValue < numericToken2.RealValue) ? Constants.TRUE : Constants.FALSE;
            }
            else
            {
                var stringToken1 = (StringValueToken)item1;
                var stringToken2 = (StringValueToken)item2;

                int compareResult = StringComparer.OrdinalIgnoreCase.Compare(stringToken1.Value, stringToken2.Value);

                result = (compareResult < 0) ? Constants.TRUE : Constants.FALSE;
            }
            return interpreter.TokensProvider.CreateIntegerValueToken(result);
        }
    }
}
