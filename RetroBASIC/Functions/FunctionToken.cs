using System;
using System.Collections.Generic;
using System.Text;

using RetroBASIC.Values;

namespace RetroBASIC.Functions
{
    [Flags]
    public enum FunctionArgumentFlags
    {
        String = 1,
        Number = 2,
        Required = 16,
        Optional = 32
    }

    public class FunctionToken : Token
    {
        public FunctionToken(string functionName, params FunctionArgumentFlags[] funcArguments)
            : base(functionName, TokenType.Function)
        {
            RequiredArgsCount = 0;
            OptionalArgsCount = 0;

            foreach (FunctionArgumentFlags arg in funcArguments)
            {
                ArgumentFlags = funcArguments;

                if ((arg & FunctionArgumentFlags.Optional) == FunctionArgumentFlags.Optional)
                {
                    OptionalArgsCount += 1;
                }
                else
                {
                    RequiredArgsCount += 1;
                }
            }
        }

        public int RequiredArgsCount { get; }
        public int OptionalArgsCount { get; }
        public FunctionArgumentFlags[] ArgumentFlags { get; }

        public virtual ValueToken Evaluate(RetroBASIC.Interpreter interpreter, ValueToken[] valueTokens)
        {
            throw new NotSupportedException();
        }
    }
}
