using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using RetroBASIC.Values;

namespace RetroBASIC.Operators
{
    public enum OperatorTokenType
    {
        Equals,
        Plus,
        Minus,
        Unary,
        Multiply,
        Divide,
        Power,
        GreaterThan,
        GreaterThanEqual,
        LesserThan,
        LesserThanEqual,
        NotEqual,
        Not,
        And,
        Or
    }

    // Based on table found here: https://www.c64-wiki.com/wiki/Operator
    public enum OperatorPrecedenceLevel
    {
        LogicalOr = 11,
        LogicalAnd = 12,
        LogicalNegation = 13,
        Comparison = 14,
        AddSub = 15,
        MulDiv = 16,
        Unary = 17,
        Power = 18,
    }

    public class OperatorToken : Token
    {
        public OperatorPrecedenceLevel Precedence { get; }
        public OperatorTokenType OperatorType { get; }

        public OperatorToken(string tokenName, OperatorTokenType operatorType, OperatorPrecedenceLevel precedence) : base(tokenName, TokenType.Operator)
        {
            OperatorType = operatorType;
            Precedence = precedence;
        }

        public override void DumpTokenContents(TextWriter tw)
        {
            tw.Write("Operator: ");
            tw.Write(TokenName);
            tw.Write(" Precedence: ");
            tw.Write(Precedence);
        }

        public virtual ValueToken Evaluate(RetroBASIC.Interpreter interpreter, ValueToken item1, ValueToken item2)
        {
            throw new NotImplementedException();
        }
    }
}
