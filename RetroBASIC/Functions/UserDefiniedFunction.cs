using System;
using System.Collections.Generic;
using System.Text;

using RetroBASIC.Variables;
using RetroBASIC.Values;

namespace RetroBASIC.Functions
{
    public class UserDefinedFunction : FunctionToken
    {
        public UserDefinedFunction(VariableNameToken _variableNameToken, Statement _statement) : base("_", FunctionArgumentFlags.Number | FunctionArgumentFlags.Required)
        {
            variableNameToken = _variableNameToken;
            statement = _statement;
            expressionEvaluator = null;
        }

        VariableNameToken variableNameToken;
        Statement statement;
        ExpressionEvaluator expressionEvaluator;

        public override ValueToken Evaluate(Interpreter interpreter, ValueToken[] valueTokens)
        {
            // A separate expression evaluator is needed since the one created by the interpreter
            // has state unrelated to this statement's state.
            if (expressionEvaluator == null)
            {
                expressionEvaluator = new ExpressionEvaluator(interpreter);
            }

            interpreter.VariablesEnvironment.SetVariableValue(variableNameToken, valueTokens[0]);

            var marker = new TokenMarker(statement.Tokens);

            return expressionEvaluator.Evaluate(marker);
        }
    }
}
