using System;
using System.Collections;

namespace CreateShapes
{
    public abstract class Shape // base class
    {
        public abstract double Area { get; }
        public abstract bool IsLegal();
    }
    public class Rectangle : Shape
    {
        protected double height;
        protected double width;

        public Rectangle(double height, double width)
        {
            this.height = height;
            this.width = width;
        }
        public Rectangle() : this(0, 0) { }

        public override double Area
        {
            get => height * width;
        }
        public override bool IsLegal()
        {
            return height >= 0 && width >= 0;
        }
    }
    public class Triangle : Shape
    {
        private double a;
        private double b;
        private double c;

        public Triangle(double a, double b, double c)
        {
            this.a = a;
            this.b = b;
            this.c = c;
        }
        public Triangle() : this(0, 0, 0) { }

        public override double Area
        {
            get
            {
                double p = (a + b + c) / 2;
                return Math.Sqrt(p * (p - a) * (p - b) * (p - c));
            }
        }
        public override bool IsLegal() // Difference here
        {
            return a >= 0 && b >= 0 && c >= 0
                   && (a + b > c) && (b + c > a) && (a + c > b);
        }
    }
    public class Square : Rectangle
    {
        private double length // length as variable is already inherited
        {
            get => width;
            set => width = (height = value);
        }

        public Square(double a)
        {
            length = a;
        }
        public Square() : this(0) { }

        //Area and Islegal() are directly inherited
    }
    public class ShapeFactory // A factory to build shapes
    {
        public static Shape RandShape() // Generate a random shape
        {
            Random rd = new Random();
            int k = rd.Next(0, 3);

            Shape x;

            switch (k)
            {
                case 0:
                    x = new Rectangle(rd.Next(0, 10), rd.Next(0, 10));
                    break;
                case 1:
                    do
                    { // guarantee triangle is legal
                        x = new Triangle(rd.Next(0, 10), rd.Next(0, 10), rd.Next(0, 10));
                    } while (!x.IsLegal());
                    break;
                case 2:
                    x = new Square(rd.Next(0, 10));
                    break;
                default: throw new ArgumentException();
            }

            return x;
        }
    }
    public class Program // Program with factory method
    {
        public static double SumRandArea(int n) // Accumulate areas
        {
            double sum = 0;
            for (int i = 0; i < n; ++i)
            {
                Shape x = ShapeFactory.RandShape(); // Call factory method
                sum += x.Area;
            }
            return sum;
        }
        public static void Main()
        {
            Console.WriteLine(SumRandArea(10));
        }
    }
}