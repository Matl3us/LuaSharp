﻿namespace LuaSharp.Parser
{
    public partial class Parser
    {
        public Parser(Lexer l)
        {
            lex = l;
            NextToken();
            NextToken();
            errors = [];

            PrefixParseFunc = new Dictionary<TokenType, Func<IExpression?>>()
            {
                {TokenType.IDENTIFIER, ParseIdentifier},
                {TokenType.NUMERICAL, ParseNumerical},
                {TokenType.MINUS, ParsePrefixExpression},
                {TokenType.TILDE, ParsePrefixExpression},
                {TokenType.NOT, ParsePrefixExpression},
                {TokenType.HASHTAG, ParsePrefixExpression}
            };

            InfixParseFunc = new Dictionary<TokenType, Func<IExpression, IExpression>>()
            {
                {TokenType.PLUS, ParseInfixExpression},
                {TokenType.MINUS, ParseInfixExpression},
                {TokenType.ASTERISK, ParseInfixExpression},
                {TokenType.SLASH, ParseInfixExpression},
                {TokenType.F_DIV, ParseInfixExpression},
                {TokenType.PERCENTAGE, ParseInfixExpression},
                {TokenType.CARET, ParseInfixExpression},
                {TokenType.B_AND, ParseInfixExpression},
                {TokenType.B_OR, ParseInfixExpression},
                {TokenType.TILDE, ParseInfixExpression},
                {TokenType.B_RSHIFT, ParseInfixExpression},
                {TokenType.B_LSHIFT, ParseInfixExpression},
                {TokenType.EQUAL, ParseInfixExpression},
                {TokenType.NOT_EQUAL, ParseInfixExpression},
                {TokenType.LESS, ParseInfixExpression},
                {TokenType.MORE, ParseInfixExpression},
                {TokenType.LESS_EQUAL, ParseInfixExpression},
                {TokenType.MORE_EQUAL, ParseInfixExpression},
                {TokenType.AND, ParseInfixExpression},
                {TokenType.OR, ParseInfixExpression},
            };
        }

        private readonly Lexer lex;
        private Token curToken;
        private Token peekToken;
        private readonly List<string> errors;

        private readonly Dictionary<TokenType, Func<IExpression?>> PrefixParseFunc;
        private readonly Dictionary<TokenType, Func<IExpression, IExpression>> InfixParseFunc;
    }
}