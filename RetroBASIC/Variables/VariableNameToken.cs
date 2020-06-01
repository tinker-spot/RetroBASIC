using System;
using System.Collections.Generic;
using System.IO;
using System.Text; 

namespace RetroBASIC.Variables
{
    public class VariableNameToken : Token
    {
        public string Name { get; }
        public string FullName { get; }

        public VariableValueType VariableType { get; }

        public VariableNameToken(string name, VariableValueType _variableType) : base("Variable", TokenType.VariableName)
        {
            name = name.ToUpper();
            FullName = name;

            // In old 6502 BASICs, only the first two characters were significant
            Name = (name.Length < 2) ? name : name.Substring(0, 2);

            VariableType = _variableType;

        }

        public override void DumpTokenContents(TextWriter tw)
        {
            tw.Write("Name: ");
            tw.Write(Name);
            if (Name.Length != FullName.Length)
            {
                tw.Write(" FullName: ");
                tw.Write(FullName);
            }

            tw.Write(" ");
            tw.Write(VariableType.ToString());
        }
    }
}
