class Program
{
    public static void Main(string[] args)
    {
        foreach (var arg in args)
        {
            Console.WriteLine(arg);
        }

        string path = "D:\\Interpreter\\LuaSharp\\LuaSharp\\hello.lua";

        try
        {
            if (!File.Exists(path))
            {
                Console.WriteLine("Error no file under path" + path);
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