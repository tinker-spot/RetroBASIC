using System;
using System.Collections.Generic;
using System.Text;

using RetroBASIC.Values;
using RetroBASIC.Variables;
using RetroBASIC.Functions;

namespace RetroBASIC
{

    public class InterpreterVariables
    {
        Dictionary<string, RealValueToken> realNumberVariables { get; set; }
        Dictionary<string, StringValueToken> stringVariables { get; set; }
        Dictionary<string, IntegerValueToken> integerVariables { get; set; }
        RetroBASIC.Interpreter interpreter;

        Dictionary<string, ArrayVariable<RealValueToken>> realNumberVariableArray { get; set; }
        Dictionary<string, ArrayVariable<StringValueToken>> stringVariableArray { get; set; }
        Dictionary<string, ArrayVariable<IntegerValueToken>> integerVariableArray { get; set; }

        Dictionary<string, Functions.UserDefinedFunction> userDefinedFunctions { get; }

        const int DIMENSION_DEFAULT_SIZE = 10;

        public InterpreterVariables(RetroBASIC.Interpreter _interpreter)
        {
            realNumberVariables = new Dictionary<string, RealValueToken>();
            stringVariables = new Dictionary<string, StringValueToken>();
            integerVariables = new Dictionary<string, IntegerValueToken>();

            realNumberVariableArray = new Dictionary<string, ArrayVariable<RealValueToken>>();
            stringVariableArray = new Dictionary<string, ArrayVariable<StringValueToken>>();
            integerVariableArray = new Dictionary<string, ArrayVariable<IntegerValueToken>>();

            userDefinedFunctions = new Dictionary<string, Functions.UserDefinedFunction>();

            interpreter = _interpreter;
        }

        public void Clear()
        {
            realNumberVariables.Clear();
            stringVariables.Clear();
            integerVariables.Clear();

            realNumberVariableArray.Clear();
            stringVariableArray.Clear();
            integerVariableArray.Clear();

            userDefinedFunctions.Clear();
        }

        public ValueToken GetVariableValue(VariableNameToken variableNameToken)
        {
            switch (variableNameToken.VariableType)
            {
                case VariableValueType.String:
                    return GetStringVariableValue(variableNameToken);

                case VariableValueType.RealNumber:
                    return GetRealNumberVariableValue(variableNameToken);

                case VariableValueType.IntegerNumber:
                    return GetIntegerVariableValue(variableNameToken);

                default:
                    throw new NotImplementedException();
            }
        }

        StringValueToken GetStringVariableValue(VariableNameToken variableNameToken)
        {
            StringValueToken stringToken;

            stringVariables.TryGetValue(variableNameToken.Name, out stringToken);

            // String variables that don't exist return an empty string token
            if (stringToken == null)
            {
                stringToken = interpreter.TokensProvider.CreateStringValueToken(string.Empty);
            }

            return stringToken;
        }

        RealValueToken GetRealNumberVariableValue(VariableNameToken variableNameToken)
        {
            RealValueToken numberToken;
            realNumberVariables.TryGetValue(variableNameToken.Name, out numberToken);

            if (numberToken == null)
            {
                numberToken = interpreter.TokensProvider.CreateRealValueToken(0);
            }

            return numberToken;
        }

        IntegerValueToken GetIntegerVariableValue(VariableNameToken variableNameToken)
        {
            IntegerValueToken numberToken;
            integerVariables.TryGetValue(variableNameToken.Name, out numberToken);

            if (numberToken == null)
            {
                numberToken = interpreter.TokensProvider.CreateIntegerValueToken(0);
            }

            return numberToken;
        }

        public void SetVariableValue(VariableNameToken variableNameToken, ValueToken value)
        {
            switch (variableNameToken.VariableType)
            {
                case VariableValueType.String:
                    SetStringValue(variableNameToken, value);
                    break;

                case VariableValueType.RealNumber:
                    SetRealNumberValue(variableNameToken, value);
                    break;

                case VariableValueType.IntegerNumber:
                    SetIntegerNumberValue(variableNameToken, value);
                    break;
            }
        }

        void SetStringValue(VariableNameToken nameToken, ValueToken valueToken)
        {
            if (valueToken is StringValueToken stringToken)
            {
                stringVariables[nameToken.Name] = stringToken;
                return;
            }

            throw new Exceptions.TypeMismatchException();
        }

        void SetRealNumberValue(VariableNameToken nameToken, ValueToken valueToken)
        {
            if (valueToken is StringValueToken)
                throw new Exceptions.TypeMismatchException();

            var numberToken = (NumericValueToken)valueToken;

            if (numberToken is RealValueToken realToken)
            {
                realNumberVariables[nameToken.Name] = realToken;
            }
            else
            {
                realNumberVariables[nameToken.Name] = new RealValueToken(numberToken.RealValue);
            }
        }

