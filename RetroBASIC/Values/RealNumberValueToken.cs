using System;
using System.Collections.Generic;
using System.Text;

using RetroBASIC.Variables;

namespace RetroBASIC.Values
{
    public class RealValueToken : NumericValueToken
    {
        public RealValueToken(double value = 0) : base("RealNumberValue", VariableValueType.RealNumber)
        {
            Value = value;
        }

        public double Value { get; }

        public override double RealValue => Value;

        public override int IntValue => (int)Math.Floor(Value);
    }
}
