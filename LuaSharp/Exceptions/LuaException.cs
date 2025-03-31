namespace LuaSharp.Exceptions
{
    public enum ExceptionType
    {
        NUMERAL_PARSE_ERROR,
        BOOLEAN_PARSE_ERROR,
        GROUPED_EXPRESSION_PARSE_ERROR,
        BLOCK_STATEMENT_PARSE_ERROR,
        ASSIGN_STATEMENT_PARSE_ERROR,
        RETURN_STATEMENT_PARSE_ERROR,
        IF_STATEMENT_PARSE_ERROR,
        WHILE_STATEMENT_PARSE_ERROR,
        PEEK_ERROR,
        PREFIX_PARSE_ERROR
    }

#pragma warning disable S2166
    public class LuaException(ExceptionType code, string msg, int line, int column)
    {
        private readonly ExceptionType _code = code;
        private readonly string _msg = msg;
        private readonly int _line = line;
        private readonly int _column = column;

        public void PrintException()
        {
            Console.WriteLine($"Error - {_code}\nError at line {_line} column {_column}\n{_msg}\n");
        }
    }
#pragma warning restore S2166
}