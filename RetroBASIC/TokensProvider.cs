using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

using RetroBASIC.Commands;
using RetroBASIC.Operators;
using RetroBASIC.Functions;
using RetroBASIC.Values;
using RetroBASIC.Variables;
using RetroBASIC.Characters;

namespace RetroBASIC
{
    public class TokensProvider
    {

        private Dictionary<string, Token> tokens;
        private Dictionary<ValueTuple<string, VariableValueType>, VariableNameToken> variableNameTokens;

        public TokensProvider()
        {
            tokens = new Dictionary<string, Token>();
            variableNameTokens = new Dictionary<ValueTuple<string, VariableValueType>, VariableNameToken>();
        }

        private void RegisterToken(Token token)
        {
            tokens.Add(token.TokenName, token);
        }

        public void RegisterTokens()
        {
            // Primary Commands
            RegisterToken(new ClrToken());
            RegisterToken(new ContToken());
            RegisterToken(new DataToken());
            RegisterToken(new DefToken());
            RegisterToken(new DimToken());
            RegisterToken(new EndToken());
            RegisterToken(new ForToken());
            RegisterToken(new GetToken());
            RegisterToken(new GoToken());
            RegisterToken(new GosubToken());
            RegisterToken(new GotoToken());
            RegisterToken(new IfToken());
            RegisterToken(new InputToken());
            RegisterToken(new LetToken());
            RegisterToken(new ListToken());
            RegisterToken(new LoadToken());
            RegisterToken(new NewToken());
            RegisterToken(new NextToken());
            RegisterToken(new OnToken());
            RegisterToken(new PeekToken());
            RegisterToken(new PokeToken());
            RegisterToken(new PrintToken());
            RegisterToken(new ReadToken());
            RegisterToken(new RemToken());
            RegisterToken(new RestoreToken());
            RegisterToken(new ReturnToken());
            RegisterToken(new RunToken());
            RegisterToken(new SaveToken());
            RegisterToken(new StopToken());

            // Secondary Commands
            RegisterToken(new TabToken());
            RegisterToken(new ToToken());
            RegisterToken(new FnToken());
//            RegisterToken(new NotToken());
            RegisterToken(new SpcToken());
            RegisterToken(new StepToken());
            RegisterToken(new ThenToken());

            // Operators
            RegisterToken(new AndToken());
            RegisterToken(new DivToken());
            RegisterToken(new EqualToken());
            RegisterToken(new GreaterThanToken());
            RegisterToken(new GreaterThanEqualToken());
            RegisterToken(new LessThanEqualToken());
            RegisterToken(new LessThanToken());
            RegisterToken(new MinusToken());
            RegisterToken(new MultToken());
            RegisterToken(new NotEqualToken());
            RegisterToken(new NotToken());
            RegisterToken(new OrToken());
            RegisterToken(new PlusToken());
            RegisterToken(new PowerToken());

            // Functions
            RegisterToken(new AbsToken());
            RegisterToken(new AscToken());
            RegisterToken(new AtnToken());
            RegisterToken(new ChrDollar());
            RegisterToken(new CosToken());
            RegisterToken(new ExpToken());
            RegisterToken(new FreToken());
            RegisterToken(new IntToken());
            RegisterToken(new LeftDollar());
            RegisterToken(new LenToken());
            RegisterToken(new LogToken());
            RegisterToken(new MidDollarToken());
            RegisterToken(new PosToken());
            RegisterToken(new RightDollarToken());
            RegisterToken(new RndToken());
            RegisterToken(new SinToken());
            RegisterToken(new SgnToken());
            RegisterToken(new SqrToken());
            RegisterToken(new StrDollarToken());
            RegisterToken(new ValToken());


            //Characters
            RegisterToken(new OpenParenToken());
            RegisterToken(new CloseParenToken());
//            RegisterToken(new Token(":", TokenType.Colon));
//            RegisterToken(new Token("$", TokenType.Dollar));
//            RegisterToken(new Token("%", TokenType.Percent));
            RegisterToken(new SemicolonToken());
            RegisterToken(new CommaToken());

            UnaryMinusToken = new UnaryMinusToken();
        }

        public IReadOnlyDictionary<string, Token> Tokens { get { return tokens; } }

        public Operators.UnaryMinusToken UnaryMinusToken { get; private set; }

        public Token GetBuiltinToken(string name)
        {
            return tokens.FirstOrDefault(token => token.Key.Equals(name, StringComparison.OrdinalIgnoreCase)).Value;
        }

        public Token GetBuiltinToken(char name)
        {
            string stringyName = name.ToString();
            return GetBuiltinToken(stringyName);
        }

        public VariableNameToken GetOrCreateVariableNameToken(string name, VariableValueType _variableType)
        {
#if false
            VariableNameToken token = null;
            var key = new ValueTuple<string, VariableValueType>(name, _variableType);

            if (variableNameTokens.TryGetValue(key, out token))
                return token;
#endif

            var token = new VariableNameToken(name, _variableType);

#if false
            variableNameTokens.Add(key, token);
#endif
            return token;
        }

        public CommentToken CreateCommentToken(string comment)
        {
            return new CommentToken(comment);
        }

        static public Values.StringValueToken emptyStringToken = new Values.StringValueToken(string.Empty);
        public StringValueToken CreateStringValueToken(string quotedString)
        {
            if (quotedString.Length == 0)
                return emptyStringToken;

            return new StringValueToken(quotedString);
        }

        public Token CreateUnknownCharacterToken(char value)
        {
            return new Token(value.ToString(), TokenType.Unrecognized);
        }

        public WhitespacesToken CreateWhitespacesToken(int count)
        {
            return new WhitespacesToken(count);
        }

        static public RealValueToken zeroRealValueToken = new RealValueToken(0);
        public RealValueToken CreateRealValueToken(double value)
        {
            if (value == 0)
            {
                return zeroRealValueToken;
            }

            return new RealValueToken(value);
        }

        static public IntegerValueToken zeroIntegerValueToken = new IntegerValueToken(0);
        public IntegerValueToken CreateIntegerValueToken(Int16 value)
        {
            if (value == 0)
            {
                return zeroIntegerValueToken;
            }

            return new Values.IntegerValueToken(value);
        }

        public IntegerValueToken CreateIntegerValueToken(int value)
        {
            if (value < Int16.MinValue || value > Int16.MaxValue)
                throw new Exceptions.IllegalQuantityException();

            return new Values.IntegerValueToken((Int16)value);
        }
    }
}
