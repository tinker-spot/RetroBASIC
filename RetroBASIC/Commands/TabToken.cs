using System;
using System.Collections.Generic;
using System.Text;

using RetroBASIC.Values;
using RetroBASIC.Characters;

namespace RetroBASIC.Commands
{
    public class TabToken : CommandToken
    {
        public TabToken() : base("TAB", CommandTokenType.Secondary)
        {

        }

        public override void Execute(Interpreter interpreter, TokenMarker tokenMarker)
        {
            // TAB is only valid inside a PRINT command.
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

            var newColumn = numericValueToken.IntValue;
            if (newColumn < 0 || newColumn > 255)
            {
                throw new Exceptions.IllegalQuantityException();
            }

            // If the current cursor column is already beyond the new column, don't move the cursor
            if (interpreter.Console.CursorColumn > newColumn)
                return;

            interpreter.Console.Tab(newColumn);
        }
    }
}
