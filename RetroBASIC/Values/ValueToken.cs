using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using RetroBASIC.Variables;

namespace RetroBASIC.Values
{
    public class ValueToken : Token
    {
        public ValueToken(string name, VariableValueType valueType) : base(name, TokenType.Value)
        {
            ValueType = valueType;
        }

//        public T Value { get; protected set; }
        public VariableValueType ValueType { get; }
    }
}
