using System;

namespace ArrayInfo
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Please input the length of the array:");
            int n;
            n = int.Parse(Console.ReadLine());
            Console.WriteLine("Please input each element in different lines:");
            int[] a = new int[n];
            for (int i = 0; i < n; ++i)
                a[i] = int.Parse(Console.ReadLine());
            GetArrayInfo(a, n, out int mxNum, out int mnNum, out double Ave);
            Console.WriteLine($"The max number is {mxNum}, min number is {mnNum}, average value is {Ave}");

        }
        static void GetArrayInfo(int[] a, int len, out int mxNum, out int mnNum, out double Ave)
        {
            mxNum = int.MinValue;
            mnNum = int.MaxValue;
            Ave = 0;
            foreach(int k in a)
            {
                mxNum = Math.Max(k, mxNum);
                mnNum = Math.Min(k, mnNum);
                Ave += k;
            }
            Ave /= len;
        }
    }
}
