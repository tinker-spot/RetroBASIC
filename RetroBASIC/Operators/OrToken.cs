﻿using System;
using System.Collections.Generic;
using System.Text;
using RetroBASIC.Values;

namespace RetroBASIC.Operators
{
    public class OrToken : OperatorToken
    {
        public OrToken() : base("OR", OperatorTokenType.Or, OperatorPrecedenceLevel.LogicalOr)
        {

        }
        public override ValueToken Evaluate(Interpreter interpreter, ValueToken item1, ValueToken item2)
        {
            if (item1 is StringValueToken)
                throw new Exceptions.TypeMismatchException();

            var numberToken1 = (NumericValueToken)item1;
            var numberToken2 = (NumericValueToken)item2;

            var result = numberToken1.IntValue | numberToken2.IntValue;

            return new RealValueToken(result);
        }
    }
}
