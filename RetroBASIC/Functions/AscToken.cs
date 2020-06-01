using System;
using System.Collections.Generic;
using System.Text;

using RetroBASIC.Values;
using RetroBASIC.Variables;

namespace RetroBASIC.Functions
{
    public class AscToken : FunctionToken
    {
        public AscToken() : base("ASC", FunctionArgumentFlags.String | FunctionArgumentFlags.Required)
        {

        }

        public override ValueToken Evaluate(Interpreter interpreter, ValueToken[] valueTokens)
        {
            if (!(valueTokens[0] is StringValueToken stringValueToken))
                throw new Exceptions.TypeMismatchException();

            if (stringValueToken.Value.Length == 0)
                throw new Exceptions.IllegalQuantityException();

            char firstChar = stringValueToken.Value[0];
            return interpreter.TokensProvider.CreateRealValueToken((float)(firstChar));
        }
    }
}
