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
                while (sr.Peek() >= 0)
                {
                    Console.Write((char)sr.Read());
                }
            }

            Console.ReadLine();
        }
        catch (Exception e)
        {
            Console.WriteLine("Exception: " + e.Message);
        }
    }
}