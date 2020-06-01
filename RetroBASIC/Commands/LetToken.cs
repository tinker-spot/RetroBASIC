using System;
using System.Collections.Generic;
using System.Text;

using RetroBASIC.Variables;
using RetroBASIC.Characters;
using RetroBASIC.Values;

namespace RetroBASIC.Commands
{
    public class LetToken : CommandToken
    {
        public LetToken() : base("LET", CommandTokenType.Primary)
        {

        }

        public override void Execute(Interpreter interpreter, TokenMarker tokenMarker)
        {
            var variableNameToken = tokenMarker.Token as VariableNameToken;
            ValueTokenArray indicies = null;

            if (variableNameToken == null)
            {
                throw new Exceptions.SyntaxErrorException();
            }

            if (tokenMarker.PeekNext() is OpenParenToken)
            {
                indicies = interpreter.ExpressionEvaluator.EvaluateVariableArrayAssignment(tokenMarker);
            }
            else
            {
                tokenMarker.MoveNext();
            }

            var equalsToken = tokenMarker.Token;
            if (!(equalsToken is Operators.EqualToken))
            {
                throw new Exceptions.SyntaxErrorException();
            }

            if (tokenMarker.GetNextToken() == null)
            {
                throw new Exceptions.SyntaxErrorException();
            }

            var result = interpreter.ExpressionEvaluator.Evaluate(tokenMarker);

            interpreter.VariablesEnvironment.SetVariableValue(variableNameToken, indicies, result);
        }
    }
}
