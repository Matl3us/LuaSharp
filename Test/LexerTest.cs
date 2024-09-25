using LuaSharp;
using System.Text;

namespace Test
{
    public class LexerTest
    {
        [Fact]
        public void ShouldIdentifyValidIdentifiers()
        {
            string input = "_validIdentifier valid123 my_var " +
                           "_tempVar _3var 1invalid 123test";

            var output = new List<Token>();
            byte[] byteArray = Encoding.ASCII.GetBytes(input);
            var stream = new MemoryStream(byteArray);
            var lexer = new Lexer(new StreamReader(stream), "test.lua");

            while (true)
            {
                Token tok = lexer.NextToken();
                output.Add(tok);
                if (tok.Type == TokenType.EOF) break;
            }

            var expectedOutcome = new List<Token>
            {
                new (TokenType.IDENTIFIER, "_validIdentifier", 1, 1, "test.lua"),
                new (TokenType.IDENTIFIER, "valid123", 1, 18, "test.lua"),
                new (TokenType.IDENTIFIER, "my_var", 1, 27, "test.lua"),
                new (TokenType.IDENTIFIER, "_tempVar", 1, 34, "test.lua"),
                new (TokenType.IDENTIFIER, "_3var", 1, 43, "test.lua"),

                new (TokenType.ILLEGAL, "1invalid", 1, 49, "test.lua"),
                new (TokenType.ILLEGAL, "123test", 1, 58, "test.lua"),

                new (TokenType.EOF, "EOF", 1, 65, "test.lua")
            };

            Assert.Equal(expectedOutcome.Count, output.Count);
            for (int i = 0; i < expectedOutcome.Count; i++)
            {
                Assert.Equal(expectedOutcome[i].Type, output[i].Type);
                Assert.Equal(expectedOutcome[i].Literal, output[i].Literal);
                Assert.Equal(expectedOutcome[i].Line, output[i].Line);
                Assert.Equal(expectedOutcome[i].Column, output[i].Column);
                Assert.Equal(expectedOutcome[i].FileName, output[i].FileName);
            }
        }

        [Fact]
        public void ShouldIdentifyKeywords()
        {
            string input = "and break do else elseif end false for function " +
                "goto if in local nil not or repeat return then true until while";

            var output = new List<Token>();
            byte[] byteArray = Encoding.ASCII.GetBytes(input);
            var stream = new MemoryStream(byteArray);
            var lexer = new Lexer(new StreamReader(stream), "test.lua");
            while (true)
            {
                Token tok = lexer.NextToken();
                output.Add(tok);
                if (tok.Type == TokenType.EOF) break;
            }

            var expectedOutcome = new List<Token>
            {
                new (TokenType.AND, "and", 1, 1, "test.lua"),
                new (TokenType.BREAK, "break", 1, 5, "test.lua"),
                new (TokenType.DO, "do", 1, 11, "test.lua"),
                new (TokenType.ELSE, "else", 1, 14, "test.lua"),
                new (TokenType.ELSEIF, "elseif", 1, 19, "test.lua"),
                new (TokenType.END, "end", 1, 26, "test.lua"),
                new (TokenType.FALSE, "false", 1, 30, "test.lua"),
                new (TokenType.FOR, "for", 1, 36, "test.lua"),
                new (TokenType.FUNCTION, "function", 1, 40, "test.lua"),
                new (TokenType.GOTO, "goto", 1, 49, "test.lua"),
                new (TokenType.IF, "if", 1, 54, "test.lua"),
                new (TokenType.IN, "in", 1, 57, "test.lua"),
                new (TokenType.LOCAL, "local", 1, 60, "test.lua"),
                new (TokenType.NIL, "nil", 1, 66, "test.lua"),
                new (TokenType.NOT, "not", 1, 70, "test.lua"),
                new (TokenType.OR, "or", 1, 74, "test.lua"),
                new (TokenType.REPEAT, "repeat", 1, 77, "test.lua"),
                new (TokenType.RETURN, "return", 1, 84, "test.lua"),
                new (TokenType.THEN, "then", 1, 91, "test.lua"),
                new (TokenType.TRUE, "true", 1, 96, "test.lua"),
                new (TokenType.UNTIL, "until", 1, 101, "test.lua"),
                new (TokenType.WHILE, "while", 1, 107, "test.lua"),
                new (TokenType.EOF, "EOF", 1, 112, "test.lua")
            };

            Assert.Equal(expectedOutcome.Count, output.Count);
            for (int i = 0; i < expectedOutcome.Count; i++)
            {
                Assert.Equal(expectedOutcome[i].Type, output[i].Type);
                Assert.Equal(expectedOutcome[i].Literal, output[i].Literal);
                Assert.Equal(expectedOutcome[i].Line, output[i].Line);
                Assert.Equal(expectedOutcome[i].Column, output[i].Column);
                Assert.Equal(expectedOutcome[i].FileName, output[i].FileName);
            }
        }

