using LuaSharp.utils;
using System.Globalization;
using System.Text.RegularExpressions;

namespace LuaSharp.Parser
{
    public partial class Parser
    {
        public IExpression ParseIdentifier() => new Identifier() { token = curToken, value = curToken.Literal };

        public IExpression? ParseNumeral()
        {
            string hexPattern = @"0[xX][0-9a-fA-F]+";
            if (Regex.IsMatch(curToken.Literal, hexPattern))
            {
                return new IntegerNumeralLiteral()
                {
                    token = curToken,
                    value = Convert.ToInt32(curToken.Literal, 16)
                };
            }


            if (int.TryParse(curToken.Literal, NumberStyles.Any, CultureInfo.InvariantCulture, out int integerValue))
            {
                if (curToken.Literal.Contains('.') || curToken.Literal.Contains('e') || curToken.Literal.Contains('E'))
                {
                    return new FloatNumeralLiteral()
                    {
                        token = curToken,
                        value = integerValue
                    };
                }

                return new IntegerNumeralLiteral()
                {
                    token = curToken,
                    value = integerValue
                };
            }

            if (double.TryParse(curToken.Literal, NumberStyles.Any, CultureInfo.InvariantCulture, out double doubleValue))
            {
                return new FloatNumeralLiteral()
                {
                    token = curToken,
                    value = doubleValue
                };
            }

            AddNumeralParseError();
            return null;
        }

        public IExpression ParsePrefixExpression()
        {
            var expression = new PrefixExpression()
            {
                token = curToken,
                operatorSign = curToken.Literal
            };

            NextToken();
            expression.rightSide = ParseExpression((int)PrecedenceValue.Unary);
            return expression;
        }


        public IExpression ParseInfixExpression(IExpression leftSide)
        {
            var expression = new InfixExpression()
            {
                token = curToken,
                leftSide = leftSide
            };

            int precedence = Precedence.CurPrecedence(curToken);
            expression.operatorSign = curToken.Literal;
            NextToken();
            expression.rightSide = ParseExpression(precedence);

            return expression;
        }

        public AST ParseCode()
        {
            var ast = new AST();

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
            var statement = new ExpressionStatement
            {
                token = curToken,
                expression = ParseExpression((int)PrecedenceValue.Lowest)
            };

            return statement;
        }

        public IExpression? ParseExpression(int precedence)
        {
            if (PrefixParseFunc.TryGetValue(curToken.Type, out Func<IExpression?>? prefixFunc))
            {
                var leftExpression = prefixFunc();

                while (!IsPeekToken(TokenType.NEWLINE) && !IsPeekToken(TokenType.EOF)
                    && precedence >= Precedence.PeekPrecedence(curToken))
                {
                    if (InfixParseFunc.TryGetValue(peekToken.Type, out Func<IExpression, IExpression>? infixFunc))
                    {
                        NextToken();
                        if (leftExpression != null)
                        {
                            leftExpression = infixFunc(leftExpression);
                        }
                    }
                }
                return leftExpression;
            }
            AddPrefixParseError(curToken.Type);
            return null;
        }

        public AssignStatement? ParseAssignStatement()
        {
            var statement = new AssignStatement
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
            var statement = new ReturnStatement()
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

        public void NextToken()
        {
            curToken = peekToken;
            peekToken = lex.NextToken();
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

        public void PrintErrors()
        {
            foreach (string err in errors)
            {
                Console.WriteLine($"Error: {err}");
            }
        }

        public List<string> GetErrors() => errors;

        public void AddNumeralParseError()
        {
            string msg = $"Error at line {peekToken.Line} column {peekToken.Column}\nInvalid numeral value\n";
            errors.Add(msg);
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