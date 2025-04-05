using System.Text;
using LuaSharp;
using LuaSharp.AST;
using LuaSharp.AST.Expressions;
using LuaSharp.AST.Statements;

namespace Test
{
    public class ParserTest
    {
        [Theory]
        [InlineData("3", 3)]
        [InlineData("5", 5)]
        [InlineData("345", 345)]
        public void ShouldParseIntegerLiteral(string input, int value)
        {
            AssertParsedLiteral<IntegerNumeralLiteral>(input, TokenType.NUMERICAL, value);
        }

        [Theory]
        [InlineData("3.0", 3.0)]
        [InlineData("3.1416", 3.1416)]
        [InlineData("314.16e-2", 3.1416)]
        [InlineData("0.31416E1", 3.1416)]
        [InlineData("34e1", 340.0)]
        public void ShouldParseFloatLiteral(string input, double value)
        {
            AssertParsedLiteral<FloatNumeralLiteral>(input, TokenType.NUMERICAL, value);
        }

        [Theory]
        [InlineData("0xff", 255)]
        [InlineData("0xBEBADA", 12499674)]
        public void ShouldParseHexLiteral(string input, int value)
        {
            AssertParsedLiteral<IntegerNumeralLiteral>(input, TokenType.NUMERICAL, value);
        }

        [Theory]
        [InlineData("true", TokenType.TRUE, true)]
        [InlineData("false", TokenType.FALSE, false)]
        public void ShouldParseBooleanLiteral(string input, TokenType tokenType, bool value)
        {
            AssertParsedLiteral<BooleanLiteral>(input, tokenType, value);
        }

        [Theory]
        [InlineData("-x", TokenType.MINUS, "[- x]")]
        [InlineData("~y", TokenType.TILDE, "[~ y]")]
        [InlineData("not z", TokenType.NOT, "[not z]")]
        [InlineData("#a", TokenType.HASHTAG, "[# a]")]
        public void ShouldParsePrefixStatements(string input, TokenType tokenType, string value)
        {
            AssertParsedPrefixStatement(input, tokenType, value);
        }

        [Theory]
        [InlineData("x + 2", TokenType.PLUS, "[x + 2]")]
        [InlineData("2 - x", TokenType.MINUS, "[2 - x]")]
        [InlineData("x * 2", TokenType.ASTERISK, "[x * 2]")]
        [InlineData("2 / x", TokenType.SLASH, "[2 / x]")]
        [InlineData("x // 2", TokenType.F_DIV, "[x // 2]")]
        [InlineData("2 % x", TokenType.PERCENTAGE, "[2 % x]")]
        [InlineData("x ^ 2", TokenType.CARET, "[x ^ 2]")]

        [InlineData("y & 3", TokenType.B_AND, "[y & 3]")]
        [InlineData("3 | y", TokenType.B_OR, "[3 | y]")]
        [InlineData("y ~ 3", TokenType.TILDE, "[y ~ 3]")]
        [InlineData("3 << y", TokenType.B_LSHIFT, "[3 << y]")]
        [InlineData("y >> 3", TokenType.B_RSHIFT, "[y >> 3]")]

        [InlineData("z == 4", TokenType.EQUAL, "[z == 4]")]
        [InlineData("4 ~= z", TokenType.NOT_EQUAL, "[4 ~= z]")]
        [InlineData("z < 4", TokenType.LESS, "[z < 4]")]
        [InlineData("4 > z", TokenType.MORE, "[4 > z]")]
        [InlineData("z <= 4", TokenType.LESS_EQUAL, "[z <= 4]")]
        [InlineData("4 >= z", TokenType.MORE_EQUAL, "[4 >= z]")]

        [InlineData("a or b", TokenType.OR, "[a or b]")]
        [InlineData("b and a", TokenType.AND, "[b and a]")]
        public void ShouldParseInfixStatements(string input, TokenType tokenType, string value)
        {
            AssertParsedInfixStatement(input, tokenType, value);
        }

