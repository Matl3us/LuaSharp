using LuaSharp;
using LuaSharp.Parser;
using System.Text;

namespace Test
{
    public class ParserTest
    {
        [Fact]
        public void ShouldParseIntegerLiteral()
        {
            var input = new List<string>()
            {
               "3", "5", "345",
            };

            var output = new[]
            {
                new { Type = TokenType.NUMERICAL, Value = 3 },
                new { Type = TokenType.NUMERICAL, Value = 5 },
                new { Type = TokenType.NUMERICAL, Value = 345 },
            }.ToList();

            for (int i = 0; i < input.Count; i++)
            {
                var statement = input[i];
                byte[] byteArray = Encoding.ASCII.GetBytes(statement);
                var stream = new MemoryStream(byteArray);
                var lexer = new Lexer(new StreamReader(stream), "");

                var parser = new Parser(lexer);
                var ast = parser.ParseCode();
                var errors = parser.GetErrors();

                Assert.Empty(errors);
                Assert.Single(ast.statements);

                var expression = ast.statements[0];

                Assert.IsType<ExpressionStatement>(expression);
                var castedExpression = (ExpressionStatement)expression;
                Assert.Equal(castedExpression.token.Type, output[i].Type);
                Assert.NotNull(castedExpression.expression);

                Assert.IsType<IntegerNumeralLiteral>(castedExpression.expression);
                var integerExpression = (IntegerNumeralLiteral)castedExpression.expression;
                Assert.Equal(integerExpression.value, output[i].Value);
            }
        }

        [Fact]
        public void ShouldParseFloatLiteral()
        {
            var input = new List<string>()
            {
               "3.0", "3.1416", "314.16e-2", "0.31416E1", "34e1"
            };

            var output = new[]
            {
                new { Type = TokenType.NUMERICAL, Value = 3.0 },
                new { Type = TokenType.NUMERICAL, Value = 3.1416 },
                new { Type = TokenType.NUMERICAL, Value = 314.16e-2 },
                new { Type = TokenType.NUMERICAL, Value = 0.31416E1 },
                new { Type = TokenType.NUMERICAL, Value = 34e1 },
            }.ToList();

            for (int i = 0; i < input.Count; i++)
            {
                var statement = input[i];
                byte[] byteArray = Encoding.ASCII.GetBytes(statement);
                var stream = new MemoryStream(byteArray);
                var lexer = new Lexer(new StreamReader(stream), "");

                var parser = new Parser(lexer);
                var ast = parser.ParseCode();
                var errors = parser.GetErrors();

                Assert.Empty(errors);
                Assert.Single(ast.statements);

                var expression = ast.statements[0];

                Assert.IsType<ExpressionStatement>(expression);
                var castedExpression = (ExpressionStatement)expression;
                Assert.Equal(castedExpression.token.Type, output[i].Type);
                Assert.NotNull(castedExpression.expression);

                Assert.IsType<FloatNumeralLiteral>(castedExpression.expression);
                var floatExpression = (FloatNumeralLiteral)castedExpression.expression;
                Assert.Equal(floatExpression.value, output[i].Value);
            }
        }

        [Fact]
        public void ShouldParseHexLiteral()
        {
            var input = new List<string>()
            {
               "0xff", "0xBEBADA"
            };

            var output = new[]
            {
                new { Type = TokenType.NUMERICAL, Value = 255 },
                new { Type = TokenType.NUMERICAL, Value = 12499674 },
            }.ToList();

            for (int i = 0; i < input.Count; i++)
            {
                var statement = input[i];
                byte[] byteArray = Encoding.ASCII.GetBytes(statement);
                var stream = new MemoryStream(byteArray);
                var lexer = new Lexer(new StreamReader(stream), "");

                var parser = new Parser(lexer);
                var ast = parser.ParseCode();
                var errors = parser.GetErrors();

                Assert.Empty(errors);
                Assert.Single(ast.statements);

                var expression = ast.statements[0];

                Assert.IsType<ExpressionStatement>(expression);
                var castedExpression = (ExpressionStatement)expression;
                Assert.Equal(castedExpression.token.Type, output[i].Type);
                Assert.NotNull(castedExpression.expression);

                Assert.IsType<IntegerNumeralLiteral>(castedExpression.expression);
                var integerExpression = (IntegerNumeralLiteral)castedExpression.expression;
                Assert.Equal(integerExpression.value, output[i].Value);
            }
        }

