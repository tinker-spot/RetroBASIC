using System;
using System.Collections.Generic;
using System.Text;
using RetroBASIC.Values;

namespace RetroBASIC.Operators
{
    public class UnaryMinusToken : OperatorToken
    {
        public UnaryMinusToken() : base("-", OperatorTokenType.Unary, OperatorPrecedenceLevel.Unary)
        {

        }

        public override ValueToken Evaluate(Interpreter interpreter, ValueToken item1, ValueToken item2)
        {
            if (item1 is IntegerValueToken value)
            {
                return interpreter.TokensProvider.CreateIntegerValueToken(-value.Value);
            }

            if (item1 is RealValueToken realValue)
            {
                return interpreter.TokensProvider.CreateRealValueToken(-realValue.Value);
            }

            throw new Exceptions.TypeMismatchException();
        }
    }
}
