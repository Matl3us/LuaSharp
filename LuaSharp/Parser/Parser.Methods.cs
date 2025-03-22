﻿using LuaSharp.utils;
using System.Globalization;
using System.Text.RegularExpressions;

namespace LuaSharp
{
    public partial class Parser
    {
        public IExpression ParseIdentifier() => new IdentifierLiteral() { Token = curToken, Value = curToken.Literal };

        public IExpression? ParseNumeral()
        {
            string hexPattern = @"0[xX][0-9a-fA-F]+";
            if (Regex.IsMatch(curToken.Literal, hexPattern))
            {
                return new IntegerNumeralLiteral()
                {
                    Token = curToken,
                    Value = Convert.ToInt32(curToken.Literal, 16)
                };
            }


            if (int.TryParse(curToken.Literal, NumberStyles.Any, CultureInfo.InvariantCulture, out int integerValue))
            {
                if (curToken.Literal.Contains('.') || curToken.Literal.Contains('e') || curToken.Literal.Contains('E'))
                {
                    return new FloatNumeralLiteral()
                    {
                        Token = curToken,
                        Value = integerValue
                    };
                }

                return new IntegerNumeralLiteral()
                {
                    Token = curToken,
                    Value = integerValue
                };
            }

            if (double.TryParse(curToken.Literal, NumberStyles.Any, CultureInfo.InvariantCulture, out double doubleValue))
            {
                return new FloatNumeralLiteral()
                {
                    Token = curToken,
                    Value = doubleValue
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
                    Token = curToken,
                    Value = value
                };
            }

            AddBooleanParseError();
            return null;
        }

        public IExpression ParsePrefixExpression()
        {
            var expression = new PrefixExpression()
            {
                Token = curToken,
                OperatorSign = curToken.Literal
            };

            NextToken();
            expression.RightSide = ParseExpression((int)PrecedenceValue.Unary);
            return expression;
        }

