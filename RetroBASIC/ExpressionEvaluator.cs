using RetroBASIC.Characters;
using RetroBASIC.Commands;
using RetroBASIC.Functions;
using RetroBASIC.Operators;
using RetroBASIC.Values;
using RetroBASIC.Variables;
using System;
using System.Collections.Generic;

namespace RetroBASIC
{
    public class ExpressionEvaluator
    {
        private Interpreter interpreter;
        private InterpreterVariables interpreterEnvironment;
        private TokensProvider tokensProvider;

        private Stack<ValueToken> valueTokens;
        private Stack<Token> operatorTokens;
        private Stack<Func<Token, ValueToken>> operatorActions;
        private Stack<int> arrayCount;

        public ExpressionEvaluator(Interpreter _interpreter)
        {
            interpreter = _interpreter;
            interpreterEnvironment = _interpreter.VariablesEnvironment;
            tokensProvider = _interpreter.TokensProvider;

            valueTokens = new Stack<ValueToken>();
            operatorTokens = new Stack<Token>();
            operatorActions = new Stack<Func<Token, ValueToken>>();
            arrayCount = new Stack<int>();

        }

        public void Clear()
        {
            operatorTokens.Clear();
            operatorActions.Clear();
            valueTokens.Clear();
            arrayCount.Clear();
        }

        public ValueToken Evaluate(TokenMarker tokenMarker)
        {
            return EvaluateInternal(tokenMarker);
        }


        // Evalute the X(A,...Z ) part of the expression LET X(A,...Z)=[expression].
        public ValueTokenArray EvaluateVariableArrayAssignment(TokenMarker tokenMarker)
        {
            if (!(tokenMarker.Token is VariableNameToken))
                throw new Exceptions.SyntaxErrorException();

            // Push the token and a custom action to be performed when the closing ")" is evaluated.
            operatorTokens.Push(tokenMarker.Token);
            operatorActions.Push(x => EvaluateVariableAssignmentToken(x));
            arrayCount.Push(0);

            tokenMarker.MoveNext();

            // Have the Evaluate function return when the closing ")" is evaluated.
            var result = EvaluateInternal(tokenMarker, () => (this.arrayCount.Count > 0));
            return (ValueTokenArray)result;

        }

