using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;

using RetroBASIC.Operators;
using RetroBASIC.Exceptions;
using RetroBASIC.Values;
using RetroBASIC.Functions;
using RetroBASIC.Variables;
using RetroBASIC.Console;
using RetroBASIC.Commands;
using RetroBASIC.Characters;

namespace RetroBASIC
{
    public enum InterpreterMode
    {
        Immmediate,
        Running
    }

    public class Interpreter
    {
        public SortedList<int, Line> Lines { get; }
        private Tokenizer tokenizer;

        internal StatementMarker CurrentStatementMarker { get; private set; }
        internal ImmediateModeStatementMarker CurrentImmediateModeStatementMarker { get; private set; }

        public bool ContinueRunning { get; set; }

        public InterpreterVariables VariablesEnvironment { get; }
        public TokensProvider TokensProvider { get; }
        public ExpressionEvaluator ExpressionEvaluator { get; }

        public IConsole Console { get; set; }

        public Func<string> GetProgramDirectory { get; set; }

        public CommandToken CurrentCommandToken { get; private set; }

        public Statement CurrentStatement { get { return CurrentStatementMarker.Statement; } }

        public StatementMarker NextStatementMarker { get; set; }

        public StatementMarker DataStatementMarker { get; private set; }

        public TokenMarker DataTokenMarker { get; private set; }

        public Stack<StatementMarker> GosubMarkers { get; }

        public Stack<ForNextLoopState> ForNextLoops { get; }

        public InterpreterMode InterpreterMode { get; private set; }

        public Interpreter(IConsole console = null)
        {
            TokensProvider = new TokensProvider();
            TokensProvider.RegisterTokens();
            tokenizer = new Tokenizer(TokensProvider);
            Lines = new SortedList<int, Line>();
            VariablesEnvironment = new InterpreterVariables(this);
            ExpressionEvaluator = new ExpressionEvaluator(this);

            CurrentStatementMarker = new StatementMarker(Lines);
            CurrentImmediateModeStatementMarker = null;

            NextStatementMarker = null;
            DataStatementMarker = new StatementMarker(Lines);
            DataTokenMarker = null;

            GosubMarkers = new Stack<StatementMarker>();
            ForNextLoops = new Stack<ForNextLoopState>();

            ContinueRunning = false;
            InterpreterMode = InterpreterMode.Immmediate;

            Console = console;
        }

        public void Run(int lineNumber = -1)
        {
            int startIndex = 0;
            if (lineNumber != -1)
            {
                startIndex = Lines.IndexOfKey(lineNumber);
                if (startIndex == -1)
                {
                    throw new NotImplementedException();
                }
            }
            RunFromLinesIndex(startIndex);
        }

        public void RunFromLinesIndex(int startIndex = 0)
        {
            VariablesEnvironment.Clear();
            GosubMarkers.Clear();
            ForNextLoops.Clear();
            ExpressionEvaluator.Clear();
            DataStatementMarker.MoveTo(0, 0);
            NextStatementMarker = null;
            DataTokenMarker = null;

            WarmRunFromIndex(startIndex);
        }

        public void WarmRun(int lineNumber = -1)
        {
            int startIndex = 0;
            if (lineNumber != -1)
            {
                startIndex = Lines.IndexOfKey(lineNumber);
                if (startIndex == -1)
                {
                    throw new NotImplementedException();
                }
            }
            WarmRunFromIndex(startIndex);
        }

        public void WarmRunFromIndex(int startIndex = 0)
        {
            CurrentStatementMarker.MoveTo(startIndex, 0);
            InterpetLines();
        }

        public void ContinueRun()
        {
            if (CurrentStatementMarker != null && CurrentStatementMarker.Valid)
                InterpetLines();
        }

        public void List(int startLine = -1, int endLine = int.MaxValue)
        {
            DetokenizeLines(Console.OutputTextWriter, startLine, endLine);
        }

