namespace LuaSharp
{
    public class Parser
    {
        public Parser(Lexer l)
        {
            lex = l;
            NextToken();
            NextToken();
            errors = new List<string>();
        }

        public Lexer lex;
        public Token curToken;
        public Token peekToken;
        public List<string> errors;

        public void NextToken()
        {
            curToken = peekToken;
            peekToken = lex.NextToken();
        }

        public AST ParseCode()
        {
            AST ast = new AST();

            while (curToken.Type != TokenType.EOF)
            {
                var statement = ParseStatement();
                if (statement != null)
                {
                    ast.statements.Add(statement);
                }
                NextToken();
            }

            return ast;
        }

        public IStatement? ParseStatement()
        {
            switch (curToken.Type)
            {
                case TokenType.IDENTIFIER:
                    if (peekToken.Type == TokenType.ASSIGN)
                    {
                        return ParseAssignStatement();
                    }
                    else
                    {
                        return null;
                    }
                default:
                    return null;
            }
        }

        public AssignStatement? ParseAssignStatement()
        {
            AssignStatement statement = new AssignStatement
            {
                name = new Identifier() { token = curToken.Type, value = curToken.Literal }
            };

            if (!CheckNextToken(TokenType.ASSIGN))
            {
                return null;
            }

            // TODO: Add expression parsing
            // For now skip to the end of the line.
            while (!IsCurToken(TokenType.NEWLINE) && !IsCurToken(TokenType.EOF))
            {
                NextToken();
            }

            return statement;
        }

        public bool IsCurToken(TokenType type)
        {
            return type == curToken.Type;
        }

        public bool IsPeekToken(TokenType type)
        {
            return type == peekToken.Type;
        }

        public bool CheckNextToken(TokenType type)
        {
            if (IsPeekToken(type))
            {
                NextToken();
                return true;
            }
            else
            {
                AddPeekError(type);
                return false;
            }
        }

        public void Errors()
        {
            foreach (string err in errors)
            {
                Console.WriteLine($"Error: {err}");
            }
        }

        public void AddPeekError(TokenType type)
        {
            string msg = $"Expected token {type} but got {peekToken.Type} instead";
            errors.Add(msg);
        }
    }
}