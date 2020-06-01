using System;
using System.Collections.Generic;
using System.Text;

using RetroBASIC.Values;
using RetroBASIC.Variables;
using RetroBASIC.Characters;

namespace RetroBASIC.Commands
{
    public class InputToken : CommandToken
    {
        enum ParseFailureReason
        {
            None = 0,
            Colon,
            CharactersAfterQuote
        }

        public InputToken() : base("INPUT", CommandTokenType.Primary)
        {

        }

        public override void Execute(Interpreter interpreter, TokenMarker tokenMarker)
        {
            var firstToken = tokenMarker.Token;

            if (firstToken == null)
                throw new Exceptions.SyntaxErrorException();

            // Is there an input prompt?
            if (firstToken is StringValueToken stringValueToken)
            {
                interpreter.Console.OutputTextWriter.Write(stringValueToken.Value);
                interpreter.Console.OutputTextWriter.Write("? ");

                tokenMarker.MoveNext();

                // Ensure a semicolon follows the input prompt.
                if ((tokenMarker.Token == null) || !(tokenMarker.Token is SemicolonToken))
                    throw new Exceptions.SyntaxErrorException();

                tokenMarker.MoveNext();
            }
            else
            {
                interpreter.Console.OutputTextWriter.Write("? ");
            }

            // Now make a list of the variable names the input will be parsed into.
            var variableNameTokens = new List<(VariableNameToken, ValueTokenArray)>();

            while (tokenMarker.Token != null)
            {
                if (!(tokenMarker.Token is VariableNameToken variableNameToken))
                    throw new Exceptions.SyntaxErrorException();
                ValueTokenArray indicies = null;
                // For an array variable, get the array indicies.
                if (tokenMarker.PeekNext() is OpenParenToken)
                {
                    indicies = interpreter.ExpressionEvaluator.EvaluateVariableArrayAssignment(tokenMarker);
                }
                else
                {
                    tokenMarker.MoveNext();
                }

                variableNameTokens.Add((variableNameToken, indicies));

                if (tokenMarker.Token == null)
                    break;

                // Make sure a comma is the next token
                if (!(tokenMarker.Token is CommaToken))
                    throw new Exceptions.SyntaxErrorException();

                tokenMarker.MoveNext();
            }

            if (variableNameTokens.Count == 0)
                throw new Exceptions.SyntaxErrorException();

            // Add default value for inputs
            var tokenResults = new List<ValueToken>();
            foreach (var (variableNameToken, indicies) in variableNameTokens)
            {
                switch (variableNameToken.VariableType)
                {
                    case VariableValueType.String:
                        tokenResults.Add(interpreter.TokensProvider.CreateStringValueToken(string.Empty));
                        break;

                    case VariableValueType.RealNumber:
                        tokenResults.Add(interpreter.TokensProvider.CreateRealValueToken(0));
                        break;

                    case VariableValueType.IntegerNumber:
                        tokenResults.Add(interpreter.TokensProvider.CreateIntegerValueToken(0));
                        break;
                }
            }

            int tokenResultsIndex = 0;

            // Get input. Support the quirks of Microsoft's 6502 BASIC I could discover.
            bool getMoreInput = true;
            while (getMoreInput)
            {
                string inputLine = interpreter.Console.ReadLine()?.ToUpper();
                if (inputLine == null)
                {
                    // Probably received a Ctrl-C.
                    interpreter.BreakDetected();
                    return;
                }
                /* A quirk of Microsoft BASIC's INPUT implementation is that pressing return without entering anything
                 * uses the default or what has been previously input. */
                if (inputLine.Length == 0)
                {
                    getMoreInput = false;
                    break;
                }

                int index = 0;
                bool parseSuccess = true;

                while (index < inputLine.Length && parseSuccess && tokenResultsIndex < tokenResults.Count)
                {
                    parseSuccess = ParseNextInput(inputLine, index, out index, out string parsedInput, out ParseFailureReason parseFailureReason);
                    // Still assign the input to a variable even if the parse failed. Another Microsoft BASIC quirk.
                    bool assignmentSuccess = AssignInputToVariable(interpreter,
                        variableNameTokens[tokenResultsIndex].Item1,
                        variableNameTokens[tokenResultsIndex].Item2,
                        parsedInput);

                    if (!parseSuccess)
                    {
                        switch (parseFailureReason)
                        {
                            // Because characters followed a closing quote but shouldn't, write out a redo from start message and start over.
                            case ParseFailureReason.CharactersAfterQuote:
                                // Set to -1 since one will be added below to bring the index back to zero
                                tokenResultsIndex = -1; 
                                interpreter.Console.OutputTextWriter.WriteLine($"{ErrorMessages.errorStart} {ErrorMessages.redoFromStart}");
                                break;

                            // No error output for a colon, except when it involves the last result.
                            case ParseFailureReason.Colon:
                                if (tokenResultsIndex + 1 == tokenResults.Count)
                                {
                                    interpreter.Console.OutputTextWriter.WriteLine($"{ErrorMessages.errorStart} {ErrorMessages.extraIgnored}");
                                }
                                break;
                        }
                    }
                    tokenResultsIndex += 1;
                }

                if (tokenResultsIndex < tokenResults.Count)
                {
                    // Still need more input and no more input is on this line. Ask for more.
                    if (index >= inputLine.Length)
                    {
                        interpreter.Console.OutputTextWriter.Write("?? ");
                        continue;
                    }
                }
                else
                {
                    // For too much input, write out an extra ignored message.
                    if (index < inputLine.Length)
                    {
                        interpreter.Console.OutputTextWriter.WriteLine($"{ErrorMessages.errorStart} {ErrorMessages.extraIgnored}");
                    }
                    getMoreInput = false;
                }
            }
        }

