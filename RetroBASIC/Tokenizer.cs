using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

using RetroBASIC.Values;
using RetroBASIC.Variables;
using RetroBASIC.Commands;
using RetroBASIC.Operators;
using RetroBASIC.Characters;

namespace RetroBASIC
{
    public class Tokenizer
    {
        const char LINE_SEPARATOR = ':';

        string untokenizedLine;
        List<Statement> statements;
        List<Token> statementBuilder;
//        Statement tokenizedStatement;
//        int lineNumber;
        TokensProvider tokensProvider;
        char? currentChar;
        StringBuilder tokenNameBuilder;
        int currentCharPos = -1;

        public Tokenizer(TokensProvider _tokensProvider)
        {
            tokensProvider = _tokensProvider;
            currentChar = null;
            tokenNameBuilder = new StringBuilder();
//            statements = new List<Statement>();
        }

        private void SetCurrentCharIndex(int index)
        {
            try
            {
                currentChar = untokenizedLine[index];
                currentCharPos = index;
            }
            catch (IndexOutOfRangeException)
            {
                currentChar = null;
                currentCharPos = untokenizedLine.Length;
            }
        }

        int CurrentCharIndex { get { return currentCharPos; } set { SetCurrentCharIndex(value); } }

        public ValueTuple<int, List<Statement>> TokenizeLine(string inputLine)
        {
            if (inputLine is null || inputLine.Length == 0)
                return (-1, null);

            this.untokenizedLine = inputLine;
            CurrentCharIndex = 0;
            statements = new List<Statement>();

            int lineNumber = -1;
//            tokenizedStatement = new Statement();
            statementBuilder = new List<Token>();

            SkipWhiteSpace();

            if (currentChar == null)
            {
                return (lineNumber, statements);
            }

            // Is there a line number?
            if (Char.IsDigit(currentChar.Value))
            {
                ParseLineNumber(out lineNumber) ;
            }

            SkipWhiteSpace();

            while (currentChar != null)
            {
                if (currentChar == ' ')
                {
                    var count = statementBuilder.Count;
                    if (count > 0 /* &&
                        (statementBuilder[count-1].TokenType == TokenType.Command)*/)
                    {
                        var whitespacesToken = TokenizeAnyWhiteSpace();
                        if (whitespacesToken != null)
                            statementBuilder.Add(whitespacesToken);

                        continue;
                    }
                    SkipWhiteSpace();
                    continue;
                }

                // For a statement separator, add current tokens to line then create a new list of tokens.
                if (currentChar == LINE_SEPARATOR)
                {
                    statements.Add(new Statement(statementBuilder));
                    statementBuilder = new List<Token>();
                    CurrentCharIndex += 1;
                    continue;
                }

                // Is this a keyword or an identifier?
                if (Char.IsLetter(currentChar.Value))
                {
                    TokenizeKeywordOrVariableName();
                    continue;
                }

                // Now check for a number.
                if (Char.IsDigit(currentChar.Value) || currentChar == '.')
                {
                    TokenizeNumberConstant();
                    continue;
                }

                // Now check for a quoted string
                if (currentChar == '"')
                {
                    TokenizeQuotedString();
                    continue;
                }

                TokenizeCharacter();
            }
            if (statementBuilder.Count > 0)
            {
                statements.Add(new Statement(statementBuilder));
            }

            return (lineNumber, statements);
        }

        void ParseLineNumber(out int lineNumber)
        {
            lineNumber = 0;
            while (currentChar != null && Char.IsDigit(currentChar.Value))
            {
                lineNumber = (currentChar.Value - '0') + (lineNumber * 10);
                CurrentCharIndex += 1;
            }

        }

