namespace LuaSharp
{
    public class Parser
    {
        public Parser(Lexer l)
        {
            lex = l;
            NextToken();
            NextToken();
        }

        public Lexer lex;
        public Token curToken;
        public Token peekToken;

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
                return false;
            }
        }
    }
}