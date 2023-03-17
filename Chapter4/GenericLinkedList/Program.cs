using System;
using System.Numerics;

namespace GenericLinkedList
{
    public class Node<T>
    {
        public Node<T>? Next
        {
            get; set;
        }
        public T data
        {
            get; set;
        }
        public Node(T t)
        {
            Next = null;
            data = t;
        }
    }
    // Abandoned inplement
    //public class GenericList<T> where T : struct, IComparable<T>, IAdditionOperators<T, T, T>
    //{
    //    // Use Generic Restriction to guarentee GetMax/Min and GetSum are valid
    //    private Node<T>? head;
    //    private Node<T>? tail;

    //    public GenericList()
    //    {
    //        head = tail = null;
    //    }
    //    public Node<T>? Head
    //    {
    //        get => head;
    //    }
    //    public void Add(T t)
    //    {
    //        Node<T> node = new Node<T>(t);
    //        if (tail == null) // Empty list
    //        {
    //            head = tail = node;
    //        }
    //        else
    //        {
    //            tail.Next = node;
    //            tail = node;
    //        }
    //    }
    //    public void ForEach(Action<T> Act) // Access each node's data, but hide other structures
    //    {
    //        if (tail == null) // Exception
    //        {
    //            Console.WriteLine("The list is empty!");
    //            throw new InvalidOperationException();
    //        }

    //        for (Node<T>? k = head; k != null; k = k.Next)
    //        {
    //            Act(k.data);
    //        }
    //    }
    //    public void Print()
    //    {
    //        try
    //        {
    //            ForEach(x => { }); // Test whether the list is empty
    //        }
    //        catch (InvalidOperationException)
    //        {
    //            return; //Empty list is acceptable
    //        }
    //        Console.WriteLine("-------\nThe elements in the list are as follows:");
    //        ForEach(x => { Console.WriteLine(x); });
    //        Console.WriteLine("-------");
    //    }
    //    public T GetMax()
    //    {
    //        T mx = default(T);
    //        try
    //        {
    //            // Use IComparable<T> method to compare T type objects
    //            ForEach(x => { mx = (mx.Equals(default(T))) ? x : (mx.CompareTo(x) < 0 ? x : mx); });
    //        }
    //        catch (InvalidOperationException e)
    //        {
    //            Console.WriteLine(e.Message);
    //            Environment.Exit(1); // Empty list here is unacceptable
    //        }
    //        return mx;
    //    }
    //    public T GetMin()
    //    {
    //        T mn = default(T);
    //        try
    //        {
    //            ForEach(x => { mn = (mn.Equals(default(T))) ? x : (mn.CompareTo(x) < 0 ? mn : x); });
    //        }
    //        catch (InvalidOperationException e)
    //        {
    //            Console.WriteLine(e.Message);
    //            Environment.Exit(1); // Empty list here is unacceptable
    //        }
    //        return mn;
    //    }
    //    // Summation
    //    // Method 1: Use the interface IAdditionOperators to calculate the summation directly
    //    public T GetSum()
    //    {
    //        T sum = default(T);

    //        try
    //        {
    //            ForEach(x => sum = sum + x);
    //        }
    //        catch (InvalidOperationException) { } // Empty list here is acceptable
    //        return sum;
    //    }

    //    // Method 2: use a static method only related to integer
    //    public static int GetIntListSum(GenericList<int> intList)
    //    {
    //        int sum = 0;
    //        try
    //        {
    //            intList.ForEach(x => sum += x);
    //        }
    //        catch (InvalidOperationException) { } // Empty list here is acceptable
    //        return sum;
    //    }
    //    public static void IntListInit(GenericList<int> intList, int n)
    //    {
    //        for (int i = 0; i < n; ++i)
    //            intList.Add(i);
    //    }

    public class GenericList<T> where T : IEquatable<T>
    {
        private Node<T>? head;
        private Node<T>? tail;
        int length;

