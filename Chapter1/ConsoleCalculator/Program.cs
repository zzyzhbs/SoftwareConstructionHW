using System;

namespace ConsoleCalculator
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("Please input the two operands in different lines");
            double A, B, C;
            A = double.Parse(Console.ReadLine());
            B = double.Parse(Console.ReadLine());
            Console.WriteLine("Please input the operator");
            char op = char.Parse(Console.ReadLine());
            switch (op)
            {
                case '+': C = A + B; break;
                case '-': C = A - B; break;
                case '*': C = A * B; break;
                case '/':
                    if (Math.Abs(B) > 1e-15)
                    {
                        C = A / B;
                    } 
                    else
                    {
                        Console.WriteLine("B cannot be 0!\nPress to exit");
                        Console.ReadLine();
                        return;
                    }
                    break;
                default:
                    Console.WriteLine("Invalid operator!\nPress to exit");
                    Console.ReadLine();
                    return;
            }
            Console.WriteLine($"The result of {A} {op} {B} is {C}");
            Console.ReadLine();
        }
    }
}