        void TokenizeKeywordOrVariableName()
        {
            var variableNameBuffer = new StringBuilder();
            WhitespacesToken whitespacesToken = null;

            while (currentChar != null && (IsCharOrDigit(currentChar.Value) || (currentChar == '$' || currentChar == '%')))
            {
                var keywordStartIndex = CurrentCharIndex;
                char? stopCharacter = null;

                // Find the next character that isn't a variable or keyword character
                while (currentChar != null && IsCharOrDigit(currentChar.Value))
                {
                    CurrentCharIndex += 1;
                }

                // Record the charcater that ended the while loop.
                if ((currentChar == '$') || (currentChar == '%'))
                {
                    stopCharacter = currentChar;
                    CurrentCharIndex += 1;
                }
                else
                {
                    stopCharacter = currentChar;
                }

                var searchString = untokenizedLine[keywordStartIndex..CurrentCharIndex];
                var keywordResultsQuery = from tokenKV in tokensProvider.Tokens
                                          where (searchString.Length >= tokenKV.Value.TokenName.Length &&
                                                 searchString.StartsWith(tokenKV.Value.TokenName, StringComparison.InvariantCultureIgnoreCase))
                                          orderby tokenKV.Value.TokenName.Length descending
                                          select tokenKV.Value;
                var keywordSearchResult = keywordResultsQuery.FirstOrDefault();
                if (keywordSearchResult != null)
                {
                    if (variableNameBuffer.Length > 0)
                    {
                        TokenizeVariableName(variableNameBuffer);
                        variableNameBuffer.Clear();

                        if (whitespacesToken != null)
                        {
                            statementBuilder.Add(whitespacesToken);
                            whitespacesToken = null;
                        }
                    }
                    statementBuilder.Add(keywordSearchResult);

                    var keywordCommandResult = keywordSearchResult as CommandToken;
                    // For comments, don't tokenize the rest of the line
                    if (keywordCommandResult != null)
                    {
                        if (keywordCommandResult.CommandType == Commands.CommandTokenType.Comment &&
                            currentChar != null)
                        {
                            var commentToken = tokensProvider.CreateCommentToken(untokenizedLine.Substring(CurrentCharIndex));
                            statementBuilder.Add(commentToken);
                            CurrentCharIndex = untokenizedLine.Length;
                            continue;
                        }

                        if (keywordCommandResult is DataToken)
                        {
                            CurrentCharIndex = keywordStartIndex + keywordSearchResult.TokenName.Length;
                            TokenizeDataCommand();
                            continue;
                        }
                    }

                    if (keywordSearchResult.TokenName.Length < searchString.Length)
                    {
                        CurrentCharIndex = keywordStartIndex + keywordSearchResult.TokenName.Length;
                        // If the current character isn't a character, then we have something else that needs tokenizing elsewhere.
                        if (!char.IsLetter(currentChar.Value))
                            break;
                    }

                    continue;
                }

                //                variableNameBuffer.Append(untokenizedLine, keywordStartIndex, CurrentCharIndex - keywordStartIndex);
                variableNameBuffer.Append(untokenizedLine[keywordStartIndex]);
                CurrentCharIndex = keywordStartIndex + 1;
                continue;

                // If the character that failed the while loop was a not space, tokenize the variable name.
                // Otherwise, keep adding characters to the variable name. This tomfoolery mimics the behavior of
                // Microsoft's 6502 BASIC tokenizer.
                if (stopCharacter != ' ')
                {
                    TokenizeVariableName(variableNameBuffer);
                }
                whitespacesToken = TokenizeAnyWhiteSpace();
            }

            if (variableNameBuffer.Length > 0)
            {
                TokenizeVariableName(variableNameBuffer);
                if (whitespacesToken != null)
                    statementBuilder.Add(whitespacesToken);
            }
        }

        void TokenizeVariableName(StringBuilder variableNameBuilder)
        {
            VariableValueType variableType = VariableValueType.RealNumber;

            switch (variableNameBuilder[^1])
            {
                // Mark variable name as a string
                case '$':
                    variableType = VariableValueType.String;
                    variableNameBuilder.Length -= 1;
                    break;

                // Mark variable name as an integer
                case '%':
                    variableType = VariableValueType.IntegerNumber;
                    variableNameBuilder.Length -= 1;
                    break;

                default:
                    break;
            }

            var variableNameToken = tokensProvider.GetOrCreateVariableNameToken(variableNameBuilder.ToString(), variableType);
            statementBuilder.Add(variableNameToken);
            variableNameBuilder.Length = 0;
        }