        bool ParseNextInput(string inputLine, int startIndex, out int endIndex, out string parsedInput, out ParseFailureReason parseFailureReason)
        {
            bool parseSuccess = true;
            parseFailureReason = ParseFailureReason.None;

            // Skip leading whitespace
            SkipWhiteSpace(inputLine, startIndex, out startIndex);

            switch (inputLine[startIndex])
            {
                // Parse a quoted string
                case '\"':
                    FindEndofQuoteOrEOL(inputLine, startIndex, out endIndex);

                    parsedInput = inputLine.Substring(startIndex, endIndex - startIndex);

                    if (endIndex >= inputLine.Length)
                        break;

                    // Skip any whitespace.
                    SkipWhiteSpace(inputLine, endIndex, out endIndex);

                    // Quit when an EOL is found
                    if (endIndex >= inputLine.Length)
                        break;

                    // If the next character isn't a comma, then we have a parse error.
                    if (inputLine[endIndex] != ',')
                    {
                        parseFailureReason = ParseFailureReason.CharactersAfterQuote;
                        parseSuccess = false;
                        endIndex = inputLine.IndexOf(',', endIndex);
                        if (endIndex == -1)
                            endIndex = inputLine.Length;
                        else
                            endIndex += 1;
                    }
                    else
                    {
                        endIndex += 1;
                    }

                    break;

                // For a comma, return the default 
                case ',':
                    parsedInput = string.Empty;
                    endIndex = startIndex + 1;
                    break;

                default:
                    endIndex = startIndex;
                    /* Either a comma or a colon end the string. A colon ends an input because in
                     * Microsoft BASIC GET/READ/INPUT share code and ':' is a statement separator.
                     */
                    try
                    {
                        while (inputLine[endIndex] != ',' && inputLine[endIndex] != ':')
                        {
                            endIndex += 1;
                        }
                    }
                    catch (IndexOutOfRangeException)
                    {

                    }

                    parsedInput = inputLine.Substring(startIndex, endIndex - startIndex);
                    if (endIndex < inputLine.Length)
                    {
                        // If a colon was found, set an error and return.
                        if (inputLine[endIndex] == ':')
                        {
                            parseFailureReason = ParseFailureReason.Colon;
                            parseSuccess = false;

                            endIndex = inputLine.Length;
                        }
                        else
                        {
                            endIndex += 1;
                        }
                    }
                    break;
            }

            return parseSuccess;
        }

        void FindEndofQuoteOrEOL(string inputLine, int startIndex, out int endIndex)
        {
            // Move past the quote character
            startIndex += 1;

            // Find the end quote
            endIndex = inputLine.IndexOf('\"', startIndex);

            // If EOL, then set the end index to the line length
            if (endIndex == -1)
                endIndex = inputLine.Length;
            else
                endIndex += 1;
        }

        void SkipWhiteSpace(string inputLine, int startIndex, out int endIndex)
        {
            endIndex = startIndex;
            if (startIndex >= inputLine.Length)
                return;

            try
            {
                while (inputLine[endIndex] == ' ')
                    endIndex += 1;
            }
            catch (IndexOutOfRangeException)
            {

            }
        }

        bool AssignInputToVariable(Interpreter interpreter, VariableNameToken variableNameToken, ValueTokenArray indicies, string parsedInput)
        {
            switch (variableNameToken.VariableType)
            {
                case VariableValueType.String:
                    StringValueToken stringValueToken;

                    // Strip the quotes before setting the value to the variable.
                    if (parsedInput.StartsWith('\"'))
                    {
                        int endIndex = parsedInput.Length;
                        if (parsedInput[endIndex - 1] == '\"')
                            endIndex -= 1;
                        stringValueToken = interpreter.TokensProvider.CreateStringValueToken(parsedInput.Substring(1, endIndex - 1 - 1));
                    }
                    else
                    {
                        stringValueToken = interpreter.TokensProvider.CreateStringValueToken(parsedInput);
                    }

                    interpreter.VariablesEnvironment.SetVariableValue(variableNameToken, indicies, stringValueToken);

                    return true;

                case VariableValueType.RealNumber:
                    // If the string starts with a quote, it's an error.
                    // ... even if what's in quotes can parse to a number
                    if (parsedInput[0] == '\"')
                        return false;

                    // If the conversion fails, it's an error.
                    if (float.TryParse(parsedInput, out float realValue) == false)
                        return false;

                    var realValueToken = interpreter.TokensProvider.CreateRealValueToken(realValue);
                    interpreter.VariablesEnvironment.SetVariableValue(variableNameToken, indicies, realValueToken);

                    break;

                case VariableValueType.IntegerNumber:
                    // If the string starts with a quote, it's an error.
                    // ... even if what's in quotes can parse to a number
                    if (parsedInput[0] == '\"')
                        return false;

                    // If the conversion fails, it's an error.
                    if (Int16.TryParse(parsedInput, out Int16 intValue) == false)
                        return false;

                    var intValueToken = interpreter.TokensProvider.CreateIntegerValueToken(intValue);
                    interpreter.VariablesEnvironment.SetVariableValue(variableNameToken, indicies, intValueToken);
                    break;
            }
            return true;
        }
    }
}