        private ValueToken EvaluateInternal(TokenMarker tokenMarker, Func<bool> ContinueIfTrue = null)
        {
            Token token = null;
            Token prevToken = null;

            if (ContinueIfTrue == null)
                ContinueIfTrue = () => true;

            while (tokenMarker.Token != null && ContinueIfTrue())
            {
                prevToken = token;
                token = tokenMarker.Token;
                tokenMarker.MoveNext();

                // Push a number on to the result stack.
                if (token is ValueToken valueToken)
                {
                    if (prevToken != null && ((prevToken is VariableNameToken) || (prevToken is CloseParenToken)))
                    {
                        // A variable token directly follows a quote. Let the caller decide if this an error.
                        tokenMarker.MovePrev();
                        break;
                    }

                    valueTokens.Push(valueToken);
                    continue;
                }

                // Return value of the variable
                if (token is VariableNameToken variableNameToken)
                {
                    // Make sure the previous token was an operator, if not let the caller decide if it's an error
                    if (prevToken != null && ((prevToken is VariableNameToken) || (prevToken is ValueToken)))
                    {
                        tokenMarker.MovePrev();
                        break;
                    }

                    var nextToken = tokenMarker.Token;
                    if (nextToken == null || !(nextToken is OpenParenToken))
                    {
                        // For non-array variables, get the value now and push the resultt.
                        var value = interpreterEnvironment.GetVariableValue(variableNameToken);
                        valueTokens.Push(value);
                        continue;
                    }
                    // For array variables, push the token and setup for evaluation later.
                    operatorTokens.Push(token);
                    operatorActions.Push(x => EvaluateGetArrayVariableToken(x));
                    arrayCount.Push(0);
                    continue;
                }

                // Push opening brack to operator stack
                if (token is OpenParenToken)
                {
                    operatorTokens.Push(token);
                    continue;
                }

                // Handle Unary operators
                if ((prevToken == null || prevToken is OperatorToken || prevToken is OpenParenToken) &&
                    token is OperatorToken)
                {
                    switch (token)
                    {
                        // Push the unary minus token
                        case MinusToken minusToken:
                            token = tokensProvider.UnaryMinusToken;
                            operatorTokens.Push(token);
                            operatorActions.Push(x => EvaluateUnaryOperatorToken(x));
                            continue;

                        // A unary plus does nothing, so also do nothing.
                        case PlusToken plusToken:
                            continue;

                        default:
                            throw new Exceptions.SyntaxErrorException();

                    }
                }

                // If a closing parenthesis is found, solve until opening brace
                if (token is CloseParenToken)
                {
                    if (operatorTokens.Count > 0)
                    {
                        /* Evaluate the expression back to the opening brace or last comma
                         * to properly handle Fn(x-1, x-2) */
                        FinishEvaluatingExpressionChunk();

                        // Remove opening brace
                        operatorTokens.Pop();

                        if (arrayCount.Count > 0 && operatorTokens.TryPeek(out Token topToken) &&
                                ((topToken is FunctionToken) || (topToken is VariableNameToken)))
                        {
                            if (prevToken is CommaToken)
                            {
                                // Not in a function or in an array dimension, so let the caller decide if this is an error.
                                tokenMarker.MovePrev();
                                break;
                            }
                            // Found a closing parenthesis for a function or an array variable. Add one to the array count.
                            var count = arrayCount.Pop();
                            arrayCount.Push(count + 1);
                            // Now remove the operator token and call the lambda function to finish
                            // evaluating the function or array reference.
                            PerformEvaluationAction();
                        }

                        continue;
                    }

                    // Too many closing parenthesis is a syntax error.
                    throw new Exceptions.SyntaxErrorException();
                }

                // As long as precedence allows, evaluate the operator tokens.
                while (operatorTokens.TryPeek(out Token stackToken) &&
                      (stackToken is OperatorToken operatorStackToken) &&
                      (token is OperatorToken currentOperatorToken) &&
                      operatorStackToken.Precedence >= currentOperatorToken.Precedence)
                {
                    PerformEvaluationAction();
                    continue;
                }

                // Handle a comma in a function parameter or a dimension 
                if (token is CommaToken)
                {
                    if (arrayCount.Count > 0)
                    {
                        /* Evaluate the expression back to the opening brace or last comma
                         * to properly handle Fn(x-1, x-2) */
                        FinishEvaluatingExpressionChunk();

                        var count = arrayCount.Pop();
                        arrayCount.Push(count + 1);
                        continue;
                    }
                    else
                    {
                        // Let the caller decide if this is an error
                        tokenMarker.MovePrev();
                        break;
                    }
                }

                // Setup evaluation of a function
                if (token is FunctionToken)
                {
                    if (tokenMarker.Token == null || !(tokenMarker.Token is OpenParenToken))
                    {
                        throw new Exceptions.SyntaxErrorException();
                    }

                    // Add the token to the stack and setup to finish the function evaluation.
                    arrayCount.Push(0); // args count
                    operatorTokens.Push(token);
                    operatorActions.Push((x) => EvaluateFunctionToken(x));
                    continue;
                }

                //Setup evaluation of a user defined function
                if (token is FnToken)
                {
                    if (tokenMarker.Token is WhitespacesToken)
                        tokenMarker.MoveNext();

                    if (tokenMarker.Token == null || !(tokenMarker.Token is VariableNameToken userDefinedFunctionName))
                    {
                        throw new Exceptions.SyntaxErrorException();
                    }

                    tokenMarker.MoveNext();

                    arrayCount.Push(0);
                    operatorTokens.Push(userDefinedFunctionName);
                    operatorActions.Push((x) => EvaluateUserDefinedFunctionToken(x));

                    continue;
                }

                // Push an operator token on to the operator tokens stack
                if (token is OperatorToken)
                {
                    operatorTokens.Push(token);
                    // Setup to finish evaluating an operator.
                    operatorActions.Push((x) => EvaluateBinaryOperatorToken(x));
                    continue;
                }

                // An unrecognized token. Let the caller decide what to do about it, if anything.
                // Move tokenMarker back by one since it had already been bumped up by one
                // above and it should be pointing at the unrecognized character.
                tokenMarker.MovePrev();
                break;

            }

            // Finishing evaluating all the pending operations. FinishPendingOperations()
            while (operatorTokens.TryPeek(out Token lastOperator))
            {
                // Check for non-matching right parenthesis
                if (lastOperator is OpenParenToken)
                {
                    throw new Exceptions.SyntaxErrorException();
                }
                PerformEvaluationAction();
            }

            var finalResult = valueTokens.Pop();
            return finalResult;

        }

