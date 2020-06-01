using System;
using System.Collections.Generic;
using System.Text;
using RetroBASIC.Values;

namespace RetroBASIC.Functions
{
    public class FreToken : FunctionToken
    {
        public FreToken() : base("FRE", FunctionArgumentFlags.Number | FunctionArgumentFlags.Required)
        {

        }

        public override ValueToken Evaluate(Interpreter interpreter, ValueToken[] valueTokens)
        {
            // Perhaps just return 64K free instead?
            throw new Exceptions.CommandNotSupportedException();
        }
    }
}
