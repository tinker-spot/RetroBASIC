using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using RetroBASIC.Variables;

namespace RetroBASIC.Values
{
    public class StringValueToken : ValueToken
    {
        public StringValueToken(string value = null) : base("StringConstantValueToken", VariableValueType.String)
        {
            if (value == null)
            {
                value = string.Empty;
            }
            Value = value;
        }

        public string Value { get; }

        public override void DumpTokenContents(TextWriter tw)
        {
            tw.Write("String Const:\"");
            tw.Write(Value);
            tw.Write("\"");
        }

        public static string GetStringValue(Token token)
        {        
            if (token.TokenType != TokenType.Value)
                throw new Exceptions.TypeMismatchException();

            var stringToken = token as StringValueToken;

            if (stringToken == null)
                throw new Exceptions.TypeMismatchException();

            return stringToken.Value;
        }
    }
}
