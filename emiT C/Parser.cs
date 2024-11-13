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

            if(At().type == TokenType.OpenBracket)
            {
                return ParseCodeBlock(); //parse code block
            }

            Statement stat = ParseStatement(); //parse statement normally

            if(stat is IActorStmt) //if statement is tagged as an actor, dont require a semicolon to end
            {
                //todo - check wether there is a valid expression next in the block
                return stat;
            }
            else if(At().type == TokenType.SemiColon)
            {
                Eat();
                return stat;
            }

            Console.WriteLine(At().symbol);

            throw new Exception($"Unclosed Statement at line {At().line}: {stat}");
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
                case TokenType.Collapse:
                    Eat();
                    return new CollapseStmt();


                case TokenType.OpenBracket: //parse code block
                    return ParseCodeBlock();
            }

            //for expressions that start with the variable
            ExprVariable lhs = ParseVariable();

            switch (At().type)
            {
                case TokenType.Assign:
                    return ParseAssignment(lhs);
                case TokenType.Kills:
                    return ParseKills(lhs);
                case TokenType.Warps:
                    return ParseWarps(lhs);
            }

            throw new Exception($"Unfinished Statement {lhs} at line {At().line}");
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

            return new BranchStmt(point,lhs);
        }

        Statement ParseKills(ExprVariable lhs)
        {
            Expect(TokenType.Kills);
            return new KillsStmt(lhs, new ExprVariable(Eat().symbol));
        }

        Statement ParseAssignment(ExprVariable lhs)
        {
            Expect(TokenType.Assign);
            if (IsType(At().type))
            {
                return new AssignmentStmt(lhs, ParseArray());
            }
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
            if (IsType(At().type))
            {
                return new CreateStmt(name.symbol, ParseArray());
            }
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

            return new IfStmt(condition);
        }

        Statement ParseCodeBlock()
        {
            List<Statement> statements = new List<Statement>();
            Expect(TokenType.OpenBracket);
            while (At().type != TokenType.CloseBracket)
            {
                statements.Add(ParseLine());
            }
            Eat();
            return new CodeBlockStmt(statements);
        }

        ExprLiteral ParseString()
        {
            string str = (string)(Eat().value);

            eArray eStr = new eArray(Type.Char, str.Length);

            for (int i = 0; i < str.Length; i++)
            {
                eStr.inner[i] = new eValue(Type.Char, str[i]);
            }

            return new ExprLiteral(eStr, Type.Array);
        }


        Expression ParseArray()
        {
            Type type = GetType(Eat().type);

            if (At().type == TokenType.OpenSquare)
            {
                return new ArrayExpr(ParseIndex(), type);
            }

            throw new Exception("Unclosed Statement at line " + At().line);
        }

        Expression ParseLiteral()
        {
            switch (At().type)
            {
                case TokenType.StringLiteral:
                    return ParseString();
                case TokenType.CharLiteral:
                    return new ExprLiteral((char)Eat().value, Type.Char);
                case TokenType.IntLiteral:
                    return new ExprLiteral((int)Eat().value, Type.Int);
                case TokenType.FloatLiteral:
                    return new ExprLiteral((float)Eat().value, Type.Float);
                case TokenType.BoolLiteralFalse:
                    Eat();
                    return new ExprLiteral(false, Type.Bool);
                case TokenType.BoolLiteralTrue:
                    Eat();
                    return new ExprLiteral(true, Type.Bool);
            }

            return ParseVariable();
        }

        ExprVariable ParseVariable()
        {
            
            if(At().type == TokenType.Symbol)
            {
                string symbol = Eat().symbol;

                if (At().type == TokenType.OpenSquare)
                {
                    Expression index = ParseIndex();

                    return new IndexExpr(symbol, index);
                }
                ExprVariable var = new ExprVariable(symbol);

                return var;
            }

            throw new Exception($"Unexpected token {At().type} at line {At().line}");
        }
        Expression ParsePrimary()
        {
            return ParseBool();
        }
        Expression ParseBool()
        {
            Expression lhs = ParseBinaryExpr(0);

            if(IsBooleanOp(At().type))
            {
                return ParseBooleanExpr(lhs);
            }
            else if(At().type == TokenType.Is)
            {
                return ParseIsStmt((ExprVariable)lhs);
            }
            return lhs;
            //throw new Exception("Invalid Boolean Operation");
        }

        Expression ParseIsStmt(ExprVariable lhs)
        {
            Expect(TokenType.Is);
            return new IsExpr(lhs, GetVarProperty(Eat().type));
        }

        Expression ParseBooleanExpr(Expression lhs)
        {
            Operand op = GetOperand(Eat().type);

            Expression rhs = ParseBinaryExpr(0);

            return new BooleanExpr(lhs, rhs, op);
        }

        public Expression ParseIndex()
        {
            Eat();
            Expression index = ParseBinaryExpr(0);
            Expect(TokenType.CloseSquare);
            return index;
        }

        public Expression ParseBinaryExpr(int minPrec)
        {
            Token op = At();
            Expression lhs;

            if (IsBinaryOp(op.type))
            {
                int rbp = PrefixBindingPower(op.type);
                Eat();
                Expression rhs = ParseBinaryExpr(rbp);

                lhs = new UnaryExpr(rhs, GetOperand(op.type));
            }
            else if(op.type == TokenType.OpenParen)
            {
                Eat();
                lhs = ParseBinaryExpr(0);
                Expect(TokenType.CloseParen);    
            }
            else
            {
                lhs = ParseLiteral();
            }


            while (true)
            {
                op = At();
                if(op.type == TokenType.EOF || op.type == TokenType.SemiColon)
                {
                    break;
                }

                (int, int)? bp = InfixBindingPower(op.type);

                if (bp.HasValue)
                {
                    if (bp.Value.Item1 < minPrec)
                    {
                        break;
                    }
                    Eat();
                    Expression rhs = ParseBinaryExpr(bp.Value.Item2);

                    lhs = new BinaryExpr(lhs, rhs, GetOperand(op.type));
                    continue;
                }

                break;


            }
            return lhs;
        }



        int PrefixBindingPower(TokenType op)
        {
            switch (op)
            {
                case TokenType.Subtract:
                case TokenType.Add:
                    return 7;
                default:
                    throw new Exception("Invalid operator:" + op);
            }
        }

        (int,int)? InfixBindingPower(TokenType op)
        {
            switch (op)
            {
                case TokenType.Subtract:
                case TokenType.Add:
                    return (1,2);

                case TokenType.Modulus:
                    return (3,4);

                case TokenType.Divide:
                case TokenType.Multiply:
                    return (5,6);

                default:
                    return null;
            }
        }

        public Operand GetOperand(TokenType op)
        {
            switch (op)
            {
                case TokenType.Add:
                    return Operand.Add;
                case TokenType.Subtract:
                    return Operand.Subtract;
                case TokenType.Divide:
                    return Operand.Divide;
                case TokenType.Multiply:
                    return Operand.Multiply;
                case TokenType.Modulus:
                    return Operand.Modulus;

                case TokenType.Equals:
                    return Operand.Equals;
                case TokenType.Less:
                    return Operand.Less;
                case TokenType.Greater:
                    return Operand.Greater;
                case TokenType.GreaterOrEqual:
                    return Operand.GreaterOrEqual;
                case TokenType.LessOrEqual:
                    return Operand.LessOrEqual;
                default:
                    throw new Exception("unrecognised operator:" + op);
            }
        }

        public Type GetType(TokenType token)
        {
            switch (token)
            {
                case TokenType.IntLiteral:
                case TokenType.IntType:
                    return Type.Int;
                case TokenType.FloatLiteral:
                case TokenType.FloatType:
                    return Type.Float;
                case TokenType.BoolLiteralFalse:
                case TokenType.BoolLiteralTrue:
                case TokenType.BoolType:
                    return Type.Bool;
                case TokenType.CharLiteral:
                case TokenType.CharType:
                    return Type.Char;
                default:
                    throw new Exception("unrecognised type:" + token);
            }
        }

        public bool IsType(TokenType token)
        {
            switch (token)
            {
                case TokenType.IntType:
                case TokenType.FloatType:
                case TokenType.BoolType:
                case TokenType.CharType:
                    return true;
            }
            return false;
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

        public bool IsBinaryOp(TokenType token)
        {
            switch (token)
            {
                case TokenType.Add:
                case TokenType.Subtract:
                case TokenType.Multiply:
                case TokenType.Divide:
                case TokenType.Modulus:
                    return true;

                default:
                    return false;
            }
        }
        public bool IsBooleanOp(TokenType token)
        {
            switch (token)
            {
                case TokenType.Equals:
                case TokenType.Greater:
                case TokenType.Less:
                case TokenType.LessOrEqual:
                case TokenType.GreaterOrEqual:
                case TokenType.NotEquals:
                    return true;

                default:
                    return false;
            }
        }
    }
}