        void SetIntegerNumberValue(VariableNameToken nameToken, ValueToken valueToken)
        {
            if (valueToken is StringValueToken)
                throw new Exceptions.TypeMismatchException();

            var numberToken = (NumericValueToken)valueToken;

            if (numberToken is IntegerValueToken integerToken)
            {
                integerVariables[nameToken.Name] = integerToken;
                return;
            }

            var realToken = (RealValueToken)valueToken;
            IntegerValueToken.CheckValueBounds(realToken.Value);

            integerVariables[nameToken.Name] = new IntegerValueToken((Int16)realToken.Value);
        }

        public void DimensionArrayVariable(VariableNameToken variableNameToken, ValueTokenArray valueTokens)
        {
            var dimensions = ConvertTokensArrayToIntArray(valueTokens, 1);
            CreateArrayVariable(variableNameToken, dimensions);
        }

        int[] ConvertTokensArrayToIntArray(ValueTokenArray valueTokenArray, int adjustAmount = 0)
        {
            var length = valueTokenArray.Values.Length;

            int[] intArray = new int[length];
            for (int i = 0; i < length; i += 1)
            {
                intArray[i] = ((NumericValueToken)(valueTokenArray.Values[i])).IntValue + adjustAmount;
            }

            return intArray;
        }

        int[] ConvertTokensArrayToIntArray(ValueTokenArray valueTokenArray, Func<int, int> convertFunc)
        {
            var length = valueTokenArray.Values.Length;

            int[] intArray = new int[length];
            for (int i = 0; i < length; i += 1)
            {
                var value = ((NumericValueToken)(valueTokenArray.Values[i])).IntValue;
                intArray[i] = convertFunc(value);
            }

            return intArray;
        }

        int[] CopyArray(int[] intArray, int adjustAmount = 0)
        {
            int[] arrayCopy = new int[intArray.Length];
            for (int i = 0; i < intArray.Length; i += 1)
                arrayCopy[i] = intArray[i] + adjustAmount;

            return arrayCopy;
        }

        int[] CopyArray(int[] intArray, Func<int, int> newValFunc)
        {
            int[] arrayCopy = new int[intArray.Length];
            for (int i = 0; i < intArray.Length; i += 1)
                arrayCopy[i] = newValFunc(intArray[i]);

            return arrayCopy;
        }

        void CreateArrayVariable(VariableNameToken variableNameToken, int[] dimensions)
        {
            switch (variableNameToken.VariableType)
            {
                case VariableValueType.String:
                    CreateStringArrayVariable(variableNameToken, dimensions);
                    break;

                case VariableValueType.RealNumber:
                    CreateRealNumberArrayVariable(variableNameToken, dimensions);
                    break;

                case VariableValueType.IntegerNumber:
                    CreateIntegerNumberArrayVariable(variableNameToken, dimensions);
                    break;
            }
        }

        ArrayVariable<StringValueToken> CreateStringArrayVariable(VariableNameToken variableNameToken, int[] dimensions)
        {
            if (stringVariableArray.TryGetValue(variableNameToken.Name, out ArrayVariable<StringValueToken> stringValueToken) == true)
                throw new Exceptions.RedimException();

            var stringArray = new ArrayVariable<StringValueToken>(variableNameToken, dimensions);
            stringVariableArray.Add(variableNameToken.Name, stringArray);

            return stringArray;
        }

        ArrayVariable<RealValueToken> CreateRealNumberArrayVariable(VariableNameToken variableNameToken, int[] dimensions)
        {
            if (realNumberVariableArray.TryGetValue(variableNameToken.Name, out ArrayVariable<RealValueToken> realNumberValueToken) == true)
                throw new Exceptions.RedimException();

            var realValueArray = new ArrayVariable<RealValueToken>(variableNameToken, dimensions);
            realNumberVariableArray.Add(variableNameToken.Name, realValueArray);

            return realValueArray;
        }

        ArrayVariable<IntegerValueToken> CreateIntegerNumberArrayVariable(VariableNameToken variableNameToken, int[] dimensions)
        {
            if (integerVariableArray.TryGetValue(variableNameToken.Name, out ArrayVariable<IntegerValueToken> integerValueToken) == true)
                throw new Exceptions.RedimException();

            var integerValueArray = new ArrayVariable<IntegerValueToken>(variableNameToken, dimensions);
            integerVariableArray.Add(variableNameToken.Name, integerValueArray);

            return integerValueArray;
        }

        void SetArrayVariableValue(VariableNameToken variableNameToken, ValueTokenArray indicies, ValueToken value)
        {
            switch (variableNameToken.VariableType)
            {
                case VariableValueType.String:
                    var stringValueToken = (StringValueToken)value;
                    SetStringArrayVariableValue(variableNameToken, indicies, stringValueToken);
                    break;

                case VariableValueType.RealNumber:
                    if (value is IntegerValueToken integerValueToken)
                    {
                        value = new RealValueToken(integerValueToken.RealValue);
                    }

                    SetRealNumberArrayVariableValue(variableNameToken, indicies, (RealValueToken)value);
                    break;

                case VariableValueType.IntegerNumber:
                    if (value is RealValueToken realValueToken)
                    {
                        IntegerValueToken.CheckValueBounds(realValueToken.RealValue);
                        value = new IntegerValueToken((Int16)realValueToken.IntValue);
                    }
                    SetIntegerNumberArrayVariableValue(variableNameToken, indicies, (IntegerValueToken)value);
                    break;
            }
        }