        void FinishEvaluatingExpressionChunk()
        {
            // Evaluate operators on the stack until finding an opening parenthesis.
            while (operatorTokens.TryPeek(out Token operatorOrParenToken) &&
                   !(operatorOrParenToken is OpenParenToken))
            {
                PerformEvaluationAction();
            }

        }

        // Pop the operator token and the evaluation lamda and push the result to the values stack.
        void PerformEvaluationAction()
        {
            var operatorToken = operatorTokens.Pop();
            var action = operatorActions.Pop();
            var result = action(operatorToken);
            valueTokens.Push(result);

        }

        // Evaluate a binary operator. Pop two values off the stack and evaluate then.
        ValueToken EvaluateBinaryOperatorToken(Token token)
        {
            var binaryToken = (OperatorToken)token;
            var value2 = valueTokens.Pop();
            var value1 = valueTokens.Pop();

            if (!(value1 is ValueToken) || !(value2 is ValueToken))
                throw new Exception("Token should be a value!");

            if (value1 is StringValueToken && !(value2 is StringValueToken))
            {
                throw new Exceptions.TypeMismatchException();
            }

            return binaryToken.Evaluate(interpreter, value1, value2);

        }

        // Evaluate an unary operator. Pop one value off the stack and evaluate that value.
        ValueToken EvaluateUnaryOperatorToken(Token token)
        {
            var unaryToken = (OperatorToken)token;
            var unaryValue = valueTokens.Pop();
            return unaryToken.Evaluate(interpreter, unaryValue, default);
        }

        // Finish evaluating a function of the form FUNC(param1, ...)
        ValueToken EvaluateFunctionToken(Token token)
        {
            var functionToken = (FunctionToken)token;

            // Pop the arguments off the stack. The number of arguments to pass
            // is stored its own stack.
            var argsCount = arrayCount.Pop();
            var args = new ValueToken[argsCount];

            // Check for the correct number of arguments
            if ((argsCount < functionToken.RequiredArgsCount) || (argsCount > (functionToken.RequiredArgsCount + functionToken.OptionalArgsCount)))
            {
                throw new Exceptions.SyntaxErrorException();
            }

            // Arguments come off the stack in reverse order,
            // so it's easier to populate the array in reverse order too.
            int argsIndex = argsCount;
            while (argsIndex > 0)
            {
                argsIndex -= 1;
                var arg = valueTokens.Pop();
                args[argsIndex] = arg;
            }


            // Type check the arguments
            for (int i = 0; i < argsCount; i++)
            {
                if ((functionToken.ArgumentFlags[i] & FunctionArgumentFlags.Number) != 0)
                {
                    if (!(args[i] is NumericValueToken))
                        throw new Exceptions.TypeMismatchException();
                }
                else // Assume a string expression
                {
                    if (args[i] is NumericValueToken)
                        throw new Exceptions.TypeMismatchException();
                }

            }

            var result = functionToken.Evaluate(interpreter, args);
            return result;

        }

        ValueToken EvaluateUserDefinedFunctionToken(Token token)
        {
            var userDefinedFunctionName = (VariableNameToken)token;

            var argsCount = arrayCount.Pop();
            if (argsCount != 1)
            {
                throw new Exceptions.SyntaxErrorException();
            }

            var argument1 = valueTokens.Pop();
            if (!(argument1 is NumericValueToken))
                throw new Exceptions.TypeMismatchException();

            var userDefinedFunction = interpreterEnvironment.GetUserDefinedFunction(userDefinedFunctionName);
            if (userDefinedFunction == null)
            {
                throw new Exceptions.UndefinedFunctionException();
            }

            return userDefinedFunction.Evaluate(interpreter, new ValueToken[] { argument1 });
        }

        // Convert the indicies portion of an array variable into an array of values.
        ValueTokenArray EvaluateVariableAssignmentToken(Token token)
        {
            var variableNameToken = (VariableNameToken)token;
            var argsCount = arrayCount.Pop();

            var args = new ValueToken[argsCount];

            // The arguments are popped in reverse order,
            // so it's easier to store them in the array in reverse order too.
            while (argsCount > 0)
            {
                argsCount -= 1;
                var arg = valueTokens.Pop();
                args[argsCount] = arg;
            }

            var result = new ValueTokenArray(args);
            return result;
        }

        // Finish evaluating the value of the array variable in the form LET A=B(1)
        ValueToken EvaluateGetArrayVariableToken(Token token)
        {
            var indicies = EvaluateVariableAssignmentToken(token);

            var result = interpreterEnvironment.GetArrayVariableValue((VariableNameToken)token, indicies);
            return result;
        }
    }
}