        public void ClearAll()
        {
            Lines.Clear();
            VariablesEnvironment.Clear();
            ExpressionEvaluator.Clear();
            GosubMarkers.Clear();
            ForNextLoops.Clear();
            DataStatementMarker.MoveTo(0, 0);
            DataTokenMarker = null;

        }

        public void Load(IEnumerable<string> programLines)
        {
            Lines.Clear();
            foreach (var line in programLines)
            {
                // Ignore direct mode commands
                ParseLine(line);
            }
        }

        public void LoadFromFile(string fileName)
        {
            string originalFileName = fileName;

            if (GetProgramDirectory != null)
            {
                string programDir = GetProgramDirectory();
                if (programDir.Length > 0)
                {
                    fileName = Path.Combine(programDir, fileName);
                }
            }

            if (this.InterpreterMode == InterpreterMode.Immmediate)
                Console.OutputTextWriter.WriteLine($"Loading \"{originalFileName}\"...");

            try
            {
                var enumLines = File.ReadLines(fileName);
                Load(enumLines);
            }
            catch (FileNotFoundException)
            {
                Console.OutputTextWriter.WriteLine("? FILE NOT FOUND ERROR");
            }
        }

        public void SaveToFile(string fileName)
        {
            using (var writer = new StreamWriter(fileName, false, Encoding.ASCII))
            {
                DumpLines(writer);
            }
        }

        public void EnterLine(string programLine)
        {
            //            (_, _) = ParseLine(programLine);
            (int lineNumer, List<Statement> tokenizedLine) = ParseLine(programLine);
            if (lineNumer == -1)
                InterpretLine(tokenizedLine);

        }

        public (int lineNumber, List<Statement> statements) ParseLine(string programLine)
        {
            (int lineNumber, List<Statement> tokenizedLine) = tokenizer.TokenizeLine(programLine);
            if (lineNumber >= 0)
            {
                Lines[lineNumber] = new Line(lineNumber, tokenizedLine);
            }

            return (lineNumber, tokenizedLine);
        }

        public void DumpLines(TextWriter writer, int startLine = -1, int endLine = int.MaxValue)
        {
            var lineStatements = from lineTuple in Lines
                                 where lineTuple.Key >= startLine && lineTuple.Key <= endLine
                                 select lineTuple.Value;

            foreach (var lineStatement in lineStatements)
            {
                DumpLine(writer, lineStatement);
            }
        }

        public void DumpLine(TextWriter writer, Line line)
        {
            writer.Write(line.LineNumber.ToString("0 "));

            int countDown = line.Statements.Count - 1;

            foreach (var statement in line.Statements)
            {
                foreach (var token in statement.Tokens)
                {
                    token.DumpToken(writer);
                }

                countDown -= 1;
                if (countDown > 0)
                {
                    writer.Write(":");
                }
            }
            writer.WriteLine();
        }

        public void DetokenizeLines(TextWriter writer, int startLine = -1, int endLine = int.MaxValue)
        {
            var linesToDetokenize = from lineTuple in Lines
                                    where lineTuple.Key >= startLine && lineTuple.Key <= endLine
                                    select lineTuple.Value;

            foreach (var lineToDetokenize in linesToDetokenize)
            {
                DetokenizeLine(writer, lineToDetokenize);
            }
        }

        public void DetokenizeLine(TextWriter writer, Line line)
        {
            writer.Write(line.LineNumber);
            writer.Write(" ");
            DetokenizeStatements(writer, line.Statements);
            writer.WriteLine();
        }

        public void DetokenizeStatements(TextWriter writer, IReadOnlyList<Statement> statements)
        {
            int countDown = statements.Count;

            foreach (var statement in statements)
            {
                DetokenizeStatement(writer, statement);
                countDown -= 1;
                if (countDown > 0)
                {
                    writer.Write(":");
                }
            }
            
        }

