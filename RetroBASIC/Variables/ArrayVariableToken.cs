using System;
using System.Collections.Generic;
using System.Text;

using RetroBASIC.Values;

namespace RetroBASIC.Variables
{
    public class ArrayVariable<T>
        where T : ValueToken
    {
        public VariableNameToken VariableName { get; }
        public int[] Dimensions { get; }

        int[] multipliers;

        T[] valueTokens;

        public ArrayVariable(VariableNameToken variableName, int[] dimensions)
        {
            VariableName = variableName;
            Dimensions = dimensions;

            SetMultipliers();
            var arraySize = multipliers[0] * Dimensions[0];
            valueTokens = new T[arraySize];
        }

        public T GetValue(int[] indicies)
        {
            CheckIndiciesForErrors(indicies);

            var index = GetIndexFromIndicies(indicies);

            var result = valueTokens[index];
            return result;
        }

        public void SetValue(int[] indicies, T valueToken)
        {
            CheckIndiciesForErrors(indicies);

            var index = GetIndexFromIndicies(indicies);

            valueTokens[index] = valueToken;
        }

        void CheckIndiciesForErrors(int[] indicies)
        {
            if (indicies.Length != Dimensions.Length)
                throw new Exceptions.BadSubscriptException();

            for (int i = 0; i < indicies.Length; i++)
            {
                if (indicies[i] < 0 || indicies[i] >= Dimensions[i])
                {
                    throw new Exceptions.BadSubscriptException();
                }
            }
        }
        int GetIndexFromIndicies(int[] indicies)
        {
            int index = 0;
            for (int i = 0; i < indicies.Length; i++)
            {
                index += (indicies[i] * multipliers[i]);
            }

            return index;
        }

        void SetMultipliers()
        {
            multipliers = new int[Dimensions.Length];

            multipliers[Dimensions.Length - 1] = 1;

            if (Dimensions.Length >= 2)
            {
                for (int i = Dimensions.Length - 2; i >= 0; i--)
                {
                    multipliers[i] = multipliers[i + 1] * Dimensions[i + 1];
                }
            }
        }

    }
}
