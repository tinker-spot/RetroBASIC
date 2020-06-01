using System;
using System.Collections.Generic;
using System.Text;

using RetroBASIC.Values;
using RetroBASIC.Variables;
using RetroBASIC.Characters;
using RetroBASIC.Functions;
using RetroBASIC.Operators;

namespace RetroBASIC.Commands
{
    public class PrintToken : CommandToken
    {
        const string numberFormatting = "{0,1}";

        public PrintToken() : base("PRINT", CommandTokenType.Primary)
        {

        }

        public override void Execute(Interpreter interpreter, TokenMarker tokenMarker)
        {
            // For an empty PRINT command, just output a writeline and return.
            if (tokenMarker.Token == null)
            {
                interpreter.Console.OutputTextWriter.WriteLine();
                return;
            }

            Token lastToken = null;

            while (tokenMarker.Token != null)
            {
                lastToken = tokenMarker.Token;
                switch (tokenMarker.Token)
                {
                    case StringValueToken stringValueToken:
                    case RealValueToken realValueToken:
                    case IntegerValueToken integerValueToken:
                    case UnaryMinusToken unaryMinusToken:
                    case MinusToken minusToken:
                    case PlusToken plusToken:
                    case VariableNameToken variableToken:
                    case FunctionToken functionToken:
                    case FnToken userDefinedFunctionToken:
                    case OpenParenToken openParenToken:
                        var result = interpreter.ExpressionEvaluator.Evaluate(tokenMarker);
                        WriteValue(interpreter, result);
                        break;

                    case SemicolonToken semiColonToken:
                        tokenMarker.MoveNext();
                        break;

                    case CommaToken commaToken:
                        MoveToNextTabMargin(interpreter);
                        tokenMarker.MoveNext();
                        break;

                    case TabToken tabToken:
                        tokenMarker.MoveNext();
                        tabToken.Execute(interpreter, tokenMarker);
                        break;

                    case SpcToken spcToken:
                        tokenMarker.MoveNext();
                        spcToken.Execute(interpreter, tokenMarker);
                        break;

                    default:
                        throw new Exceptions.SyntaxErrorException();
                }
//                lastToken = tokenMarker.Token;
//                tokenMarker.MoveNext();
            }
            if (!((lastToken is SemicolonToken) || (lastToken is CommaToken)))
            {
                 interpreter.Console.OutputTextWriter.WriteLine();
            }
        }

        void WriteValue(Interpreter interpreter, Token token)
        {
            var OutputTextWriter = interpreter.Console.OutputTextWriter;

            switch (token)
            {
                case StringValueToken stringValueToken:
                    OutputTextWriter.Write(stringValueToken.Value);
                    break;

                case RealValueToken realValueToken:
                    if (realValueToken.Value >= 0)
                        OutputTextWriter.Write(' ');

                    OutputTextWriter.Write(realValueToken.Value);
                    OutputTextWriter.Write(' ');

                    break;

                case IntegerValueToken integerValueToken:
                    if (integerValueToken.Value >= 0)
                        OutputTextWriter.Write(' ');

                    interpreter.Console.OutputTextWriter.Write(integerValueToken.Value);
                    OutputTextWriter.Write(' ');

                    break;
            }
        }

        void MoveToNextTabMargin(Interpreter interpreter)
        {
            var currentCol = interpreter.Console.CursorColumn;
            // Move to the next tab margin, set at 15 for the comma to match the output printouts shown in Basic Computer Games.
            int newCol = ((int)(currentCol / Constants.COMMA_TAB_COLUMN_MARGIN)) * Constants.COMMA_TAB_COLUMN_MARGIN + Constants.COMMA_TAB_COLUMN_MARGIN;
            interpreter.Console.CursorColumn = newCol;
        }
    }
}