        bool IsCharOrDigit(char ch)
        {
            return Char.IsLetter(ch) || Char.IsDigit(ch);

        }
        void TokenizeNumberConstant()
        {
            bool haveSeenExponentCharacter = false;

            StringBuilder numbers = new StringBuilder();
            while (currentChar != null && (Char.IsDigit(currentChar.Value) || currentChar == '.' || currentChar == 'E' || currentChar == '-'))
            {
                if ((currentChar == 'E') || (currentChar == 'e'))
                    haveSeenExponentCharacter = true;

                if (currentChar == '-' && !haveSeenExponentCharacter)
                    break;

                SkipWhiteSpace();
                numbers.Append(currentChar);
                CurrentCharIndex += 1;
            }

            float value = float.Parse(numbers.ToString());
            var numberToken = new Values.RealValueToken(value);
            statementBuilder.Add(numberToken);

            var whitespacesToken = TokenizeAnyWhiteSpace();
            if (whitespacesToken != null)
                statementBuilder.Add(whitespacesToken);
        }

        void TokenizeQuotedString()
        {
            // Move past opening quote
            CurrentCharIndex += 1;

            int startQuoteIndex = CurrentCharIndex;

            int endQuoteIndex = untokenizedLine.IndexOf('\"', CurrentCharIndex);
            if (endQuoteIndex == -1)
                endQuoteIndex = untokenizedLine.Length;

            var result = untokenizedLine[startQuoteIndex..endQuoteIndex];

            CurrentCharIndex = endQuoteIndex;

            if (currentChar == '"')
                CurrentCharIndex += 1;

            var stringToken = new Values.StringValueToken(result);
            statementBuilder.Add(stringToken);

        }

        void TokenizeCharacter()
        {
            // Check for a one character long token
            var token = tokensProvider.GetBuiltinToken(currentChar.Value);
            if (token == null)
            {
                token = tokensProvider.CreateUnknownCharacterToken(currentChar.Value);
            }

            // Try to combine <=, >=, and <> tokens into one.
            if (token is OperatorToken)
            {
                var prevTokenIndex = statementBuilder.Count - 1;
                // Make sure the previous token was also an operator
                if (prevTokenIndex >= 0 && (statementBuilder[prevTokenIndex] is OperatorToken prevToken))
                {
                    string combinedOperator = prevToken.TokenName + token.TokenName;

                    var newToken = tokensProvider.GetBuiltinToken(combinedOperator);
                    if (newToken != null)
                    {
                        // Two operator tokens can be combined into one. Remove the previous token and prepare to add the combined one
                        statementBuilder.RemoveAt(prevTokenIndex);
                        token = newToken;
                    }

                }
            }

            // Add the token and move to the next character
            statementBuilder.Add(token);
            CurrentCharIndex += 1;

        }

        void TokenizeUnquotedDataString()
        {
            int startIndex = CurrentCharIndex;

            while (currentChar != null && currentChar.Value != ',' && currentChar.Value != LINE_SEPARATOR)
            {
                CurrentCharIndex += 1;
            }

            var result = untokenizedLine[startIndex..CurrentCharIndex];
            var stringToken = new Values.StringValueToken(result);

            statementBuilder.Add(stringToken);
        }

        void TokenizeDataCommand()
        {
            while (currentChar != null && currentChar.Value != LINE_SEPARATOR)
            {
                SkipWhiteSpace();
                // Handle a quoted string just like any other quoted string
                if (currentChar.Value == '"')
                {
                    TokenizeQuotedString();
                    continue;
                }
                
                // Tokenize an item seperator.
                if (currentChar.Value == ',')
                {
                    if (statementBuilder.Count > 0 && statementBuilder[^1] is CommaToken)
                    {
                        statementBuilder.Add(TokensProvider.emptyStringToken);
                    }

                    TokenizeCharacter();
                    continue;
                }

                // Anything else is considered an unquoted string
                TokenizeUnquotedDataString();
            }
        }

        void SkipWhiteSpace()
        {
            while (currentChar != null && Char.IsWhiteSpace(currentChar.Value))
            {
                CurrentCharIndex += 1;
            }
        }

        WhitespacesToken TokenizeAnyWhiteSpace()
        {
            int times = 0;
            while (currentChar != null && Char.IsWhiteSpace(currentChar.Value))
            {
                times += 1;
                CurrentCharIndex += 1;
            }

            if (times > 0)
            {
                return tokensProvider.CreateWhitespacesToken(times);
            }

            return null;
        }
    }
}
