using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

using RetroBASIC.Values;
using RetroBASIC.Variables;
using RetroBASIC.Characters;

namespace RetroBASIC.Commands
{
    public class GetToken : CommandToken
    {
        public GetToken() : base("GET", CommandTokenType.Primary)
        {

        }

        public override void Execute(Interpreter interpreter, TokenMarker tokenMarker)
        {
            if (interpreter.InterpreterMode == InterpreterMode.Immmediate)
                throw new Exceptions.IllegalDirectModeException();

            var variableNameTokens = new List<(VariableNameToken, ValueTokenArray)>();

            while (tokenMarker.Token != null)
            {
                ValueTokenArray indicies = null;
                if (!(tokenMarker.Token is VariableNameToken variableNameToken))
                    throw new Exceptions.SyntaxErrorException();

                if (tokenMarker.PeekNext() is OpenParenToken)
                {
                    indicies = interpreter.ExpressionEvaluator.EvaluateVariableArrayAssignment(tokenMarker);
                }
                else
                {
                    tokenMarker.MoveNext();
                }

                if (tokenMarker.Token != null && !(tokenMarker.Token is CommaToken))
                    throw new Exceptions.SyntaxErrorException();

                variableNameTokens.Add((variableNameToken, indicies));
            }

            foreach (var (nameToken, indicies) in variableNameTokens)
            {
                var input = interpreter.Console.ReadChar();
                switch (nameToken.VariableType)
                {
                    case VariableValueType.String:
                        string inputString = (input != null) ? input.ToString() : string.Empty;
                        interpreter.VariablesEnvironment.SetVariableValue(nameToken, indicies,
                            interpreter.TokensProvider.CreateStringValueToken(inputString));
                        break;

                    case VariableValueType.RealNumber:
                        float realValue;
                        if (input == null)
                        {
                            realValue = 0;
                        }
                        else
                        {
                            if (input >= '0' && input <= '9')
                            {
                                realValue = (float)(input - '0');
                            }
                            else
                            {
                                throw new Exceptions.SyntaxErrorException();
                            }
                        }
                        interpreter.VariablesEnvironment.SetVariableValue(nameToken, indicies,
                            interpreter.TokensProvider.CreateRealValueToken(realValue));

                        break;

                    case VariableValueType.IntegerNumber:
                        Int16 intValue;
                        if (input == null)
                        {
                            intValue = 0;
                        }
                        else
                        {
                            if (input >= '0' && input <= '9')
                            {
                                intValue = (Int16)(input - '0');
                            }
                            else
                            {
                                throw new Exceptions.SyntaxErrorException();
                            }
                        }
                        interpreter.VariablesEnvironment.SetVariableValue(nameToken, indicies,
                            interpreter.TokensProvider.CreateIntegerValueToken(intValue));

                        break;
                }

                if (input == null)
                    Thread.Sleep(100);
            }
        }
    }
}
