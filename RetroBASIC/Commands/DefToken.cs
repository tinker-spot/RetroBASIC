using System;
using System.Collections.Generic;
using System.Text;

using RetroBASIC.Variables;
using RetroBASIC.Characters;

namespace RetroBASIC.Commands
{
    public class DefToken : CommandToken
    {
        public DefToken() : base("DEF", CommandTokenType.Primary)
        {

        }

        public override void Execute(Interpreter interpreter, TokenMarker tokenMarker)
        {
            // Check to see if in direct mode

            if (!(tokenMarker.Token is FnToken))
                throw new Exceptions.SyntaxErrorException();


            var userDefinedFunctionNameToken = tokenMarker.GetNextToken();

            if (!(userDefinedFunctionNameToken is VariableNameToken userDefinedNameToken))
                throw new Exceptions.SyntaxErrorException();

            if (userDefinedNameToken.VariableType != VariableValueType.RealNumber)
                throw new Exceptions.TypeMismatchException();

            var openParenToken = tokenMarker.GetNextToken();

            if (!(openParenToken is OpenParenToken))
                throw new Exceptions.SyntaxErrorException();

            var parameterToken = tokenMarker.GetNextToken();
            if (!(parameterToken is VariableNameToken functionParameterToken))
                throw new Exceptions.SyntaxErrorException();

            var closeParenToken = tokenMarker.GetNextToken();
            if (!(closeParenToken is CloseParenToken))
                throw new Exceptions.SyntaxErrorException();

            var equalsParenToken = tokenMarker.GetNextToken();
            if (!(equalsParenToken is Operators.EqualToken))
                throw new Exceptions.SyntaxErrorException();

            List<Token> userDefinedFunctionTokens = new List<Token>();
            while (tokenMarker.GetNextToken() != null)
            {
                userDefinedFunctionTokens.Add(tokenMarker.Token);
            }

            var userDefinedStatement = new Statement(userDefinedFunctionTokens);

            var userDefinedFunction = new Functions.UserDefinedFunction(functionParameterToken, userDefinedStatement);

            interpreter.VariablesEnvironment.CreateUserDefinedFunction(userDefinedNameToken, userDefinedFunction);
        }
    }
}
