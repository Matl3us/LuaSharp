﻿using LuaSharp.utils;
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
            if (!CheckAnPushToken(TokenType.L_PARENT))
            {
                AddIfStatementParseError();
                return null;
            }

            var condition = ParseExpression((int)PrecedenceValue.Lowest);
            if (!IsCurToken(TokenType.R_PARENT) || condition == null)
            {
                AddIfStatementParseError();
                return null;
            }
            statement.condition = condition;

            if (!CheckAnPushToken(TokenType.THEN))
            {
                AddIfStatementParseError();
                return null;
            }

            var consequence = ParseBlockStatement();
            if (consequence == null)
            {
                AddIfStatementParseError();
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