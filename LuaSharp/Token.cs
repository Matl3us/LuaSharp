using LuaSharp.AST.Expressions;
using System.Data.Common;

namespace LuaSharp
{
    public class Token
    {
        public TokenType Type { get; set; } = TokenType.ILLEGAL;
        public string Literal { get; set; } = "";
        public int Line { get; set; } = default;
        public int Column { get; set; } = default;
        public string FileName { get; set; } = "";
        public override string ToString() => $"[{Type},{Literal},{Line},{Column}]";

        public Token() { }

        public Token(TokenType type, string literal, int line, int column, string filename)
        {
            Type = type;
            Literal = literal;
            Line = line;
            Column = column;
            FileName = filename;
        }

        public Token SetType(TokenType type)
        {
            Type = type;
            return this;
        }

        public Token SetLiteral(string literal)
        {
            Literal = literal;
            return this;
        }

        public Token SetLine(int line)
        {
            Line = line;
            return this;
        }

        public Token SetColumn(int column)
        {
            Column = column;
            return this;
        }

        public Token SetFileName(string fileName)
        {
            FileName = fileName;
            return this;
        }
    }

    public enum TokenType
    {
        // * Keywords *
        BREAK,
        DO,
        ELSE,
        ELSEIF,
        END,
        FALSE,
        FOR,
        FUNCTION,
        GOTO,
        IF,
        IN,
        LOCAL,
        NIL,
        REPEAT,
        RETURN,
        THEN,
        TRUE,
        UNTIL,
        WHILE,

        // * Operators *
        // - Arithmetic Operators
        PLUS,
        MINUS,
        ASTERISK,
        SLASH,
        PERCENTAGE,
        CARET,

        // - Relational Operators
        EQUAL,
        NOT_EQUAL,
        LESS_EQUAL,
        MORE_EQUAL,
        LESS,
        MORE,

        // - Logical Operators
        AND,
        OR,
        NOT,

        // - Bitwise Operators
        B_AND,
        B_OR,
        TILDE,
        B_RSHIFT,
        B_LSHIFT,

        // - Other Operators
        HASHTAG,
        CONCAT,
        ASSIGN,
        F_DIV,
        LABEL,
        VARARG,

        // * Delimiters and Punctuation *
        L_PARENT,
        R_PARENT,
        L_CURLY,
        R_CURLY,
        L_SQUARE,
        R_SQUARE,
        SEMICOLON,
        COLON,
        COMMA,
        DOT,

        // * Literals *
        IDENTIFIER,
        NUMERICAL,
        STRING,

        // * Comment and Whitespace
        COMMENT,
        NEWLINE,

        // * Miscellaneous *
        ILLEGAL,
        EOF
    }
}