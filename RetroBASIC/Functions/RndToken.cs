using System;
using System.Collections.Generic;
using System.Text;
using RetroBASIC.Values;

namespace RetroBASIC.Functions
{
    public class RndToken : FunctionToken
    {
        public RndToken() : base("RND", FunctionArgumentFlags.Number | FunctionArgumentFlags.Required)
        {
            randomProvider = null;
        }

        private Random randomProvider;

        public override ValueToken Evaluate(Interpreter interpreter, ValueToken[] valueTokens)
        {
            var seedValue = (((NumericValueToken)valueTokens[0])).IntValue;

            // Negative number will use a random number generator that will generate a reproducable sequence
            if (seedValue < 0)
                randomProvider = new Random(seedValue);
            // A zero will produce a pseudo random sequence based on the current time
            else if (seedValue == 0 || randomProvider == null)
                randomProvider = new Random();

            // A positive number will produce the next pseudo random number in the sequence
            var randomNumber = randomProvider.NextDouble();
            return interpreter.TokensProvider.CreateRealValueToken((float)randomNumber);
        }
    }
}