        public GenericList()
        {
            head = tail = null;
            length = 0;
        }
        public Node<T>? Head
        {
            get => head;
        }
        public void Add(T t)
        {
            Node<T> node = new Node<T>(t);
            if (tail == null) // Empty list
            {
                head = tail = node;
            }
            else
            {
                tail.Next = node;
                tail = node;
            }
            ++length;
        }
        public void ForEach(Action<Node<T>> Act) // Access each node
        {
            if (tail == null) // Exception
            {
                Console.WriteLine("The list is empty!");
                throw new InvalidOperationException();
            }

            for (Node<T>? k = head; k != null; k = k.Next)
            {
                Act(k);
            }
        }
        public bool Has(T x) // Find out whether given element is in the list
        {
            bool ret = false;
            try
            {
                ForEach(nd =>
                    {
                        if (x.Equals(nd.data)) { ret = true; }
                    }
                );
            }
            catch (InvalidOperationException) { ret = false; }
            return ret;
        }
    }

    public class Program
    {
        // Abandoned implement
        //public static void Main()
        //{
        //    GenericList<int> intList = new GenericList<int>();

        //    intList.Print();
        //    GenericList<int>.IntListInit(intList, 10);
        //    intList.Print();

        //    Console.WriteLine("The minute element is " + intList.GetMin());
        //    Console.WriteLine("The maximum element is " + intList.GetMax());

        //    Console.WriteLine("The summation of all the elements is " + intList.GetSum()); 
        //    Console.WriteLine("The summation of all the elements is " + GenericList<int>.GetIntListSum(intList));
        //}
        public static void Main()
        {
            GenericList<int> intList = new GenericList<int>();
            ListPrint(intList);

            Console.WriteLine("Please input n");
            int n;
            try
            {
                n = int.Parse(Console.ReadLine());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + "\nInput is illegal!");
                return;
            }
            Console.WriteLine("Please input n integer elements, each in one line");
            int tmp;
            for (int i = 0; i < n; ++i)
            {
                try
                {
                    tmp = int.Parse(Console.ReadLine());
                    intList.Add(tmp);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message + "\nInput is illegal!");
                    return;
                }
            }

            ListPrint(intList);

            Console.WriteLine("The minute element is " + GetIntListMin(intList));
            Console.WriteLine("The maximum element is " + GetIntListMax(intList));

            Console.WriteLine("The summation of all the elements is " + GetIntListSum(intList));

            Console.WriteLine("Please input a element to check whether it is in the list");
            try
            {
                tmp = int.Parse(Console.ReadLine());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + "\nInput is illegal!");
                return;
            }
            Console.WriteLine($"The element {tmp} is {(intList.Has(tmp) ? "" : "not ")}in the list");
        }
        public static void ListPrint(GenericList<int> intList)
        {
            try
            {
                intList.ForEach(x => { }); // Test whether the list is empty
            }
            catch (InvalidOperationException)
            {
                return; //Empty list is acceptable
            }
            Console.WriteLine("-------\nThe elements in the list are as follows:");
            intList.ForEach(x => { Console.WriteLine(x.data); });
            Console.WriteLine("-------");
        }
        public static int GetIntListMin(GenericList<int> intList)
        {
            int mn = int.MaxValue;
            try
            {
                intList.ForEach(nd => { mn = (mn < nd.data) ? mn : nd.data; });
            }
            catch (InvalidOperationException e)
            {
                Console.WriteLine(e.Message);
                Environment.Exit(1); // Empty list here is unacceptable
            }
            return mn;
        }
        public static int GetIntListMax(GenericList<int> intList)
        {
            int mx = int.MinValue;
            try
            {
                intList.ForEach(nd => { mx = (mx > nd.data) ? mx : nd.data; });
            }
            catch (InvalidOperationException e)
            {
                Console.WriteLine(e.Message);
                Environment.Exit(1); // Empty list here is unacceptable
            }
            return mx;
        }
        public static int GetIntListSum(GenericList<int> intList)
        {
            int sum = 0;
            try
            {
                intList.ForEach(nd => sum += nd.data);
            }
            catch (InvalidOperationException) { } // Empty list here is acceptable
            return sum;
        }
        public static void IntListInit(GenericList<int> intList, int n)
        {
            for (int i = 0; i < n; ++i)
                intList.Add(i);
        }
        
    }
}
