using System;
using System.Threading;

namespace EventTimer
{
    public class TimerCore // Publisher
    {
        private int tmBase;

        public TimerCore()
        {
            Tick = (x) => { };
            Alarm = () => { };
        }

        public delegate void TickHandler(int tm);
        public delegate void AlarmHandler();

        public event TickHandler Tick;
        public event AlarmHandler Alarm;

        public void Start(int bound) // Start the core with the timing upbound
        {
            int cnt = 1; // Count by second
            Thread.Sleep(1000);

            while (cnt <= bound)
            {
                Tick(cnt);

                if (cnt % 10 == 0)
                {
                    Alarm(); // Alarm per 10 seconds
                }

                cnt++;
                Thread.Sleep(1000);
            }
        }
    }
    class Timer //Subscriber
    {
        public TimerCore timer = new TimerCore(); 

        public Timer() // Subscribe the core's event
        {
            timer.Tick += PrintTick;
            timer.Alarm += PrintAlarm;
        }

        void PrintTick(int t)
        {
            Console.WriteLine(t + " seconds have passed");
        }
        void PrintAlarm()
        {
            Console.WriteLine("Alarm!");
        }
        public void Run(int bound)
        {
            timer.Start(bound); // Start the core
        }
    }
    class Program
    {
        public static void Main()
        {
            Timer myTimer = new Timer();
            myTimer.Run(30);
        }
    }
}