        public void DetokenizeStatement(TextWriter writer, Statement statement)
        {
            foreach (var token in statement.Tokens)
            {
                switch (token)
                {
                    case Commands.CommandToken commandToken:
                        writer.Write(commandToken.TokenName);
                        break;

                    case FunctionToken funcToken:
                        writer.Write(funcToken.TokenName);
                        break;

                    case OperatorToken operatorToken:
                        writer.Write(operatorToken.TokenName);
                        break;

                    case VariableNameToken variableNameToken:
                        writer.Write(variableNameToken.FullName);
                        switch (variableNameToken.VariableType)
                        {
                            case VariableValueType.String:
                                writer.Write('$');
                                break;

                            case VariableValueType.IntegerNumber:
                                writer.Write('%');
                                break;
                        }
                        break;

                    case StringValueToken stringConstantToken:
                        writer.Write($"\"{stringConstantToken.Value}\"");
                        break;

                    case CommentToken commentToken:
                        writer.Write(commentToken.Value);
                        break;

                    case RealValueToken realNumberToken:
                        writer.Write(realNumberToken.Value);
                        break;

                    case IntegerValueToken integerNumberToken:
                        writer.Write(integerNumberToken.Value);
                        break;

                    case WhitespacesToken whiteSpaceToken:
                        writer.Write(" ");
                        break;

                    default:
                        writer.Write(token.TokenName);
                        break;
                }
            }
        }

        protected void InterpetLines()
        {
            InterpreterMode = InterpreterMode.Running;
            ContinueRunning = true;

            try
            {
                while (ContinueRunning)
                 {
                    var currentStatement = CurrentStatementMarker.Statement;
                    if (currentStatement == null)
                    {
                        break;
                    }

                    var commandToken = currentStatement.Tokens[0] as Commands.CommandToken;
                    var isVariableAssignment = (currentStatement.Tokens[0] as VariableNameToken) != null;

                    if (commandToken == null && !isVariableAssignment)
                    {
                        throw new SyntaxErrorException();
                    }

                    CurrentCommandToken = commandToken;

                    TokenMarker marker = new TokenMarker(currentStatement.Tokens);
                    if (isVariableAssignment)
                    {
                        commandToken = TokensProvider.GetBuiltinToken("LET") as Commands.CommandToken;
                    }
                    else
                    {
                        marker.MoveNext();
                    }
#if DEBUG
                    if (CurrentStatementMarker.LineNumber == 630)
                    {
                        CurrentStatementMarker = CurrentStatementMarker;
                    }

                    if (CurrentStatementMarker.LineNumber == 770)
                    {
                        CurrentStatementMarker = CurrentStatementMarker;
                    }
#endif

                    commandToken.Execute(this, marker);

                    if (!ContinueRunning)
                        break;

                    CurrentCommandToken = null;

                    if (NextStatementMarker != null)
                    {
                        CurrentStatementMarker.MoveTo(NextStatementMarker);
                        NextStatementMarker = null;
                    }
                    else
                    {
                        CurrentStatementMarker.MoveToNextStatement();
                    }

                    if (CurrentStatementMarker.Valid == false)
                    {
                        ContinueRunning = false;
                    }
                }
            }
            catch (RetroBASIC.Exceptions.RetroBASICException basicException)
            {
                string outputMessage = $"{ErrorMessages.errorStart} {basicException.Message}{ErrorMessages.error}{ErrorMessages.inMsg}{CurrentStatementMarker.LineNumber}";
                Console.OutputTextWriter.WriteLine();
                Console.OutputTextWriter.WriteLine(outputMessage);
            }
            finally
            {
                ContinueRunning = false;
                InterpreterMode = InterpreterMode.Immmediate;
            }
        }

