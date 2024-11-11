using emiT_C;

internal class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("Starting Primary Timeline...");
        Lexer lexer = new Lexer();
        List<Token> tokens = lexer.Tokenize("""
            create x = 10;

            x = x - 3;

            create y;

            time strike;

            y kills x;

            create traveler;

            print(x); //timeline 1 - nothing since x is dead. timeline 2 - prints 7 since traveler stopped x from being killed.

            if(x is dead){

                traveler warps strike{
                    //traveler waits strike;
                    traveler kills y;
                    traveler kills traveler; //kill self, preventing paradoxes by interfering with itself
                };

            };
            """);

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