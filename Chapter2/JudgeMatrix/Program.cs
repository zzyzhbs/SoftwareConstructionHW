using System;

namespace JudgeMatrix
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Please input the size (n and m) of the matrix:");
            int n = int.Parse(Console.ReadLine());
            int m = int.Parse(Console.ReadLine());
            int[,] M = new int[n + 10, m + 10];
            Console.WriteLine("Please input each element in different lines");
            for (int i = 0; i < n; ++i)
                for (int j = 0; j < m; ++j)
                {
                    M[i,j] = int.Parse(Console.ReadLine()); //按行优先顺序依次读入数组元素
                }
            Console.WriteLine("The result of the judgement is " + Judge(M, n, m));
        }
        static bool Judge(int[,] M, int n, int m)
        {
            for (int del = 1 - m; del < n; ++del) //用del=(i-j)定位对角线
            {
                int curVal = int.MinValue;
                for (int i = Math.Max(0, del), j = i - del; i < n && j < m; ++i, ++j) 
                {
                    //遍历del对应的对角线元素
                    if (curVal == int.MinValue) curVal = M[i,j];
                    if (curVal != M[i,j]) return false;
                }
            }
            return true;
        }
    }
}
