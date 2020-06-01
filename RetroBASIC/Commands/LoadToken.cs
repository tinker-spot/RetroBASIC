using System;
using System.Collections.Generic;
using System.Text;

using RetroBASIC.Variables;
using RetroBASIC.Values;

namespace RetroBASIC.Commands
{
    public class LoadToken : CommandToken
    {
        public LoadToken() : base("LOAD", CommandTokenType.Primary)
        {

        }

        public override void Execute(Interpreter interpreter, TokenMarker tokenMarker)
        {
            string fileName;

            switch (tokenMarker.Token)
            {
                case StringValueToken stringValueToken:
                    fileName = stringValueToken.Value;
                    break;

                case VariableNameToken variableNameToken:
                    var evalToken = interpreter.ExpressionEvaluator.Evaluate(tokenMarker);
                    if (evalToken is StringValueToken evalStringToken)
                    {
                        fileName = evalStringToken.Value;
                        break;
                    }
                    throw new Exceptions.SyntaxErrorException();

                default:
                    throw new Exceptions.SyntaxErrorException();
            }

            interpreter.LoadFromFile(fileName);
        }
    }
}
