using System.Text;
using LuaSharp.AST;
using LuaSharp.Evaluation;

namespace LuaSharp
{
    static class Program
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
                        var stream = new MemoryStream(byteArray);

                        var lexer = new Lexer(new StreamReader(stream));
                        var parser = new Parser(lexer);
                        Ast ast = parser.ParseCode();

                        parser.PrintErrors();

                        var evaluated = Evaluator.Eval(ast);
                        Console.WriteLine(evaluated.InspectValue());
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

                        using var sr = new StreamReader(path);

                        var lexer = new Lexer(sr, path);
                        var parser = new Parser(lexer);
                        Ast ast = parser.ParseCode();

                        parser.PrintErrors();

                        var evaluated = Evaluator.Eval(ast);
                        Console.WriteLine(evaluated.InspectValue());
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
}