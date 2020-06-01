using System;
using System.Collections.Generic;
using System.Text;

using RetroBASIC.Variables;

namespace RetroBASIC.Values
{
    public abstract class NumericValueToken : ValueToken
    {
        public NumericValueToken(string tokenName, VariableValueType numericType) : base(tokenName, numericType)
        {

        }

        public abstract double RealValue { get; }

        public abstract int IntValue { get; }
    }
}
