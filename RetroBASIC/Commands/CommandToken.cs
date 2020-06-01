using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RetroBASIC.Commands
{
    public enum CommandTokenType
    {
        Primary,
        Secondary,
        Comment,
    };

    public class CommandToken : Token
    {
        public CommandToken(string name, CommandTokenType commandType) : base(name, TokenType.Command)
        {
            CommandType = commandType;
        }
        public CommandTokenType CommandType { get; }

        public override void DumpTokenContents(TextWriter tw)
        {
            tw.Write("Command: ");
            tw.Write(TokenName);
            tw.Write(" , ");

            switch (this.CommandType)
            {
                case CommandTokenType.Primary:
                    tw.Write("Primary");
                    break;

                case CommandTokenType.Secondary:
                    tw.Write("Secondary");
                    break;

                case CommandTokenType.Comment:
                    tw.Write("Comment (and Primary)");
                    break;
            }

        }

        public virtual void Execute(RetroBASIC.Interpreter interpreter, TokenMarker tokenMarker)
        {
            throw new NotImplementedException();
        }
    }
}
