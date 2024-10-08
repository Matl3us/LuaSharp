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

        public IExpression? ParseBoolean()
        {
            if (bool.TryParse(curToken.Literal, out bool value))
            {
                return new BooleanLiteral()
                {
                    token = curToken,
                    value = value
                };
            }

            AddBooleanParseError();
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

        public IExpression? ParseGroupedExpression()
        {
            NextToken();
            var expression = ParseExpression((int)PrecedenceValue.Lowest);

            if (!CheckAnPushToken(TokenType.R_PARENT))
            {
                AddGroupedExpressionParseError();
                return null;
            }

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
                        return ParseAssignStatement(false);
                    }
                    else
                    {
                        return ParseExpressionStatement();
                    }
                case TokenType.LOCAL:
                    return ParseLocalAssignStatement();
                case TokenType.IF:
                    return ParseIfStatement();
                case TokenType.WHILE:
                    return ParseWhileStatement();
                case TokenType.FOR:
                    return ParseForStatement();
                case TokenType.REPEAT:
                    return ParseRepeatStatement();
                case TokenType.RETURN:
                    return ParseReturnStatement();
                case TokenType.FUNCTION:
                    return ParseFunctionStatement();
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
            if (!PrefixParseFunc.TryGetValue(curToken.Type, out Func<IExpression?>? prefixFunc))
            {
                AddPrefixParseError(curToken.Type);
                return null;
            }

            var leftExpression = prefixFunc();

            while (!IsPeekToken(TokenType.NEWLINE) && !IsPeekToken(TokenType.EOF)
                    && precedence < Precedence.PeekPrecedence(peekToken))
            {
                if (!InfixParseFunc.TryGetValue(peekToken.Type, out Func<IExpression, IExpression>? infixFunc))
                {
                    return leftExpression;
                }

                NextToken();

                if (leftExpression != null)
                {
                    leftExpression = infixFunc(leftExpression);
                }
            }

            return leftExpression;
        }

        public BlockStatement? ParseBlockStatement()
        {
            var blockStatement = new BlockStatement();
            NextToken();
            while (!BlockEndings.Contains(curToken.Type) && curToken.Type != TokenType.EOF)
            {
                var statement = ParseStatement();
                if (statement != null)
                {
                    blockStatement.statements.Add(statement);
                }
                NextToken();
            }

            if (curToken.Type == TokenType.EOF)
            {
                AddBlockStatementParseError();
                return null;
            }

            return blockStatement;
        }

        public AssignStatement? ParseAssignStatement(bool isLocal)
        {
            var statement = new AssignStatement
            {
                name = new Identifier() { token = curToken, value = curToken.Literal },
                isLocal = isLocal
            };

            if (!CheckAnPushToken(TokenType.ASSIGN))
            {
                AddAssignStatementParseError();
                return null;
            }

            statement.token = curToken;
            NextToken();
            var expression = ParseExpression((int)PrecedenceValue.Lowest);
            if (expression != null)
            {
                statement.expression = expression;
            }
            else
            {
                AddAssignStatementParseError();
                return null;
            }

            return statement;
        }

        public AssignStatement? ParseLocalAssignStatement()
        {
            if (!CheckAnPushToken(TokenType.IDENTIFIER))
            {
                AddAssignStatementParseError();
                return null;
            }

            return ParseAssignStatement(true);
        }

        public ReturnStatement? ParseReturnStatement()
        {
            var statement = new ReturnStatement()
            {
                token = curToken
            };

            NextToken();
            var expression = ParseExpression((int)PrecedenceValue.Lowest);
            if (expression != null)
            {
                statement.expression = expression;
            }
            else
            {
                AddReturnStatementParseError();
                return null;
            }

            return statement;
        }

        public IfStatement? ParseIfStatement()
        {
            var statement = new IfStatement();
            NextToken();

            var condition = ParseExpression((int)PrecedenceValue.Lowest);
            if (!CheckAnPushToken(TokenType.THEN) || condition == null)
            {
                AddIfStatementParseError();
                return null;
            }
            statement.condition = condition;

            var consequence = ParseBlockStatement();
            if (consequence == null)
            {
                AddBlockStatementParseError();
                return null;
            }
            statement.consequence = (BlockStatement)consequence;


            if (IsCurToken(TokenType.ELSE))
            {
                var alternative = ParseBlockStatement();
                if (IsCurToken(TokenType.END))
                {
                    statement.alternative = alternative;
                    return statement;
                }

                AddIfStatementParseError();
                return null;
            }
            else if (IsCurToken(TokenType.END))
            {
                statement.alternative = null;
                return statement;
            }
            else
            {
                AddIfStatementParseError();
                return null;
            }

        }

        public WhileStatement? ParseWhileStatement()
        {
            var statement = new WhileStatement();
            NextToken();

            var condition = ParseExpression((int)PrecedenceValue.Lowest);
            if (!CheckAnPushToken(TokenType.DO) || condition == null)
            {
                AddWhileStatementParseError();
                return null;
            }
            statement.condition = condition;

            var body = ParseBlockStatement();
            if (body == null)
            {
                AddBlockStatementParseError();
                return null;
            }
            statement.body = (BlockStatement)body;

            if (IsCurToken(TokenType.END))
            {
                return statement;
            }
            else
            {
                AddWhileStatementParseError();
                return null;
            }
        }

        public ForStatement? ParseForStatement()
        {
            var statement = new ForStatement();
            NextToken();

            var initialValue = ParseAssignStatement(false);
            if (!CheckAnPushToken(TokenType.COMMA) || initialValue == null)
            {
                return null;
            }
            statement.initialValue = (AssignStatement)initialValue;
            NextToken();

            var limit = ParseExpression((int)PrecedenceValue.Lowest);
            if (limit == null)
            {
                return null;
            }
            statement.limit = limit;

            if (IsPeekToken(TokenType.COMMA))
            {
                NextToken();
                NextToken();
                var step = ParseExpression((int)PrecedenceValue.Lowest);
                if (step != null)
                {
                    statement.step = step;
                }
            }
            else
            {
                var step = new IntegerNumeralLiteral()
                {
                    token = new Token(TokenType.NUMERICAL, "1", peekToken.Line, peekToken.Column, ""),
                    value = 1
                };
                statement.step = step;
            }

            if (!CheckAnPushToken(TokenType.DO))
            {
                return null;
            }

            var body = ParseBlockStatement();
            if (body == null)
            {
                AddBlockStatementParseError();
                return null;
            }
            statement.body = (BlockStatement)body;

            if (IsCurToken(TokenType.END))
            {
                return statement;
            }
            else
            {
                return null;
            }
        }

        public RepeatStatement? ParseRepeatStatement()
        {
            var statement = new RepeatStatement();

            var body = ParseBlockStatement();
            if (!IsCurToken(TokenType.UNTIL) || body == null)
            {
                AddBlockStatementParseError();
                return null;
            }
            statement.body = (BlockStatement)body;
            NextToken();

            var condition = ParseExpression((int)PrecedenceValue.Lowest);
            if (condition == null)
            {
                return null;
            }
            statement.condition = condition;
            return statement;
        }

        public FunctionStatement? ParseFunctionStatement()
        {
            var statement = new FunctionStatement();
            NextToken();
            if (!IsCurToken(TokenType.IDENTIFIER))
            {
                return null;
            }
            statement.name = new Identifier()
            {
                token = curToken,
                value = curToken.Literal
            };

            if (!CheckAnPushToken(TokenType.L_PARENT))
            {
                return null;
            }
            statement.parameters = ParseFunctionIdentifiers();

            if (!CheckAnPushToken(TokenType.R_PARENT))
            {
                return null;
            }

            var body = ParseBlockStatement();
            if (body == null)
            {
                return null;
            }
            statement.body = (BlockStatement)body;

            if (IsCurToken(TokenType.END))
            {
                return statement;
            }
            else
            {
                return null;
            }
        }

        public List<Identifier> ParseFunctionIdentifiers()
        {
            var identifiers = new List<Identifier>();

            if (IsPeekToken(TokenType.R_PARENT))
            {
                NextToken();
                return identifiers;
            }
            NextToken();

            identifiers.Add(new Identifier()
            {
                token = curToken,
                value = curToken.Literal
            });

            while (IsPeekToken(TokenType.COMMA))
            {
                NextToken();
                NextToken();
                identifiers.Add(new Identifier()
                {
                    token = curToken,
                    value = curToken.Literal
                });
            }

            return identifiers;
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

        public bool CheckAnPushToken(TokenType type)
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

        public void AddBooleanParseError()
        {
            string msg = $"Error at line {peekToken.Line} column {peekToken.Column}\nInvalid boolean value\n";
            errors.Add(msg);
        }

        public void AddGroupedExpressionParseError()
        {
            string msg = $"Error at line {peekToken.Line} column {peekToken.Column}\nRight parenthesis missing\n";
            errors.Add(msg);
        }

        public void AddBlockStatementParseError()
        {
            string msg = $"Error at line {peekToken.Line} column {peekToken.Column}\nBlock ending missing\n";
            errors.Add(msg);
        }

        public void AddAssignStatementParseError()
        {
            string msg = $"Error at line {peekToken.Line} column {peekToken.Column}\nInvaild assignment statement\n";
            errors.Add(msg);
        }

        public void AddReturnStatementParseError()
        {
            string msg = $"Error at line {peekToken.Line} column {peekToken.Column}\nInvaild assignment statement\n";
            errors.Add(msg);
        }

        public void AddIfStatementParseError()
        {
            string msg = $"Error at line {peekToken.Line} column {peekToken.Column}\nInvalid format of If statement\n";
            errors.Add(msg);
        }

        public void AddWhileStatementParseError()
        {
            string msg = $"Error at line {peekToken.Line} column {peekToken.Column}\nInvalid format of while statement\n";
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