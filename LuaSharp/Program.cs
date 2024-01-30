using LuaSharp;

class Program
{
    public static void Main(string[] args)
    {
        if (args.Length != 1)
        {
            Console.WriteLine("No file specified!");
            return;
        }

        string path = args[0];

        try
        {
            if (!File.Exists(path))
            {
                Console.WriteLine("Invalid path to a file.");
                return;
            }

            using (StreamReader sr = new StreamReader(path))
            {
                Lexer lexer = new Lexer(sr);

                while (true)
                {
                    Token tok = lexer.NextToken();
                    Console.WriteLine(tok.ToString());
                    if (tok.Type == TokenType.EOF) break;
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Exception: " + e.Message);
        }
    }
}