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

            var expectedOutput = new[]
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
                Assert.Equal(expectedOutput[i].Type, castedExpression.token.Type);
                Assert.NotNull(castedExpression.expression);

                Assert.IsType<IntegerNumeralLiteral>(castedExpression.expression);
                var integerExpression = (IntegerNumeralLiteral)castedExpression.expression;
                Assert.Equal(expectedOutput[i].Value, integerExpression.value);
            }
        }

        [Fact]
        public void ShouldParseFloatLiteral()
        {
            var input = new List<string>()
            {
               "3.0", "3.1416", "314.16e-2", "0.31416E1", "34e1"
            };

            var expectedOutput = new[]
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
                Assert.Equal(expectedOutput[i].Type, castedExpression.token.Type);
                Assert.NotNull(castedExpression.expression);

                Assert.IsType<FloatNumeralLiteral>(castedExpression.expression);
                var floatExpression = (FloatNumeralLiteral)castedExpression.expression;
                Assert.Equal(expectedOutput[i].Value, floatExpression.value);
            }
        }

        [Fact]
        public void ShouldParseHexLiteral()
        {
            var input = new List<string>()
            {
               "0xff", "0xBEBADA"
            };

            var expectedOutput = new[]
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
                Assert.Equal(expectedOutput[i].Type, castedExpression.token.Type);
                Assert.NotNull(castedExpression.expression);

                Assert.IsType<IntegerNumeralLiteral>(castedExpression.expression);
                var integerExpression = (IntegerNumeralLiteral)castedExpression.expression;
                Assert.Equal(expectedOutput[i].Value, integerExpression.value);
            }
        }

        [Fact]
        public void ShouldParseBooleanLiteral()
        {
            var input = new List<string>()
            {
               "true", "false"
            };

            var expectedOutput = new[]
            {
                new { Type = TokenType.TRUE, Value = true },
                new { Type = TokenType.FALSE, Value = false },
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
                Assert.Equal(expectedOutput[i].Type, castedExpression.token.Type);
                Assert.NotNull(castedExpression.expression);

                Assert.IsType<BooleanLiteral>(castedExpression.expression);
                var booleanExpression = (BooleanLiteral)castedExpression.expression;
                Assert.Equal(expectedOutput[i].Value, booleanExpression.value);
            }
        }

        [Fact]
        public void ShouldParsePrefixStatements()
        {
            var input = new List<string>()
            {
               "-x", "~y", "not z", "#a"
            };

            var expectedOutput = new[]
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
                Assert.Equal(expectedOutput[i].Type, castedExpression.token.Type);
                Assert.NotNull(castedExpression.expression);
                Assert.Equal(expectedOutput[i].String, castedExpression.expression.String());
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

            var expectedOutput = new[]
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
                Assert.Equal(expectedOutput[i].Type, castedExpression.token.Type);
                Assert.Equal(expectedOutput[i].String, castedExpressionStatement.expression.String());
            }
        }

        [Fact]
        public void ShouldParseGroupedStatements()
        {
            var input = new List<string>()
            {
                "1 + 2 * 3 / 4 - 5",
                "(1 + 2) * (3 - 4) / (5 + 6)",
                "1 + (2 * (3 + (4 / 5)))",
                "-(1 + 2) * -(3 / 4)",
                "(2 ^ 3) ^ (1 / 2)",
                "(1 + 2 > 3) and (4 / 2 == 2)",
                "not (1 > 2) or (3 == 3 and 4 <= 5)",
                "(1 & 2) | (3 ~ 4)",
                "1 < 2 and 3 > 4 or 5 == 5",
            };

            var expectedOutput = new List<string>()
            {
                "[[1 + [[2 * 3] / 4]] - 5]",
                "[[[1 + 2] * [3 - 4]] / [5 + 6]]",
                "[1 + [2 * [3 + [4 / 5]]]]",
                "[[- [1 + 2]] * [- [3 / 4]]]",
                "[[2 ^ 3] ^ [1 / 2]]",
                "[[[1 + 2] > 3] and [[4 / 2] == 2]]",
                "[[not [1 > 2]] or [[3 == 3] and [4 <= 5]]]",
                "[[1 & 2] | [3 ~ 4]]",
                "[[[1 < 2] and [3 > 4]] or [5 == 5]]",
            };

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
                Assert.Equal(expectedOutput[i], castedExpression.String());
            }
        }

        [Fact]
        public void ShouldParseAssignStatements()
        {
            var input = new List<string>()
            {
                "a = 1 + 2 * 3",
                "a = (1 + 2) * 3",
                "local a = 1",
                "local x = (1 + 2) * (3 - 4)",
                "local a = 2 ^ 3",
                "x = not 1 > 2 or 3 == 3",
            };

            var expectedOutput = new List<string>()
            {
                "a = [1 + [2 * 3]]",
                "a = [[1 + 2] * 3]",
                "local a = 1",
                "local x = [[1 + 2] * [3 - 4]]",
                "local a = [2 ^ 3]",
                "x = [[[not 1] > 2] or [3 == 3]]",
            };

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
                Assert.IsType<AssignStatement>(expression);
                var castedAssignStatement = (AssignStatement)expression;
                Assert.Equal(expectedOutput[i], castedAssignStatement.String());
            }
        }

        [Fact]
        public void ShouldParseReturnStatements()
        {
            var input = new List<string>()
            {
                "return 1 + 2 * 3",
                "return (1 + 2) * 3",
                "return x - 2 * var",
                "return not 1 > 2 or 3 == 3",
            };

            var expectedOutput = new List<string>()
            {
                "return [1 + [2 * 3]]",
                "return [[1 + 2] * 3]",
                "return [x - [2 * var]]",
                "return [[[not 1] > 2] or [3 == 3]]",
            };

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
                Assert.IsType<ReturnStatement>(expression);
                var castedReturnStatement = (ReturnStatement)expression;
                Assert.Equal(expectedOutput[i], castedReturnStatement.String());
            }
        }

        [Fact]
        public void ShouldParseIfStatements()
        {
            var input = new List<string>()
            {
                "if a > 5 then\n a = 5\n else b = 10\n end",
                "if (x == 1) then\n y = 2\n end",
                "if a == b and c ~= d then\n e = f + 1\n else e = f - 1\n end",
                "if not (x < 10) then\n y = 0\n else z = 1\n end",
                "if a == b then if (c > d) then\n e = f\n else g = h\n end\n else i = j\n end",
                "if ((a or b) and c) then\n d = 1\n else d = 2\n end",
            };

            var expectedOutput = new List<string>()
            {
                "if [a > 5] then [a = 5] else [b = 10] end",
                "if [x == 1] then [y = 2] end",
                "if [[a == b] and [c ~= d]] then [e = [f + 1]] else [e = [f - 1]] end",
                "if [not [x < 10]] then [y = 0] else [z = 1] end",
                "if [a == b] then [if [c > d] then [e = f] else [g = h] end] else [i = j] end",
                "if [[a or b] and c] then [d = 1] else [d = 2] end",
            };

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
                Assert.IsType<IfStatement>(expression);
                var castedIfStatement = (IfStatement)expression;
                Assert.Equal(expectedOutput[i], castedIfStatement.String());
            }
        }

        [Fact]
        public void ShouldParseWhileStatements()
        {
            var input = new List<string>()
            {
                "while a > 5 do\n a = 5\n b = 10\n end",
                "while (x == 1) do\n y = 2\n end",
                "while z <= 0 do\n z = z + 1\n end",
                "while (a < 100 and b > 50) do\n a = a + 10\n b = b - 5\n end"

            };

            var expectedOutput = new List<string>()
            {
                "while [a > 5] do [a = 5][b = 10] end",
                "while [x == 1] do [y = 2] end",
                "while [z <= 0] do [z = [z + 1]] end",
                "while [[a < 100] and [b > 50]] do [a = [a + 10]][b = [b - 5]] end"
            };

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
                Assert.IsType<WhileStatement>(expression);
                var castedWhileStatement = (WhileStatement)expression;
                Assert.Equal(expectedOutput[i], castedWhileStatement.String());
            }
        }

        [Fact]
        public void ShouldParseForStatements()
        {
            var input = new List<string>()
            {
                "for i = 10,19,1 do\n x = x + 2\n end",
                "for j = 1,5 do\n sum = sum + j\n end",
                "for k = 0,10,2 do\n k = k * 2\n end",
                "for i = 5,1,-1 do\n total = total + i\n end"

            };

            var expectedOutput = new List<string>()
            {
                "for [i = 10, 19, 1] do [x = [x + 2]] end",
                "for [j = 1, 5, 1] do [sum = [sum + j]] end",
                "for [k = 0, 10, 2] do [k = [k * 2]] end",
                "for [i = 5, 1, [- 1]] do [total = [total + i]] end"
            };

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
                Assert.IsType<ForStatement>(expression);
                var castedForStatement = (ForStatement)expression;
                Assert.Equal(expectedOutput[i], castedForStatement.String());
            }
        }

        [Fact]
        public void ShouldParseRepeatStatements()
        {
            var input = new List<string>()
            {
                "repeat x = x + 2\n until x > 2",
                "repeat count = count + 5\n until count >= 100",
                "repeat sum = sum + 1\n x = x + sum\n until x >= 50"

            };

            var expectedOutput = new List<string>()
            {
                "repeat [x = [x + 2]] until [x > 2]",
                "repeat [count = [count + 5]] until [count >= 100]",
                "repeat [sum = [sum + 1]][x = [x + sum]] until [x >= 50]"
            };

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
                Assert.IsType<RepeatStatement>(expression);
                var castedRepeatStatement = (RepeatStatement)expression;
                Assert.Equal(expectedOutput[i], castedRepeatStatement.String());
            }
        }

        [Fact]
        public void ShouldParseFunctionStatements()
        {
            var input = new List<string>()
            {
                "function sum(x, y)\n local result = x + y\n return result\n end",
                "function max(num1, num2)\n if (num1 > num2) then\n return num1\n end\n end",
                "function factorial(n)\n if n == 0 then\n return 1\n else\n return n * factorial(n-1)\n end\n end"

            };

            var expectedOutput = new List<string>()
            {
                "function sum[[x][y]] [local result = [x + y]][return result] end",
                "function max[[num1][num2]] [if [num1 > num2] then [return num1] end] end",
                "function factorial[[n]] [if [n == 0] then [return 1] else [return [n * factorial]][[n - 1]] end] end"
            };

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
                Assert.IsType<FunctionStatement>(expression);
                var castedFunctionStatement = (FunctionStatement)expression;
                Assert.Equal(expectedOutput[i], castedFunctionStatement.String());
            }
        }
    }
}