        [Fact]
        public void ShouldParsePrefixStatements()
        {
            var input = new List<string>()
            {
               "-x", "~y", "not z", "#a"
            };

            var output = new[]
            {
                new { Type = TokenType.MINUS, String = "[- x]" },
                new { Type = TokenType.TILDE, String = "[~ y]" },
                new { Type = TokenType.NOT, String = "[not z]" },
                new { Type = TokenType.HASHTAG, String = "[# a]" }
            }.ToList();

            for (int i = 0; i < input.Count; i++)
            {
                var statement = input[i];
                byte[] byteArray = Encoding.ASCII.GetBytes(statement);
                var stream = new MemoryStream(byteArray);
                var lexer = new Lexer(new StreamReader(stream), "");

                var parser = new Parser(lexer);
                var ast = parser.ParseCode();
                var errors = parser.GetErrors();

                Assert.Empty(errors);
                Assert.Single(ast.statements);

                var expression = ast.statements[0];
                Assert.IsType<ExpressionStatement>(expression);
                var castedExpression = (ExpressionStatement)expression;
                Assert.Equal(castedExpression.token.Type, output[i].Type);
                Assert.NotNull(castedExpression.expression);
                Assert.Equal(castedExpression.expression.String(), output[i].String);
            }
        }

        [Fact]
        public void ShouldParseInfixStatements()
        {
            var input = new List<string>()
            {
                // Arithmetic
                "x + 2", "2 - x", "x * 2", "2 / x",
                "x // 2", "2 % x", "x ^ 2",
                // Bitwise
                "y & 3", "3 | y", "y ~ 3",
                "3 << y", "y >> 3",
                // Relational
                "z == 4", "4 ~= z", "z < 4", "4 > z",
                "z <= 4", "4 >= z",
                // Logical
                "a or b", "b and a"
            };

            var output = new[]
            {
                new { Type = TokenType.PLUS, String = "[x + 2]" },
                new { Type = TokenType.MINUS, String = "[2 - x]" },
                new { Type = TokenType.ASTERISK, String = "[x * 2]" },
                new { Type = TokenType.SLASH, String = "[2 / x]" },
                new { Type = TokenType.F_DIV, String = "[x // 2]" },
                new { Type = TokenType.PERCENTAGE, String = "[2 % x]" },
                new { Type = TokenType.CARET, String = "[x ^ 2]" },

                new { Type = TokenType.B_AND, String = "[y & 3]" },
                new { Type = TokenType.B_OR, String = "[3 | y]" },
                new { Type = TokenType.TILDE, String = "[y ~ 3]" },
                new { Type = TokenType.B_LSHIFT, String = "[3 << y]" },
                new { Type = TokenType.B_RSHIFT, String = "[y >> 3]" },

                new { Type = TokenType.EQUAL, String = "[z == 4]" },
                new { Type = TokenType.NOT_EQUAL, String = "[4 ~= z]" },
                new { Type = TokenType.LESS, String = "[z < 4]" },
                new { Type = TokenType.MORE, String = "[4 > z]" },
                new { Type = TokenType.LESS_EQUAL, String = "[z <= 4]" },
                new { Type = TokenType.MORE_EQUAL, String = "[4 >= z]" },

                new { Type = TokenType.OR, String = "[a or b]" },
                new { Type = TokenType.AND, String = "[b and a]" },
            }.ToList();

            for (int i = 0; i < input.Count; i++)
            {
                var statement = input[i];
                byte[] byteArray = Encoding.ASCII.GetBytes(statement);
                var stream = new MemoryStream(byteArray);
                var lexer = new Lexer(new StreamReader(stream), "");

                var parser = new Parser(lexer);
                var ast = parser.ParseCode();
                var errors = parser.GetErrors();

                Assert.Empty(errors);
                Assert.Single(ast.statements);

                var expression = ast.statements[0];
                Assert.IsType<ExpressionStatement>(expression);
                var castedExpressionStatement = (ExpressionStatement)expression;
                Assert.NotNull(castedExpressionStatement.expression);
                var castedExpression = (InfixExpression)castedExpressionStatement.expression;
                Assert.Equal(castedExpression.token.Type, output[i].Type);
                Assert.Equal(castedExpressionStatement.expression.String(), output[i].String);
            }
        }
    }
}