        [Theory]
        [InlineData("1 + 2 * 3 / 4 - 5", "[[1 + [[2 * 3] / 4]] - 5]")]
        [InlineData("(1 + 2) * (3 - 4) / (5 + 6)", "[[[1 + 2] * [3 - 4]] / [5 + 6]]")]
        [InlineData("1 + (2 * (3 + (4 / 5)))", "[1 + [2 * [3 + [4 / 5]]]]")]
        [InlineData("-(1 + 2) * -(3 / 4)", "[[- [1 + 2]] * [- [3 / 4]]]")]
        [InlineData("(2 ^ 3) ^ (1 / 2)", "[[2 ^ 3] ^ [1 / 2]]")]
        [InlineData("(1 + 2 > 3) and (4 / 2 == 2)", "[[[1 + 2] > 3] and [[4 / 2] == 2]]")]
        [InlineData("not (1 > 2) or (3 == 3 and 4 <= 5)", "[[not [1 > 2]] or [[3 == 3] and [4 <= 5]]]")]
        [InlineData("(1 & 2) | (3 ~ 4)", "[[1 & 2] | [3 ~ 4]]")]
        [InlineData("1 < 2 and 3 > 4 or 5 == 5", "[[[1 < 2] and [3 > 4]] or [5 == 5]]")]
        public void ShouldParseGroupedStatements(string input, string output)
        {
            AssertParsedStatement<ExpressionStatement>(input, output);
        }

        [Theory]
        [InlineData("a = 1 + 2 * 3", "a = [1 + [2 * 3]]")]
        [InlineData("a = (1 + 2) * 3", "a = [[1 + 2] * 3]")]
        [InlineData("local a = 1", "local a = 1")]
        [InlineData("local x = (1 + 2) * (3 - 4)", "local x = [[1 + 2] * [3 - 4]]")]
        [InlineData("local a = 2 ^ 3", "local a = [2 ^ 3]")]
        [InlineData("x = not 1 > 2 or 3 == 3", "x = [[[not 1] > 2] or [3 == 3]]")]
        public void ShouldParseAssignStatements(string input, string output)
        {
            AssertParsedStatement<AssignStatement>(input, output);
        }

        [Theory]
        [InlineData("return 1 + 2 * 3", "return [1 + [2 * 3]]")]
        [InlineData("return (1 + 2) * 3", "return [[1 + 2] * 3]")]
        [InlineData("return x - 2 * var", "return [x - [2 * var]]")]
        [InlineData("return not 1 > 2 or 3 == 3", "return [[[not 1] > 2] or [3 == 3]]")]
        public void ShouldParseReturnStatements(string input, string output)
        {
            AssertParsedStatement<ReturnStatement>(input, output);
        }

        [Theory]
        [InlineData("if a > 5 then\n a = 5\n else b = 10\n end", "if [a > 5] then [a = 5] else [b = 10] end")]
        [InlineData("if (x == 1) then\n y = 2\n end", "if [x == 1] then [y = 2] end")]
        [InlineData("if a == b and c ~= d then\n e = f + 1\n else e = f - 1\n end", "if [[a == b] and [c ~= d]] then [e = [f + 1]] else [e = [f - 1]] end")]
        [InlineData("if not (x < 10) then\n y = 0\n else z = 1\n end", "if [not [x < 10]] then [y = 0] else [z = 1] end")]
        [InlineData("if a == b then if (c > d) then\n e = f\n else g = h\n end\n else i = j\n end", "if [a == b] then [if [c > d] then [e = f] else [g = h] end] else [i = j] end")]
        [InlineData("if ((a or b) and c) then\n d = 1\n else d = 2\n end", "if [[a or b] and c] then [d = 1] else [d = 2] end")]
        public void ShouldParseIfStatements(string input, string output)
        {
            AssertParsedStatement<IfStatement>(input, output);
        }

        [Theory]
        [InlineData("while a > 5 do\n a = 5\n b = 10\n end", "while [a > 5] do [a = 5][b = 10] end")]
        [InlineData("while (x == 1) do\n y = 2\n end", "while [x == 1] do [y = 2] end")]
        [InlineData("while z <= 0 do\n z = z + 1\n end", "while [z <= 0] do [z = [z + 1]] end")]
        [InlineData("while (a < 100 and b > 50) do\n a = a + 10\n b = b - 5\n end", "while [[a < 100] and [b > 50]] do [a = [a + 10]][b = [b - 5]] end")]
        public void ShouldParseWhileStatements(string input, string output)
        {
            AssertParsedStatement<WhileStatement>(input, output);
        }

        [Theory]
        [InlineData("for i = 10,19,1 do\n x = x + 2\n end", "for [i = 10, 19, 1] do [x = [x + 2]] end")]
        [InlineData("for j = 1,5 do\n sum = sum + j\n end", "for [j = 1, 5, 1] do [sum = [sum + j]] end")]
        [InlineData("for k = 0,10,2 do\n k = k * 2\n end", "for [k = 0, 10, 2] do [k = [k * 2]] end")]
        [InlineData("for i = 5,1,-1 do\n total = total + i\n end", "for [i = 5, 1, [- 1]] do [total = [total + i]] end")]
        public void ShouldParseForStatements(string input, string output)
        {
            AssertParsedStatement<ForStatement>(input, output);
        }

