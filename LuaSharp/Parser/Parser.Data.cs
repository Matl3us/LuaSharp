namespace LuaSharp.Parser
{
    partial class Parser
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
                {TokenType.NOT, ParsePrefixExpression},
                {TokenType.HASHTAG, ParsePrefixExpression}
            };

            InfixParseFunc = new Dictionary<TokenType, Func<IExpression, IExpression>>()
            {
                {TokenType.PLUS, ParseInfixExpression},
                {TokenType.MINUS, ParseInfixExpression},
                {TokenType.ASTERISK, ParseInfixExpression},
                {TokenType.SLASH, ParseInfixExpression},
                {TokenType.EQUAL, ParseInfixExpression},
                {TokenType.NOT_EQUAL, ParseInfixExpression},
                {TokenType.LESS, ParseInfixExpression},
                {TokenType.MORE, ParseInfixExpression},
                {TokenType.LESS_EQUAL, ParseInfixExpression},
                {TokenType.MORE_EQUAL, ParseInfixExpression},
            };
        }

        public Lexer lex;
        public Token curToken;
        public Token peekToken;
        public List<string> errors;

        public Dictionary<TokenType, Func<IExpression?>> PrefixParseFunc;
        public Dictionary<TokenType, Func<IExpression, IExpression>> InfixParseFunc;
    }
}