        int AdjustDimensionForUndimensionedCreate(int dimension)
        {
            if (dimension < Constants.DEFAULT_DIMENSION_SIZE)
                dimension = Constants.DEFAULT_DIMENSION_SIZE;

            dimension += 1;
            return dimension;
        }

        void SetStringArrayVariableValue(VariableNameToken variableNameToken, ValueTokenArray indicies, StringValueToken value)
        {
            if (stringVariableArray.TryGetValue(variableNameToken.Name, out var stringArray) == false)
            {
                stringArray = CreateStringArrayVariable(variableNameToken,
                    ConvertTokensArrayToIntArray(indicies, AdjustDimensionForUndimensionedCreate));
            }

            stringArray.SetValue(ConvertTokensArrayToIntArray(indicies), value);
        }

        void SetRealNumberArrayVariableValue(VariableNameToken variableNameToken, ValueTokenArray indicies, RealValueToken value)
        {
            if (realNumberVariableArray.TryGetValue(variableNameToken.Name, out var numberArray) == false)
            {
                numberArray = CreateRealNumberArrayVariable(variableNameToken,
                    ConvertTokensArrayToIntArray(indicies, AdjustDimensionForUndimensionedCreate));
            }

            numberArray.SetValue(ConvertTokensArrayToIntArray(indicies), value);
        }

        void SetIntegerNumberArrayVariableValue(VariableNameToken variableNameToken, ValueTokenArray indicies, IntegerValueToken value)
        {
            if (integerVariableArray.TryGetValue(variableNameToken.Name, out var numberArray) == false)
            {
                numberArray = CreateIntegerNumberArrayVariable(variableNameToken,
                    ConvertTokensArrayToIntArray(indicies, AdjustDimensionForUndimensionedCreate));
            }

            numberArray.SetValue(ConvertTokensArrayToIntArray(indicies), value);
        }

        public ValueToken GetArrayVariableValue(VariableNameToken variableNameToken, ValueTokenArray indicies)
        {
            switch (variableNameToken.VariableType)
            {
                case VariableValueType.String:
                    return GetStringArrayValue(variableNameToken, indicies);

                case VariableValueType.RealNumber:
                    return GetRealArrayValue(variableNameToken, indicies);

                case VariableValueType.IntegerNumber:
                    return GetIntegerArrayValue(variableNameToken, indicies);

                default:
                    throw new NotImplementedException();
            }
        }

        ValueToken GetStringArrayValue(VariableNameToken variableNameToken, ValueTokenArray indicies)
        {
            if (stringVariableArray.TryGetValue(variableNameToken.Name, out var stringArray) == false)
            {
                stringArray = CreateStringArrayVariable(variableNameToken,
                    ConvertTokensArrayToIntArray(indicies, AdjustDimensionForUndimensionedCreate));
            }

            var result = stringArray.GetValue(ConvertTokensArrayToIntArray(indicies));

            if (result == null)
                result = interpreter.TokensProvider.CreateStringValueToken(string.Empty);

            return result;
        }

        ValueToken GetRealArrayValue(VariableNameToken variableNameToken, ValueTokenArray indicies)
        {
            if (realNumberVariableArray.TryGetValue(variableNameToken.Name, out var numberArray) == false)
            {
                numberArray = CreateRealNumberArrayVariable(variableNameToken,
                    ConvertTokensArrayToIntArray(indicies, AdjustDimensionForUndimensionedCreate));
            }
 
            var result = numberArray.GetValue(ConvertTokensArrayToIntArray(indicies));

            if (result == null)
                result = interpreter.TokensProvider.CreateRealValueToken(0);

            return result;
        }

        ValueToken GetIntegerArrayValue(VariableNameToken variableNameToken, ValueTokenArray indicies)
        {
            if (integerVariableArray.TryGetValue(variableNameToken.Name, out var numberArray) == false)
            {
                numberArray = CreateIntegerNumberArrayVariable(variableNameToken,
                    ConvertTokensArrayToIntArray(indicies, AdjustDimensionForUndimensionedCreate));
            }

            var result = numberArray.GetValue(ConvertTokensArrayToIntArray(indicies));

            if (result == null)
                result = interpreter.TokensProvider.CreateIntegerValueToken(0);

            return result;
        }

        public void CreateUserDefinedFunction(VariableNameToken functionName, UserDefinedFunction function)
        {
            // A second definition will happily replace any previous one.
            userDefinedFunctions[functionName.Name] = function;
        }

        public UserDefinedFunction GetUserDefinedFunction(VariableNameToken functionName)
        {
            UserDefinedFunction function = null;

            userDefinedFunctions.TryGetValue(functionName.Name, out function);
            return function;
        }
        public void SetVariableValue(VariableNameToken variableNameToken, ValueTokenArray indicies, ValueToken value)
        {
            if (indicies == null)
                SetVariableValue(variableNameToken, value);
            else
                SetArrayVariableValue(variableNameToken, indicies, value);
        }

    }

}