using emiT_C;
using emiT_C.Legacy;
using System.IO;
using Token = emiT_C.Token;

internal class Program
{
    private static void Main(string[] args)
    {

        string src = null;


#if DEBUG
        if (args.Length == 0)
        {
            string exefolder = System.Reflection.Assembly.GetEntryAssembly().Location;
            src = File.ReadAllText(Path.GetFullPath(Path.Combine(exefolder, "..", "..", "..", "..","..","examples", "fizzbuzz.emit")));
        }
        else
        {
            src = File.ReadAllText(args[0]);
        }
#elif RELEASE
        if(args.Length != 0)
        {
            src = File.ReadAllText(args[0]);
        }

#endif

        if(src == null)
        {
            Console.WriteLine("No Source Code provided");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
            return;
        }
        Console.WriteLine("Starting Primary Timeline...");
        ImprovedLexer lexer = new ImprovedLexer(src);
        List<Token> tokens = lexer.Tokenize();

        //foreach (var item in tokens)
        //{
        //    Console.WriteLine(item);
        //}

        Parser parser = new Parser();

        List<Statement> statements = parser.Parse(tokens);

        //foreach (var item in statements)
        //{
        //    Console.WriteLine(item);
        //}
        Multiverse multiverse = new Multiverse();

        Timeline original = new Timeline(multiverse, new Dictionary<string, eVariable>(), new Dictionary<string, eTime>(), new CodeBlockStmt(statements), 0, 0);



        multiverse.Run(original);
        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
    }
}