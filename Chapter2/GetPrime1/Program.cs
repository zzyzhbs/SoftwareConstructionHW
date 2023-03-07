using System;
using System.Collections;

namespace GetPrime1
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Please input the number x:");
            int x;
            x = int.Parse(Console.ReadLine());
            ArrayList a = new ArrayList(); 
            GetPrime(x, ref a);
            Console.WriteLine($"The {a.Count} prime factors of x are:");
            foreach(int k in a)
            {
                Console.Write(k + " ");
            }
        }
        static void GetPrime(int x, ref ArrayList a) //通过变长数组实现传参
        {
            for (int k = 2; k <= x; ++k)
            {
                if (x % k != 0) continue;
                a.Add(k);
                while (x % k == 0) x /= k;
            }
        }
    }
}
