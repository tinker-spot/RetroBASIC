using System;
using System.Collections.Generic;
using System.Text;

using RetroBASIC.Values;
using RetroBASIC.Variables;
using RetroBASIC.Characters;

namespace RetroBASIC.Commands
{
    class ReadToken : CommandToken
    {
        public ReadToken() : base("READ", CommandTokenType.Primary)
        {
            
        }

        public override void Execute(Interpreter interpreter, TokenMarker tokenMarker)
        {
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

                var valueToken = interpreter.ReadNextDataValue();
                if (valueToken == null)
                    throw new Exceptions.OutOfDataException();

                switch (variableNameToken.VariableType)
                {
                    case VariableValueType.String:
                        break;

                    case VariableValueType.RealNumber:
                        float realValue = 0;
                        var success = float.TryParse(((StringValueToken)valueToken).Value, out realValue);
                        if (success == false)
                        {
                            throw new Exceptions.TypeMismatchException();
                        }
                        valueToken = interpreter.TokensProvider.CreateRealValueToken(realValue);
                        break;

                    case VariableValueType.IntegerNumber:                        
                        success = float.TryParse(((StringValueToken)valueToken).Value, out realValue);
                        if (success == false)
                        {
                            throw new Exceptions.TypeMismatchException();
                        }
                        // In Commodore BASIC, a READ of a number with a decimal into an integer number produces a zero.
                        int intValue = (int)Math.Abs(realValue);
                        if (realValue != intValue)
                            intValue = 0;

                        if (intValue < Int16.MinValue || intValue > Int16.MaxValue)
                            throw new Exceptions.OverflowException();

                        valueToken = interpreter.TokensProvider.CreateIntegerValueToken((Int16)intValue);
                        break;
                }

                interpreter.VariablesEnvironment.SetVariableValue(variableNameToken, indicies, valueToken);

                if (tokenMarker.Token == null)
                    break;

                if (!(tokenMarker.Token is CommaToken))
                    throw new Exceptions.SyntaxErrorException();

                tokenMarker.MoveNext();
                // The next token should now be a variable name token
            }
        }

    }
}
