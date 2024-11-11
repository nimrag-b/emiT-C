using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace emiT_C
{
    public class Parser
    {
        Queue<Token> queue;
        public List<Statement> Parse(List<Token> tokens)
        {
            queue = new Queue<Token>(tokens);
            List<Statement> result = new List<Statement>();

            while (At().type != TokenType.EOF)
            {
                result.Add(ParseLine());
            }

            return result;
        }

        Token Eat()
        {
            return queue.Dequeue();
        }

        Token At()
        {
            return queue.Peek();
        }
        Token Expect(TokenType type)
        {
            Token t = Eat();
            if (t.type != type)
            {
                throw new Exception("Expected token " + type + ". Got token " + t.type);
            }
            return t;
        }

        Statement ParseLine()
        {
            Statement stat = ParseStatement();

            Expect(TokenType.SemiColon);

            return stat;
        }

        Statement ParseStatement()
        {
            switch (At().type)
            {
                case TokenType.Create:
                    return ParseCreate();
                case TokenType.Time:
                    return ParseTime();
                case TokenType.If: 
                    return ParseIf();
                case TokenType.Print:
                    return ParsePrint();
            }

            //for expressions that start with the variable
            ExprVariable lhs = (ExprVariable)ParseToken();

            switch (At().type)
            {
                case TokenType.Assign:
                    return ParseAssignment(lhs);
                case TokenType.Kills:
                    return ParseKills(lhs);
                case TokenType.Warps:
                    return ParseWarps(lhs);
            }

            throw new Exception("Unfinished Statement: " + lhs);
        }

        Statement ParsePrint()
        {
            Expect(TokenType.Print);

            return new PrintStmt(ParsePrimary());

        }

        Statement ParseWarps(ExprVariable lhs)
        {
            Expect(TokenType.Warps);
            string point = Eat().symbol;


            List<Statement> statements = ParseCodeBlock();

            return new BranchStmt(point,lhs,statements);
        }

        Statement ParseKills(ExprVariable lhs)
        {
            Expect(TokenType.Kills);
            return new KillsStmt(lhs, new ExprVariable(Eat().symbol));
        }

        Statement ParseAssignment(ExprVariable lhs)
        {
            Expect(TokenType.Assign);
            return new AssignmentStmt(lhs, ParseBinaryExpr(0));
        }

        Statement ParseCreate()
        {
            Eat();
            Token name = Eat(); //dont eat the identifier
            if(At().type == TokenType.SemiColon)
            {
                return new CreateStmt(name.symbol, null);
            }
            Expect(TokenType.Assign);
            return new CreateStmt(name.symbol, ParseBinaryExpr(0));

        }
        

        Statement ParseTime()
        {
            Eat();
            Token name = Expect(TokenType.Symbol);
            return new CreateTime(name.symbol);
        }

        Statement ParseIf()
        {
            Expect(TokenType.If);
            Expect(TokenType.OpenParen);
            ExprBool condition = (ExprBool)ParseBool();
            Expect(TokenType.CloseParen);

            List<Statement> statements = ParseCodeBlock();

            return new IfStmt(condition, statements);
        }

        List<Statement> ParseCodeBlock()
        {
            List<Statement> statements = new List<Statement>();
            Expect(TokenType.OpenBracket);
            while (At().type != TokenType.CloseBracket)
            {
                statements.Add(ParseLine());
            }
            Eat();
            return statements;
        }


        Expression ParseToken()
        {
            switch (At().type)
            {
                case TokenType.EOF:
                    break;

                case TokenType.Symbol:
                    return new ExprVariable(Eat().symbol);
                case TokenType.Int:
                case TokenType.Float:
                case TokenType.String:
                    return ParseLiteral();


                   
            }

            throw new Exception("Unrecognized token: "+ At().type);
        }



        Expression ParseLiteral()
        {
            switch (At().type)
            {
                case TokenType.Int:
                    return new ExprLiteral(int.Parse(At().symbol), GetType(Eat().type));
                case TokenType.Float:
                    return new ExprLiteral(float.Parse(At().symbol), GetType(Eat().type));
                case TokenType.Bool:
                    return new ExprLiteral(bool.Parse(At().symbol), GetType(Eat().type));
            }

            throw new Exception("Literal type not found");
        }
        Expression ParsePrimary()
        {
            return ParseBool();
        }
        Expression ParseBool()
        {
            Expression lhs = ParseBinaryExpr(0);
            switch (At().type)
            {
                case TokenType.BooleanOp:
                    return ParseBooleanExpr(lhs);
                case TokenType.Is:
                    return ParseIsStmt((ExprVariable)lhs);
            }
            return lhs;
            //throw new Exception("Invalid Boolean Operation");
        }

        Expression ParseIsStmt(ExprVariable lhs)
        {
            Expect(TokenType.Is);
            return new IsStmt(lhs, GetVarProperty(Eat().type));
        }

        Expression ParseBooleanExpr(Expression lhs)
        {
            Operand op = GetOperand(Expect(TokenType.BooleanOp).symbol);

            Expression rhs = ParseBinaryExpr(0);

            return new BooleanExpr(lhs, rhs, op);
        }

        public Expression ParseBinaryExpr(int minPrec)
        {
            Token op = At();
            Expression lhs;

            if (op.type == TokenType.BinaryOp)
            {
                int rbp = PrefixBindingPower(op.symbol);
                Eat();
                Expression rhs = ParseBinaryExpr(rbp);

                lhs = new UnaryExpr(rhs, GetOperand(op.symbol));
            }
            else if(op.type == TokenType.OpenParen)
            {
                Eat();
                lhs = ParseBinaryExpr(0);
                Expect(TokenType.CloseParen);    
            }
            else
            {
                lhs = ParseToken();
            }


            while (true)
            {
                op = At();
                if(op.type == TokenType.EOF || op.type == TokenType.SemiColon)
                {
                    break;
                }

                (int, int)? bp = InfixBindingPower(op.symbol);

                if (bp.HasValue)
                {
                    if (bp.Value.Item1 < minPrec)
                    {
                        break;
                    }
                    Eat();
                    Expression rhs = ParseBinaryExpr(bp.Value.Item2);

                    lhs = new BinaryExpr(lhs, rhs, GetOperand(op.symbol));
                    continue;
                }

                break;


            }
            return lhs;
        }



        int PrefixBindingPower(string op)
        {
            switch (op)
            {
                case "-":
                case "+":
                    return 5;
                default:
                    throw new Exception("Invalid operator:" + op);
            }
        }

        (int,int)? InfixBindingPower(string op)
        {
            switch (op)
            {
                case "-":
                case "+":
                    return (1,2);

                case "/":
                case "*":
                    return (3,4);
                default:
                    return null;
            }
        }

        public Operand GetOperand(string op)
        {
            switch (op)
            {
                case "-":
                    return Operand.Subtract;
                case "+":
                    return Operand.Add;
                case "/":
                    return Operand.Divide;
                case "*":
                    return Operand.Multiply;

                case "==":
                    return Operand.Equals;
                case ">":
                    return Operand.Less;
                case "<":
                    return Operand.Greater;
                default:
                    throw new Exception("unrecognised operator:" + op);
            }
        }

        public Type GetType(TokenType token)
        {
            switch (token)
            {
                case TokenType.Int:
                    return Type.Int;
                case TokenType.Float:
                    return Type.Float;
                case TokenType.Bool:
                    return Type.Bool;
                default:
                    throw new Exception("unrecognised type:" + token);
            }
        }

        public VarProperty GetVarProperty(TokenType token)
        {
            switch (token)
            {
                case TokenType.Dead:
                    return VarProperty.Dead;
                case TokenType.Alive:
                    return VarProperty.Alive;
                case TokenType.Exists:
                    return VarProperty.Exists;
                default:
                    throw new Exception("unrecognised type:" + token);
            }
        }
    }
}
