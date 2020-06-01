using System;
using System.Collections.Generic;
using System.Text;

using RetroBASIC.Values;
using RetroBASIC.Variables;
using RetroBASIC.Commands;
using RetroBASIC.Operators;

namespace RetroBASIC.Commands
{
    public class ForToken : CommandToken
    {
        public ForToken() : base("FOR", CommandTokenType.Primary)
        {

        }

        public override void Execute(Interpreter interpreter, TokenMarker tokenMarker)
        {
            if (!(tokenMarker.Token is VariableNameToken variableNameToken))
                throw new Exceptions.SyntaxErrorException();

            if (variableNameToken.VariableType != VariableValueType.RealNumber)
                throw new Exceptions.SyntaxErrorException();

            if (!(tokenMarker.GetNextToken() is EqualToken))
                throw new Exceptions.SyntaxErrorException();

            tokenMarker.MoveNext();

            NumericValueToken startValue;
            try
            {
                startValue = (NumericValueToken)interpreter.ExpressionEvaluator.Evaluate(tokenMarker);
            }
            catch (InvalidCastException)
            {
                throw new Exceptions.TypeMismatchException();
            }

            if (!(tokenMarker.Token is ToToken))
                throw new Exceptions.SyntaxErrorException();

            tokenMarker.MoveNext();

            NumericValueToken endValue;
            try
            {
                endValue = (NumericValueToken)interpreter.ExpressionEvaluator.Evaluate(tokenMarker);
            }
            catch (InvalidCastException)
            {
                throw new Exceptions.TypeMismatchException();
            }

            NumericValueToken stepValueToken = null;

            if (tokenMarker.Token != null)
            {
                if (!(tokenMarker.Token is StepToken))
                    throw new Exceptions.SyntaxErrorException();

                tokenMarker.MoveNext();
                var result = interpreter.ExpressionEvaluator.Evaluate(tokenMarker);
                if (!(result is NumericValueToken))
                    throw new Exceptions.TypeMismatchException();
                stepValueToken = (NumericValueToken)result;
            }
            else
            {
                // Default step is 1 or -1
//                if (startValue.RealValue <= endValue.RealValue)
                    stepValueToken = interpreter.TokensProvider.CreateRealValueToken(1);
//                else
//                    stepValueToken = interpreter.TokensProvider.CreateRealValueToken(-1);
            }

            ForNextLoopState forNextLoopState;
            if (interpreter.InterpreterMode == InterpreterMode.Running)
            {
                var forNextMarker = interpreter.CopyCurrentStatementMarker();

                // In immediate mode, FOR may not neccessarily be followed by a statement.
                if (forNextMarker.Valid)
                    forNextMarker.MoveToNextStatement();

                forNextLoopState = new ForNextLoopState(forNextMarker, variableNameToken, startValue, endValue, stepValueToken);
            }
            else
            {
                var index = interpreter.CurrentImmediateModeStatementMarker.StatementIndex;
                forNextLoopState = new ForNextLoopState(index, variableNameToken, startValue, endValue, stepValueToken);
            }

            interpreter.ForNextLoops.Push(forNextLoopState);
            interpreter.VariablesEnvironment.SetVariableValue(variableNameToken, startValue);
        }
    }
}