        [Fact]
        public void ShouldIdentifyOperators()
        {
            string input = "+ - * / % ^ # & ~ | << >> // == " +
                "~= <= >= < > = ( ) { } [ ] :: : , . .. ...";

            var output = new List<Token>();
            byte[] byteArray = Encoding.ASCII.GetBytes(input);
            var stream = new MemoryStream(byteArray);
            var lexer = new Lexer(new StreamReader(stream), "test.lua");
            while (true)
            {
                Token tok = lexer.NextToken();
                output.Add(tok);
                if (tok.Type == TokenType.EOF) break;
            }

            var expectedOutcome = new List<Token>
            {
                new (TokenType.PLUS, "+", 1, 1, "test.lua"),
                new (TokenType.MINUS, "-", 1, 3, "test.lua"),
                new (TokenType.ASTERISK, "*", 1, 5, "test.lua"),
                new (TokenType.SLASH, "/", 1, 7, "test.lua"),
                new (TokenType.PERCENTAGE, "%", 1, 9, "test.lua"),
                new (TokenType.CARET, "^", 1, 11, "test.lua"),
                new (TokenType.HASHTAG, "#", 1, 13, "test.lua"),
                new (TokenType.B_AND, "&", 1, 15, "test.lua"),
                new (TokenType.TILDE, "~", 1, 17, "test.lua"),
                new (TokenType.B_OR, "|", 1, 19, "test.lua"),
                new (TokenType.B_LSHIFT, "<<", 1, 21, "test.lua"),
                new (TokenType.B_RSHIFT, ">>", 1, 24, "test.lua"),
                new (TokenType.F_DIV, "//", 1, 27, "test.lua"),
                new (TokenType.EQUAL, "==", 1, 30, "test.lua"),
                new (TokenType.NOT_EQUAL, "~=", 1, 33, "test.lua"),
                new (TokenType.LESS_EQUAL, "<=", 1, 36, "test.lua"),
                new (TokenType.MORE_EQUAL, ">=", 1, 39, "test.lua"),
                new (TokenType.LESS, "<", 1, 42, "test.lua"),
                new (TokenType.MORE, ">", 1, 44, "test.lua"),
                new (TokenType.ASSIGN, "=", 1, 46, "test.lua"),
                new (TokenType.L_PARENT, "(", 1, 48, "test.lua"),
                new (TokenType.R_PARENT, ")", 1, 50, "test.lua"),
                new (TokenType.L_CURLY, "{", 1, 52, "test.lua"),
                new (TokenType.R_CURLY, "}", 1, 54, "test.lua"),
                new (TokenType.L_SQUARE, "[", 1, 56, "test.lua"),
                new (TokenType.R_SQUARE, "]", 1, 58, "test.lua"),
                new (TokenType.LABEL, "::", 1, 60, "test.lua"),
                new (TokenType.COLON, ":", 1, 63, "test.lua"),
                new (TokenType.COMMA, ",", 1, 65, "test.lua"),
                new (TokenType.DOT, ".", 1, 67, "test.lua"),
                new (TokenType.CONCAT, "..", 1, 69, "test.lua"),
                new (TokenType.VARARG, "...", 1, 72, "test.lua"),
                new (TokenType.EOF, "EOF", 1, 75, "test.lua")
            };

            Assert.Equal(expectedOutcome.Count, output.Count);
            for (int i = 0; i < expectedOutcome.Count; i++)
            {
                Assert.Equal(expectedOutcome[i].Type, output[i].Type);
                Assert.Equal(expectedOutcome[i].Literal, output[i].Literal);
                Assert.Equal(expectedOutcome[i].Line, output[i].Line);
                Assert.Equal(expectedOutcome[i].Column, output[i].Column);
                Assert.Equal(expectedOutcome[i].FileName, output[i].FileName);
            }
        }

