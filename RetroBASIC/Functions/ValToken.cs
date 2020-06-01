using System;
using System.Collections.Generic;
using System.Text;
using RetroBASIC.Values;

namespace RetroBASIC.Functions
{
    public class ValToken : FunctionToken
    {
        public ValToken() : base("VAL", FunctionArgumentFlags.String | FunctionArgumentFlags.Required)
        {
        }

        public override ValueToken Evaluate(Interpreter interpreter, ValueToken[] valueTokens)
        {
            var stringValue = ((StringValueToken)(valueTokens[0])).Value;

            var result = ParseString(stringValue);

            return interpreter.TokensProvider.CreateRealValueToken(result);
        }

        float ParseString(string inputString)
        {
            StringBuilder processed = new StringBuilder();
            bool foundDecimal = false;
            bool foundPlusOrMinus = false;
            bool foundExponentChar = false;
            bool exitLoop = false;

            foreach (var inputChar in inputString)
            {
                switch (inputChar)
                {
                    // Skip spaces
                    case ' ':
                        break;

                    // Only allow one plus or minus on the expression, except for the exponent.
                    case '-':
                    case '+':
                        // Allow a plus or minus to follow an exponent without error
                        if (foundPlusOrMinus && !(processed.Length > 0 && IsExponentChar(processed[processed.Length - 1])))
                        {
                            exitLoop = true;
                            break;
                        }

                        foundPlusOrMinus = true;
                        processed.Append(inputChar);
                        break;

                    // Allow one decimal per expression
                    case '.':
                        if (foundDecimal)
                        {
                            exitLoop = true;
                            break;
                        }

                        foundDecimal = true;
                        processed.Append(inputChar);
                        break;

                    // Handle an exponent character
                    case 'e':
                    case 'E':
                        if (foundExponentChar)
                        {
                            exitLoop = true;
                            break;
                        }

                        foundExponentChar = true;
                        processed.Append(inputChar);
                        break;

                    default:
                        // Accept only digits
                        if (char.IsDigit(inputChar))
                            processed.Append(inputChar);
                        else
                            exitLoop = true;
                        break;
                }

                if (exitLoop)
                    break;
            }

            if (float.TryParse(processed.ToString(), out float result) == false)
                result = 0;

            return result;
        }

        bool IsExponentChar(char ch) => ch == 'e' || ch == 'E';

    }
}