        public void InterpretLine(List<Statement> statement)
        {
            if (InterpreterMode == InterpreterMode.Running)
                return;

            ContinueRunning = true;
            CurrentImmediateModeStatementMarker = new ImmediateModeStatementMarker(statement);

            try
            {
                while (CurrentImmediateModeStatementMarker.Valid && CurrentImmediateModeStatementMarker.StatementIndex < statement.Count)
                {
                    var currentStatement = CurrentImmediateModeStatementMarker.Statement;
                    var commandToken = currentStatement.Tokens[0] as Commands.CommandToken;
                    var isVariableAssignment = currentStatement.Tokens[0] is VariableNameToken;

                    if (commandToken == null && !isVariableAssignment)
                    {
                        throw new SyntaxErrorException();
                    }

                    CurrentCommandToken = commandToken;

                    TokenMarker marker = new TokenMarker(currentStatement.Tokens);
                    if (isVariableAssignment)
                    {
                        commandToken = (CommandToken)TokensProvider.GetBuiltinToken("LET");
                    }
                    else
                    {
                        marker.MoveNext();
                    }

                    commandToken.Execute(this, marker);

                    CurrentImmediateModeStatementMarker.MoveToNextStatement();
                }
            }
            catch (RetroBASIC.Exceptions.RetroBASICException basicException)
            {
                ExpressionEvaluator.Clear();
                string outputMessage = $"{ErrorMessages.errorStart} {basicException.Message}{ErrorMessages.error}";
                Console.OutputTextWriter.WriteLine();
                Console.OutputTextWriter.WriteLine(outputMessage);
            }
            finally
            {
                ContinueRunning = false;
                CurrentImmediateModeStatementMarker = null;
                InterpreterMode = InterpreterMode.Immmediate;
            }
        }

        void ErrorMessageInLineNumber(string message)
        {
            string errorMessage = String.Format("{0}{1}{2}{3}{4}", ErrorMessages.errorStart, message, ErrorMessages.error, ErrorMessages.inMsg, CurrentStatementMarker.LineNumber.ToString());
            Console.OutputTextWriter.WriteLine(errorMessage);
        }

        public StatementMarker CreateStatementMarker()
        {
            return new StatementMarker(Lines);
        }

        public StatementMarker CopyCurrentStatementMarker()
        {
            return new StatementMarker(CurrentStatementMarker);
        }

        public ValueToken ReadNextDataValue()
        {
            if (DataTokenMarker == null)
            {
                FindDataStatement();
                if (DataStatementMarker.Valid == false)
                    return null;

                DataTokenMarker = new TokenMarker(DataStatementMarker.Statement.Tokens);
                DataTokenMarker.MoveNext();
            }

            if (DataTokenMarker.Token == null)
            {
                while (DataTokenMarker.Token == null)
                {
                    DataStatementMarker.MoveToNextStatement();
                    FindDataStatement();

                    if (DataStatementMarker.Valid == false)
                        return null;

                    DataTokenMarker = new TokenMarker(DataStatementMarker.Statement.Tokens);
                    DataTokenMarker.MoveNext();
                }
            }

            var result = (ValueToken)DataTokenMarker.Token;

            DataTokenMarker.MoveNext();
            if (DataTokenMarker.Token is CommaToken)
                DataTokenMarker.MoveNext();

            return result;
        }

        void FindDataStatement()
        {
            while (DataStatementMarker.Valid && !(DataStatementMarker.Statement.Tokens[0] is DataToken))
            {
                DataStatementMarker.MoveToNextStatement();
            }
        }

        public void BreakDetected()
        {
            // Bail out if break already detected
            if (ContinueRunning == false)
                return;

            ContinueRunning = false;

            if (this.Console.CursorColumn != 0)
                Console.OutputTextWriter.WriteLine();

            if (InterpreterMode == InterpreterMode.Running)
            {
                var lineNumber = CurrentStatementMarker.LineNumber;
                Console.OutputTextWriter.WriteLine($"BREAK IN {lineNumber}");
            }
            else
            {
                Console.OutputTextWriter.WriteLine("BREAK");
            }
        }

    }
}
