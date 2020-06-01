using System;
using System.Collections.Generic;
using System.Text;

using RetroBASIC.Values;
using RetroBASIC.Variables;

namespace RetroBASIC
{
    public class ForNextLoopState
    {
        public ForNextLoopState(StatementMarker startMarker, VariableNameToken _variableNameToken, NumericValueToken _startValue,
            NumericValueToken _endValue, NumericValueToken _stepValue)
        {
            LoopMarker = startMarker;
            VariableNameToken = _variableNameToken;
            StartValue = _startValue;
            EndValue = _endValue;
            StepValue = _stepValue;
            CurrentValue = StartValue;

            ImmediateModeStatementIndex = -1;
        }

        public ForNextLoopState(int _immediateModeStatementIndex, VariableNameToken _variableNameToken, NumericValueToken _startValue,
            NumericValueToken _endValue, NumericValueToken _stepValue)
        {
            ImmediateModeStatementIndex = _immediateModeStatementIndex;
            VariableNameToken = _variableNameToken;
            StartValue = _startValue;
            EndValue = _endValue;
            StepValue = _stepValue;
            CurrentValue = StartValue;

            LoopMarker = null;
        }

        public StatementMarker LoopMarker { get; }

        public int ImmediateModeStatementIndex { get; }

        public VariableNameToken VariableNameToken { get; }

        public NumericValueToken StartValue { get; }

        public NumericValueToken EndValue { get; }

        public NumericValueToken StepValue { get; }

        public NumericValueToken CurrentValue { get; set; }
    }
}
