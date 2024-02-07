using System.Globalization;

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

            PrefixParseFunc = new Dictionary<TokenType, Func<IExpression?>>()
            {
                {TokenType.IDENTIFIER, ParseIdentifier},
                {TokenType.NUMERICAL, ParseNumerical},
                {TokenType.MINUS, ParsePrefixExpression},
                {TokenType.NOT, ParsePrefixExpression},
                {TokenType.HASHTAG, ParsePrefixExpression}
            };
        }

        public Lexer lex;
        public Token curToken;
        public Token peekToken;
        public List<string> errors;

        public Dictionary<TokenType, Func<IExpression?>> PrefixParseFunc;
        public IExpression ParseIdentifier() => new Identifier() { token = curToken, value = curToken.Literal };
        public IExpression? ParseNumerical()
        {
            if (double.TryParse(curToken.Literal, NumberStyles.Any, CultureInfo.InvariantCulture, out double value))
            {
                NumericalLiteral literal = new NumericalLiteral()
                {
                    token = curToken,
                    value = value
                };
                return literal;
            }
            else
            {
                return null;
            }
        }
        public IExpression ParsePrefixExpression()
        {
            PrefixExpression expression = new PrefixExpression()
            {
                token = curToken,
                operatorSign = curToken.Literal
            };

            NextToken();
            expression.rightSide = ParseExpression((int)Precedence.Unary);
            return expression;
        }

        //public Dictionary<TokenType, Func<IExpression, IExpression>> InfixParseFunc;
        public enum Precedence : int
        {
            FuncCall,
            Unary,          // not # -
            Concatenation,  // ..
            Multiplicative, // * / %
            Additive,       // + -
            Relational,     // < > <= >= == ~= 
            Equality,       // == ~=
            AND,            // and
            OR,             // 	or
            Lowest
        }

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
                        return ParseExpressionStatement();
                    }
                case TokenType.RETURN:
                    return ParseReturnStatement();
                case TokenType.NEWLINE:
                    return null;
                default:
                    return ParseExpressionStatement();

            }
        }

        public ExpressionStatement ParseExpressionStatement()
        {
            ExpressionStatement statement = new ExpressionStatement
            {
                token = curToken,
                expression = ParseExpression((int)Precedence.Lowest)
            };

            return statement;
        }

        public IExpression? ParseExpression(int precedence)
        {
            if (PrefixParseFunc.TryGetValue(curToken.Type, out Func<IExpression?>? value))
            {
                var prefixFunc = value;
                return prefixFunc();
            }
            AddPrefixParseError(curToken.Type);
            return null;
        }

        public AssignStatement? ParseAssignStatement()
        {
            AssignStatement statement = new AssignStatement
            {
                name = new Identifier() { token = curToken, value = curToken.Literal }
            };

            if (!CheckNextToken(TokenType.ASSIGN))
            {
                return null;
            }

            statement.token = curToken;

            // TODO: Add expression parsing
            // For now skip to the end of the line.
            while (!IsCurToken(TokenType.NEWLINE) && !IsCurToken(TokenType.EOF))
            {
                NextToken();
            }

            return statement;
        }

        public ReturnStatement? ParseReturnStatement()
        {
            ReturnStatement statement = new ReturnStatement()
            {
                token = curToken
            };

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
            string msg = $"Error at line {peekToken.Line} column {peekToken.Column}\nExpected token {type} but got {peekToken.Type} instead\n";
            errors.Add(msg);
        }

        public void AddPrefixParseError(TokenType type)
        {
            string msg = $"No prefix parse function for {type} found";
            errors.Add(msg);
        }
    }
}