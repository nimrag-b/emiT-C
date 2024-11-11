using emiT_C;
using System.IO;

internal class Program
{
    private static void Main(string[] args)
    {

        string src;
        if (args.Length == 0)
        {
            string exefolder = System.Reflection.Assembly.GetEntryAssembly().Location;
            src = File.ReadAllText(Path.GetFullPath(Path.Combine(exefolder, "..", "..", "..", "..","..","examples","example.emit")));
        }
        else
        {
            src = File.ReadAllText(args[0]);  
        }

        Console.WriteLine("Starting Primary Timeline...");
        Lexer lexer = new Lexer();
        List<Token> tokens = lexer.Tokenize(src);

        //foreach (var item in tokens)
        //{
        //    Console.WriteLine(item);
        //}

        Parser parser = new Parser();

        List<Statement> statements = parser.Parse(tokens);

        Timeline original = new Timeline(new Dictionary<string, eVariable>(), new Dictionary<string, eTime>(), statements, 0);

        Console.WriteLine("Timelines Created: " +original.Run());
    }
}