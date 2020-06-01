using System;
using System.Collections.Generic;
using System.Text;

namespace RetroBASIC
{
    public class TokenMarker
    {
        private IReadOnlyList<Token> tokens;
        private int index;

        public TokenMarker(IReadOnlyList<Token> _tokens)
        {
            tokens = _tokens;
            index = 0;
            Token = tokens[index];
        }

        public Token Token { get; private set; }

        public Token GetNextToken()
        {
            MoveNext();
            return Token;
        }

        public void MoveNext()
        {
            if (Token == null)
                return;

            index += 1;

            try
            {
                if (tokens[index] is WhitespacesToken)
                    index += 1;
            }
            catch (ArgumentOutOfRangeException)
            {
                Token = null;
            }

            try
            {
                Token = tokens[index];
            }
            catch (ArgumentOutOfRangeException)
            {
                Token = null;
            }
        }

        public Token PeekNext()
        {
            if (Token == null)
                return null;

            try
            {
                return tokens[index + 1];
            }
            catch (ArgumentOutOfRangeException)
            {
                return null;
            }
        }

        public Token PeekPrev()
        {
            try
            {
                var result = tokens[index - 1];
                if (result is WhitespacesToken)
                    index -= 1;
                return tokens[index - 1];
            }
            catch (ArgumentOutOfRangeException)
            {
                return null;
            }
        }
        
        public void MovePrev()
        {
            if (index == 0)
                throw new IndexOutOfRangeException();

            index -= 1;
            if (tokens[index] is WhitespacesToken)
                index -= 1;

            Token = tokens[index];
        }
    }
}
