using System;
using System.Collections.Generic;
using System.Text;

using RetroBASIC.Values;
using RetroBASIC.Variables;
using RetroBASIC.Operators;

namespace RetroBASIC.Commands
{
    public class ListToken : CommandToken
    {
        public ListToken() : base("LIST", CommandTokenType.Primary)
        {

        }

        public override void Execute(Interpreter interpreter, TokenMarker tokenMarker)
        {
            // Short circuit in case of no parameters
            if (tokenMarker.Token == null)
            {
                interpreter.List();
                return;
            }

            int startIndex = -1;
            int endIndex = int.MaxValue;

            Token token;
            bool haveMinus = false;
            bool haveEndIndex = false;

            // Look for a starting line or a '-'
            token = tokenMarker.Token;
            switch (token)
            {
                case NumericValueToken startValueToken:
                    startIndex = startValueToken.IntValue;
                    break;

                case MinusToken minusStartToken:
                    haveMinus = true;
                    break;

                default:
                    throw new Exceptions.SyntaxErrorException();

            }

            token = tokenMarker.GetNextToken();
            if (token == null)
            {
                interpreter.List(startIndex, startIndex);
                return;
            }

            //Look for a '-' or an ending number
            switch (token)
            {
                case MinusToken minusEndToken:
                    if (haveMinus)
                        throw new Exceptions.SyntaxErrorException();
                    haveMinus = true;
                    endIndex = int.MaxValue;
                    break;

                case NumericValueToken endValueToken1:
                    endIndex = endValueToken1.IntValue;
                    haveEndIndex = true;
                    break;

                default:
                    throw new Exceptions.SyntaxErrorException();
            }

            token = tokenMarker.GetNextToken();
            if (token == null)
            {
                interpreter.List(startIndex, endIndex);
                return;
            }

            // Look for an ending line number
            switch (token)
            {
                case NumericValueToken endValueToken2:
                    if (haveEndIndex)
                        throw new Exceptions.SyntaxErrorException();
                    endIndex = endValueToken2.IntValue;
                    break;

                default:
                    throw new Exceptions.SyntaxErrorException();
            }

            interpreter.List(startIndex, endIndex);
        }
    }
}
