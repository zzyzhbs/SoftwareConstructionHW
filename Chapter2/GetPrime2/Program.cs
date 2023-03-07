using System;

namespace GetPrime2
{
    class Program
    {
        static void Main(string[] args)
        {
            bool[] Vis = new bool[101];
            GetPrimes(100, Vis);
            Console.WriteLine($"The prime numbers in [2, 100] are: ");
            for (int k = 2; k <= 100; ++k)
                if (!Vis[k]) Console.Write(k + " ");
        }
        static void GetPrimes(int n, bool[] Vis)
        {
            
            for (int k = 2; k <= n; ++k)
            {
                if (Vis[k]) continue;
                for (int i = 2; i * k <= n; ++i)
                    Vis[i * k] = true;

            }
        }
            
    }
}
