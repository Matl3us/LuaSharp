
string? line;

try
{
    StreamReader sr = new StreamReader("D:\\Interpreter\\LuaSharp\\LuaSharp\\hello.lua");
    line = sr.ReadLine();
    while (line != null)
    {
        Console.WriteLine(line);
        line = sr.ReadLine();
    }
    sr.Close();
    Console.ReadLine();
}
catch (Exception e)
{
    Console.WriteLine("Exception: " + e.Message);
}
finally
{
    Console.WriteLine("Executing finally block.");
}