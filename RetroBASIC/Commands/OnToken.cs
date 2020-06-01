using System;
using System.Collections.Generic;
using System.Text;

using RetroBASIC.Values;
using RetroBASIC.Variables;
using RetroBASIC.Characters;

namespace RetroBASIC.Commands
{
    public class OnToken : CommandToken
    {
        public OnToken() : base("ON", CommandTokenType.Primary)
        {

        }

        public override void Execute(Interpreter interpreter, TokenMarker tokenMarker)
        {
            var indexToken = interpreter.ExpressionEvaluator.Evaluate(tokenMarker);
            if (!(indexToken is NumericValueToken indexValueToken))
                throw new Exceptions.TypeMismatchException();

            var commandToken = tokenMarker.Token;

            if (!(commandToken is GosubToken || commandToken is GotoToken))
                throw new Exceptions.SyntaxErrorException();

            tokenMarker.MoveNext();

            var index = indexValueToken.IntValue;
            if (index < 0 || index > 255)
                throw new Exceptions.IllegalQuantityException();

            if (index == 0)
                return;

            index -= 1;
            while (index > 0 && tokenMarker.Token != null)
            {
                var token = tokenMarker.Token;
                if (!(token is NumericValueToken))
                    throw new Exceptions.SyntaxErrorException();

                tokenMarker.MoveNext();

                if (tokenMarker.Token == null)
                    break;

                if (!(tokenMarker.Token is CommaToken))
                    throw new Exceptions.SyntaxErrorException();

                tokenMarker.MoveNext();
                index -= 1;
            }

            // Index was out of range. Continue on to the next statement.
            if (tokenMarker.Token == null)
                return;

            if (!(tokenMarker.Token is NumericValueToken lineNumberToken))
                throw new Exceptions.SyntaxErrorException();

            var lineNumber = lineNumberToken.IntValue;

            if (commandToken is GosubToken)
            {
                // Setup the return stack.
                var currentMarker = interpreter.CopyCurrentStatementMarker();
                currentMarker.MoveToNextStatement();
                interpreter.GosubMarkers.Push(currentMarker);
            }

            var gotoMarker = interpreter.CreateStatementMarker();
            if (gotoMarker.MoveToLine(lineNumber) == false)
                throw new Exceptions.UndefinedStatementException();

            interpreter.NextStatementMarker = gotoMarker;

        }
    }
}
