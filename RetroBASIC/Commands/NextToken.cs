using System;
using System.Collections.Generic;
using System.Text;

using RetroBASIC.Values;
using RetroBASIC.Variables;
using RetroBASIC.Characters;

namespace RetroBASIC.Commands
{
    public class NextToken : CommandToken
    {
        public NextToken() : base("NEXT", CommandTokenType.Secondary)
        {

        }

        public override void Execute(Interpreter interpreter, TokenMarker tokenMarker)
        {
            var variableNameTokens = new List<VariableNameToken>();
            while (tokenMarker.Token != null)
            {
                if (!(tokenMarker.Token is VariableNameToken variableNameToken))
                    throw new Exceptions.SyntaxErrorException();

                if (!((variableNameToken.VariableType == VariableValueType.IntegerNumber) ||
                    (variableNameToken.VariableType == VariableValueType.RealNumber)))
                    throw new Exceptions.TypeMismatchException();

                variableNameTokens.Add(variableNameToken);
                
                // Maybe add more variable names to the list
                if (tokenMarker.GetNextToken() is CommaToken)
                {
                    tokenMarker.MoveNext();
                    continue;
                }
            }

            if (interpreter.ForNextLoops.Count == 0)
                throw new Exceptions.NextWithoutForException();

            if (variableNameTokens.Count == 0)
            {
                ProcessForNextState(interpreter, interpreter.ForNextLoops.Peek());
                return;
            }

            int variableNameIndex = 0;
            while (variableNameIndex < variableNameTokens.Count)
            {
                if (interpreter.ForNextLoops.TryPeek(out ForNextLoopState forNextState) == false)
                    throw new Exceptions.NextWithoutForException();

                bool forNextLoopDone = ProcessForNextState(interpreter, forNextState, variableNameTokens[variableNameIndex]);
                if (!forNextLoopDone)
                    return;

                variableNameIndex += 1;
            }
        }

        bool ProcessForNextState(Interpreter interpreter, ForNextLoopState forNextState, VariableNameToken variableNameToken = null)
        {
            // For NEXT with a variable name, check the variable name matches the one stored in the stack
//            if (variableNameToken != null && (forNextState.VariableNameToken.Name != variableNameToken.Name))
//                throw new Exceptions.NextWithoutForException();

            if (variableNameToken != null && (forNextState.VariableNameToken.Name != variableNameToken.Name))
            {
                /* Get Calendar from David Ahl's BASIC Computer Games to work without producing
                 * a NEXT WITHOUT FOR error. If the variable in NEXT [x] is not on top of the stack
                 * like it should be, then pop variables off the stack until a name match is found.
                 * If still no match, only then produce a NEXT WITHOUT FOR error. This supports the case
                 * where the program GOTOs out of an inner loop to an outer loop.
                 */
                interpreter.ForNextLoops.Pop();

                while (interpreter.ForNextLoops.Count > 0)
                {
                    forNextState = interpreter.ForNextLoops.Peek();
                    if (forNextState.VariableNameToken.Name == variableNameToken.Name)
                    {
                        break;
                    }

                    // Discard this loop and try matching the next one
                    interpreter.ForNextLoops.Pop();
                }

                if (interpreter.ForNextLoops.Count == 0)
                    throw new Exceptions.NextWithoutForException();
            }

            var newValue = forNextState.CurrentValue.RealValue + forNextState.StepValue.RealValue;

            bool forNextLoopIsDone = false;

            if (forNextState.StepValue.RealValue >= 0)
            {
                forNextLoopIsDone = (newValue > forNextState.EndValue.RealValue);
            }
            else
            {
                forNextLoopIsDone = (newValue < forNextState.EndValue.RealValue);
            }

            if (forNextLoopIsDone)
            {
                interpreter.ForNextLoops.Pop();
                return true;
            }
#if false
            // If done, pop the stack and return true
            if (forNextState.StartValue.RealValue <= forNextState.EndValue.RealValue)
            {
                if (newValue > forNextState.EndValue.RealValue)
                {
                    interpreter.ForNextLoops.Pop();
                    return true;
                }
            }
            else
            {
                var endValue = forNextState.EndValue.RealValue;
                bool isLessThan = (newValue < endValue);

                if (newValue > forNextState.EndValue.RealValue)
                {
                    interpreter.ForNextLoops.Pop();
                    return true;
                }
            }
#endif
            // Keep going, set the NEXT variable and goto the first statement after the FOR
            forNextState.CurrentValue = interpreter.TokensProvider.CreateRealValueToken(newValue);
            interpreter.VariablesEnvironment.SetVariableValue(forNextState.VariableNameToken, forNextState.CurrentValue);

            if (forNextState.LoopMarker != null)
            {
                // If in immediate mode, there may be no valid statement to loop to.
                if (forNextState.LoopMarker.Valid)
                {
                    var forNextStatement = new StatementMarker(forNextState.LoopMarker);
                    interpreter.NextStatementMarker = forNextStatement;
                }
            }
            else
            {
                var index = forNextState.ImmediateModeStatementIndex;
                interpreter.CurrentImmediateModeStatementMarker.MoveTo(index);
            }

            // Not done
            return false;
        }
    }
}