        [Fact]
        public void ShouldIdentifyValidStrings()
        {
            string input = "'string'\r\n" +
                "\"test\\n123\"\r\n";

            var output = new List<Token>();
            byte[] byteArray = Encoding.ASCII.GetBytes(input);
            var stream = new MemoryStream(byteArray);
            var lexer = new Lexer(new StreamReader(stream), "test.lua");

            while (true)
            {
                Token tok = lexer.NextToken();
                output.Add(tok);
                if (tok.Type == TokenType.EOF) break;
            }

            var expectedOutcome = new List<Token>
            {
                new (TokenType.STRING, "'string'", 1, 1, "test.lua"),
                new (TokenType.NEWLINE, "", 1, 9, "test.lua"),

                new (TokenType.STRING, "\"test\\n123\"", 2, 1, "test.lua"),
                new (TokenType.NEWLINE, "", 2, 12, "test.lua"),

                new (TokenType.EOF, "EOF", 3, 1, "test.lua")
            };

            Assert.Equal(expectedOutcome.Count, output.Count);
            for (int i = 0; i < expectedOutcome.Count; i++)
            {
                Assert.Equal(expectedOutcome[i].Type, output[i].Type);
                Assert.Equal(expectedOutcome[i].Literal, output[i].Literal);
                Assert.Equal(expectedOutcome[i].Line, output[i].Line);
                Assert.Equal(expectedOutcome[i].Column, output[i].Column);
                Assert.Equal(expectedOutcome[i].FileName, output[i].FileName);
            }
        }

        [Fact]
        public void ShouldIdentifyValidNumber()
        {
            string input = "3 345 0xff 0xBEBADA " +
                "3.0 3.1416 314.16e-2 0.31416E1 34e1";

            var output = new List<Token>();
            byte[] byteArray = Encoding.ASCII.GetBytes(input);
            var stream = new MemoryStream(byteArray);
            var lexer = new Lexer(new StreamReader(stream), "test.lua");

            while (true)
            {
                Token tok = lexer.NextToken();
                output.Add(tok);
                if (tok.Type == TokenType.EOF) break;
            }

            var expectedOutcome = new List<Token>
            {
                new (TokenType.NUMERICAL, "3", 1, 1, "test.lua"),
                new (TokenType.NUMERICAL, "345", 1, 3, "test.lua"),
                new (TokenType.NUMERICAL, "0xff", 1, 7, "test.lua"),
                new (TokenType.NUMERICAL, "0xBEBADA", 1, 12, "test.lua"),

                new (TokenType.NUMERICAL, "3.0", 1, 21, "test.lua"),
                new (TokenType.NUMERICAL, "3.1416", 1, 25, "test.lua"),
                new (TokenType.NUMERICAL, "314.16e-2", 1, 32, "test.lua"),
                new (TokenType.NUMERICAL, "0.31416E1", 1, 42, "test.lua"),
                new (TokenType.NUMERICAL, "34e1", 1, 52, "test.lua"),

                new (TokenType.EOF, "EOF", 1, 56, "test.lua")
            };

            Assert.Equal(expectedOutcome.Count, output.Count);
            for (int i = 0; i < expectedOutcome.Count; i++)
            {
                Assert.Equal(expectedOutcome[i].Type, output[i].Type);
                Assert.Equal(expectedOutcome[i].Literal, output[i].Literal);
                Assert.Equal(expectedOutcome[i].Line, output[i].Line);
                Assert.Equal(expectedOutcome[i].Column, output[i].Column);
                Assert.Equal(expectedOutcome[i].FileName, output[i].FileName);
            }
        }
    }
}