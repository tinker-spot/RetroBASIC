using System;
using System.Collections.Generic;
using System.Text;

using RetroBASIC.Values;
using RetroBASIC.Variables;
using RetroBASIC.Characters;

namespace RetroBASIC.Commands
{
    public class DimToken : CommandToken
    {
        public DimToken() : base("DIM", CommandTokenType.Primary)
        {

        }

        public override void Execute(Interpreter interpreter, TokenMarker tokenMarker)
        {
            while (tokenMarker.Token != null)
            {
                if (!(tokenMarker.Token is VariableNameToken variableNameToken))
                    throw new Exceptions.SyntaxErrorException();

                if (!(tokenMarker.PeekNext() is OpenParenToken))
                    throw new Exceptions.SyntaxErrorException();

                var indicies = interpreter.ExpressionEvaluator.EvaluateVariableArrayAssignment(tokenMarker);
                interpreter.VariablesEnvironment.DimensionArrayVariable(variableNameToken, indicies);

                if (!(tokenMarker.Token is CommaToken))
                    break;

                tokenMarker.MoveNext();
            }
        }
    }
}