        public IExpression ParseInfixExpression(IExpression leftSide)
        {
            var expression = new InfixExpression()
            {
                Token = curToken,
                LeftSide = leftSide
            };

            int precedence = Precedence.PeekPrecedence(curToken);
            expression.OperatorSign = curToken.Literal;
            NextToken();
            expression.RightSide = ParseExpression(precedence);

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

        public Ast ParseCode()
        {
            var ast = new Ast();

            while (curToken.Type != TokenType.EOF)
            {
                var statement = ParseStatement();
                if (statement != null)
                {
                    ast.Statements.Add(statement);
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
                    else if (peekToken.Type == TokenType.L_PARENT)
                    {
                        return ParseFunctionCallStatement();
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
                Token = curToken,
                Expression = ParseExpression((int)PrecedenceValue.Lowest)
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
                    blockStatement.Statements.Add(statement);
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
                Name = new IdentifierLiteral() { Token = curToken, Value = curToken.Literal },
                IsLocal = isLocal
            };

            if (!CheckAnPushToken(TokenType.ASSIGN))
            {
                AddAssignStatementParseError();
                return null;
            }

            statement.Token = curToken;
            NextToken();
            var expression = ParseExpression((int)PrecedenceValue.Lowest);
            if (expression != null)
            {
                statement.Expression = expression;
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
                Token = curToken
            };

            NextToken();
            var expression = ParseExpression((int)PrecedenceValue.Lowest);
            if (expression != null)
            {
                statement.Expression = expression;
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
            statement.Condition = condition;

            var consequence = ParseBlockStatement();
            if (consequence == null)
            {
                AddBlockStatementParseError();
                return null;
            }
            statement.Consequence = (BlockStatement)consequence;


            if (IsCurToken(TokenType.ELSE))
            {
                var alternative = ParseBlockStatement();
                if (IsCurToken(TokenType.END))
                {
                    statement.Aternative = alternative;
                    return statement;
                }

                AddIfStatementParseError();
                return null;
            }
            else if (IsCurToken(TokenType.END))
            {
                statement.Aternative = null;
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
            statement.Condition = condition;

            var body = ParseBlockStatement();
            if (body == null)
            {
                AddBlockStatementParseError();
                return null;
            }
            statement.Body = (BlockStatement)body;

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
            statement.InitialValue = (AssignStatement)initialValue;
            NextToken();

            var limit = ParseExpression((int)PrecedenceValue.Lowest);
            if (limit == null)
            {
                return null;
            }
            statement.Limit = limit;

            if (IsPeekToken(TokenType.COMMA))
            {
                NextToken();
                NextToken();
                var step = ParseExpression((int)PrecedenceValue.Lowest);
                if (step != null)
                {
                    statement.Step = step;
                }
            }
            else
            {
                var step = new IntegerNumeralLiteral()
                {
                    Token = new Token(TokenType.NUMERICAL, "1", peekToken.Line, peekToken.Column, ""),
                    Value = 1
                };
                statement.Step = step;
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
            statement.Body = (BlockStatement)body;

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
            statement.Body = (BlockStatement)body;
            NextToken();

            var condition = ParseExpression((int)PrecedenceValue.Lowest);
            if (condition == null)
            {
                return null;
            }
            statement.Condition = condition;
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
            statement.Name = new IdentifierLiteral()
            {
                Token = curToken,
                Value = curToken.Literal
            };

            if (!CheckAnPushToken(TokenType.L_PARENT))
            {
                return null;
            }
            statement.Parameters = ParseFunctionParameters();

            if (!CheckAnPushToken(TokenType.R_PARENT))
            {
                return null;
            }

            var body = ParseBlockStatement();
            if (body == null)
            {
                return null;
            }
            statement.Body = (BlockStatement)body;

            if (IsCurToken(TokenType.END))
            {
                return statement;
            }
            else
            {
                return null;
            }
        }

        public List<IdentifierLiteral> ParseFunctionParameters()
        {
            var identifiers = new List<IdentifierLiteral>();

            if (IsPeekToken(TokenType.R_PARENT))
            {
                NextToken();
                return identifiers;
            }
            NextToken();

            identifiers.Add(new IdentifierLiteral()
            {
                Token = curToken,
                Value = curToken.Literal
            });

            while (IsPeekToken(TokenType.COMMA))
            {
                NextToken();
                NextToken();
                identifiers.Add(new IdentifierLiteral()
                {
                    Token = curToken,
                    Value = curToken.Literal
                });
            }

            return identifiers;
        }

        public FunctionCallStatement? ParseFunctionCallStatement()
        {
            var statement = new FunctionCallStatement
            {
                Name = new IdentifierLiteral()
                {
                    Token = curToken,
                    Value = curToken.Literal
                }
            };

            if (!CheckAnPushToken(TokenType.L_PARENT))
            {
                return null;
            }

            var arguments = ParseFunctionArguments();
            if (arguments == null)
            {
                return null;
            }
            statement.Arguments = arguments;

            if (!CheckAnPushToken(TokenType.R_PARENT))
            {
                return null;
            }
            return statement;
        }

        public List<IExpression>? ParseFunctionArguments()
        {
            var expressions = new List<IExpression>();

            if (IsPeekToken(TokenType.R_PARENT))
            {
                NextToken();
                return expressions;
            }
            NextToken();

            var expStatement = ParseExpressionStatement();
            if (expStatement.Expression == null)
            {
                return null;
            }
            expressions.Add(expStatement.Expression);


            while (IsPeekToken(TokenType.COMMA))
            {
                NextToken();
                NextToken();
                var expStat = ParseExpressionStatement();
                if (expStat.Expression == null)
                {
                    return null;
                }
                expressions.Add(expStat.Expression);
            }

            return expressions;
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
            string msg = $"Error at line {peekToken.Line} column {peekToken.Column}\nInvaild return statement\n";
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