using System.Text;
using LuaSharp;

class Program
{
    public static void Main(string[] args)
    {
        if (args.Length < 1)
        {
            Console.WriteLine("\nNo arguments selected. Please select correct argument:");
            Console.WriteLine("> -c for console mode");
            Console.WriteLine("> -f <path to file> for file mode\n");
            return;
        }

        switch (args[0])
        {
            case "-c":

                string prompt = ">> ";
                Console.WriteLine("Console mode activated. Type 'exit' to exit the program.");
                while (true)
                {
                    Console.Write(prompt);

                    string? s = Console.ReadLine();
                    if (s == null) return;
                    if (s == "exit") return;

                    byte[] byteArray = Encoding.ASCII.GetBytes(s);
                    MemoryStream stream = new MemoryStream(byteArray);

                    Lexer lexer = new Lexer(new StreamReader(stream));
                    Parser parser = new Parser(lexer);
                    AST ast = parser.ParseCode();
                    ast.PrintStatements();
                }

            case "-f":
                if (args.Length != 2)
                {
                    Console.WriteLine("\nNo file selected. Please select a file with -f <path to file>\n");
                    return;
                }

                string path = args[1];

                try
                {
                    if (!File.Exists(path))
                    {
                        Console.WriteLine("\nInvalid path to a file.\n");
                        return;
                    }

                    using (StreamReader sr = new StreamReader(path))
                    {
                        Lexer lexer = new Lexer(sr);
                        Parser parser = new Parser(lexer);
                        AST ast = parser.ParseCode();
                        ast.PrintStatements();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception: " + e.Message);
                }

                break;
            default:
                Console.WriteLine("\nIncorrect option selected. Please select correct argument:");
                Console.WriteLine("> -c for console mode");
                Console.WriteLine("> -f <path to file> for file mode\n");
                break;
        }
    }
}