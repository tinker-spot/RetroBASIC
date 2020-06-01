using System;
using System.Collections.Generic;
using System.Text;

using RetroBASIC.Values;
using RetroBASIC.Characters;

namespace RetroBASIC.Commands
{
    public class SpcToken : CommandToken
    {
        public SpcToken() : base("SPC", CommandTokenType.Secondary)
        {

        }

        public override void Execute(Interpreter interpreter, TokenMarker tokenMarker)
        {
            // SPC is only valid inside a PRINT command
            if (!(interpreter.CurrentCommandToken is PrintToken))
                throw new Exceptions.SyntaxErrorException();

            // Make sure next token is a '('.
            if (tokenMarker.Token == null || !(tokenMarker.Token is OpenParenToken))
                throw new Exceptions.SyntaxErrorException();

            var valueToken = interpreter.ExpressionEvaluator.Evaluate(tokenMarker);
            if (!(valueToken is NumericValueToken numericValueToken))
                throw new Exceptions.TypeMismatchException();

            // Make sure last token in expression ended in a ')'.
            if (!(tokenMarker.PeekPrev() is CloseParenToken))
                throw new Exceptions.SyntaxErrorException();

            var value = numericValueToken.IntValue;
            if (value < 0 || value > 255)
            {
                throw new Exceptions.IllegalQuantityException();
            }

            interpreter.Console.Spc(value);
        }
    }
}