        [Theory]
        [InlineData("repeat x = x + 2\n until x > 2", "repeat [x = [x + 2]] until [x > 2]")]
        [InlineData("repeat count = count + 5\n until count >= 100", "repeat [count = [count + 5]] until [count >= 100]")]
        [InlineData("repeat sum = sum + 1\n x = x + sum\n until x >= 50", "repeat [sum = [sum + 1]][x = [x + sum]] until [x >= 50]")]
        public void ShouldParseRepeatStatements(string input, string output)
        {
            AssertParsedStatement<RepeatStatement>(input, output);
        }

        [Theory]
        [InlineData(
            "function sum(x, y)\n local result = x + y\n return result\n end",
            "function sum[[x][y]] [local result = [x + y]][return result] end")]
        [InlineData(
            "function max(num1, num2)\n if (num1 > num2) then\n return num1\n end\n end",
            "function max[[num1][num2]] [if [num1 > num2] then [return num1] end] end")]
        [InlineData(
            "function factorial(n)\n if n == 0 then\n return 1\n else\n return n * factorial(n-1)\n end\n end",
            "function factorial[[n]] [if [n == 0] then [return 1] else [return [n * factorial]][[n - 1]] end] end")]
        public void ShouldParseFunctionStatements(string input, string output)
        {
            AssertParsedStatement<FunctionStatement>(input, output);
        }

        [Theory]
        [InlineData("sum(x, y)", "sum[[x][y]]")]
        [InlineData("sum(x + 5, y * 2)", "sum[[[x + 5]][[y * 2]]]")]
        [InlineData("power((2 + var) / 3, z - 1)", "power[[[[2 + var] / 3]][[z - 1]]]")]
        public void ShouldParseFunctionCallStatements(string input, string output)
        {
            AssertParsedStatement<FunctionCallStatement>(input, output);
        }

        private static IStatement ParseSingleStatement(string input)
        {
            byte[] byteArray = Encoding.ASCII.GetBytes(input);
            using var stream = new MemoryStream(byteArray);
            using var reader = new StreamReader(stream);

            var lexer = new Lexer(reader);
            var parser = new Parser(lexer);

            var ast = parser.ParseCode();
            var errors = parser.GetErrors();

            Assert.Empty(errors);
            Assert.Single(ast.Statements);

            return ast.Statements[0];
        }

        private static void AssertParsedLiteral<T>(string input, TokenType expectedType, object expectedValue)
        {
            var expression = ParseSingleStatement(input);
            Assert.IsType<ExpressionStatement>(expression);
            var castedExpression = (ExpressionStatement)expression;
            Assert.Equal(expectedType, castedExpression.Token.Type);
            Assert.NotNull(castedExpression.Expression);

            dynamic literal = castedExpression.Expression;
            Assert.IsType<T>(literal);
            Assert.Equal(expectedValue, literal.Value);
        }

        private static void AssertParsedPrefixStatement(string input, TokenType expectedType, string expectedValue)
        {
            var statement = ParseSingleStatement(input);

            Assert.IsType<ExpressionStatement>(statement);
            var exprStatement = (ExpressionStatement)statement;

            Assert.Equal(expectedType, exprStatement.Token.Type);
            Assert.NotNull(exprStatement.Expression);
            Assert.Equal(expectedValue, exprStatement.Expression.String());
        }

        private static void AssertParsedInfixStatement(string input, TokenType expectedType, string expectedValue)
        {
            var statement = ParseSingleStatement(input);

            Assert.IsType<ExpressionStatement>(statement);
            var exprStatement = (ExpressionStatement)statement;

            Assert.NotNull(exprStatement.Expression);
            var infix = Assert.IsType<InfixExpression>(exprStatement.Expression);
            Assert.Equal(expectedType, infix.Token.Type);
            Assert.Equal(expectedValue, infix.String());
        }

        private static void AssertParsedStatement<T>(string input, string expectedValue)
        {
            var expression = ParseSingleStatement(input);
            Assert.IsType<T>(expression);
            dynamic statement = (T)expression;
            Assert.Equal(expectedValue, statement.String());
        }
    }
}
