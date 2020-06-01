using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using RetroBASIC.Variables;

namespace RetroBASIC.Values
{
    public class ValueTokenArray : ValueToken
    {
        public ValueTokenArray(ValueToken[] _values) : base("ValueTokenArray", VariableValueType.Array)
        {
            Values = _values;
        }

        public ValueToken[] Values { get; }

        public override void DumpTokenContents(TextWriter tw)
        {
            tw.Write("Value Token Array: ");
            foreach (var value in Values)
            {
                value.DumpTokenContents(tw);
            }
        }
    }
}
