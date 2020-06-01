using System;
using System.Collections.Generic;
using System.Text;

using RetroBASIC.Values;
using RetroBASIC.Variables;

namespace RetroBASIC.Commands
{
    public class IfToken : CommandToken
    {
        public IfToken() : base("IF", CommandTokenType.Primary)
        {

        }

        public override void Execute(Interpreter interpreter, TokenMarker tokenMarker)
        {
            var conditionToken = interpreter.ExpressionEvaluator.Evaluate(tokenMarker);
            if (!(conditionToken is NumericValueToken resultToken))
                throw new Exceptions.TypeMismatchException();

            // Condition evaluated to false. Skip to the next line.
            if (resultToken.IntValue == 0)
            {
                var nextLineMarker = interpreter.CopyCurrentStatementMarker();
                nextLineMarker.MoveToNextLine();
                interpreter.NextStatementMarker = nextLineMarker;
                return;
            }

            // Condition evaluated to true. Continue on...
            if (!((tokenMarker.Token is ThenToken) || tokenMarker.Token is GotoToken))
                throw new Exceptions.SyntaxErrorException();

            tokenMarker.MoveNext();

            if (tokenMarker.Token is NumericValueToken gotoLineValueToken)
            {
                var gotoLineMarker = interpreter.CreateStatementMarker();
                gotoLineMarker.MoveToLine(gotoLineValueToken.IntValue);
                interpreter.NextStatementMarker = gotoLineMarker;
                return;
            }

            var commandToken = tokenMarker.Token as CommandToken;
            var variableNameToken = tokenMarker.Token as VariableNameToken;
            bool performMoveNext = true;

            if (commandToken == null && variableNameToken != null)
            {
                commandToken = (CommandToken)interpreter.TokensProvider.GetBuiltinToken("LET");
                performMoveNext = false;
            }

            if (commandToken == null)
                throw new Exceptions.SyntaxErrorException();

            // GOTO <linenumber> is fine, GOTO <command> is not!
            if (commandToken is GotoToken)
                throw new Exceptions.SyntaxErrorException();

            // Execute the command following the THEN
            if (performMoveNext)
                tokenMarker.MoveNext();

            commandToken.Execute(interpreter, tokenMarker);
        }
    }
}
