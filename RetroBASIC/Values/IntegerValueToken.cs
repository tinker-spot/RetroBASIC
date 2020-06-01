using System;
using System.Collections.Generic;
using System.Text;

using RetroBASIC.Variables;

namespace RetroBASIC.Values
{
    public class IntegerValueToken : NumericValueToken
    {
        public IntegerValueToken(Int16 value = 0) : base("IntegerNumberValue", VariableValueType.IntegerNumber)
        {
            Value = value;
        }

        public Int16 Value { get; }

        public override double RealValue => Value;

        public override int IntValue => Value;

        public static void CheckValueBounds(double value)
        {
            if (value < Int16.MinValue || value > Int16.MaxValue)
                throw new Exceptions.IllegalQuantityException();
        }
    }
}
