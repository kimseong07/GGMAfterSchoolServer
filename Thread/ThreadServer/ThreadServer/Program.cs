using System;
using System.Threading;
using System.Threading.Tasks;

namespace ThreadServer
{
    class SpinLock
    {
        int _locked = 0;

        public void Acquire()
        {
            while (true)
            {
                int expected = 0;
                int desired = 1;
                if(Interlocked.CompareExchange(ref _locked, desired, expected) == expected)
                {
                    break;
                }
                Thread.Yield();
            }
        }

        public void Release()
        {
            _locked = 0;
        }
    }
    class Program
    {
        static int number;

        static SpinLock _lock = new SpinLock();

        static void Thread_1()
        {
            for (int i = 0; i < 100000; i++)
            {
                _lock.Acquire();
                number++;
                _lock.Release();
            }
        }

        static void Thread_2()
        {
            for (int i = 0; i < 100000; i++)
            {
                _lock.Acquire();
                number--;
                _lock.Release();
            }
        }

        static void Main(string[] args)
        {
            Task t1 = new Task(Thread_1);
            Task t2 = new Task(Thread_2);

            t1.Start();
            t2.Start();

            Task.WaitAll(t1, t2);
            Console.WriteLine($"Exit!! : {number}");


            Thread.Sleep(1); //무조건 휴식
            Thread.Sleep(0); //우선순위가 높은 쓰레드에게 양보
            Thread.Yield(); //실행가능한 쓰레드에게 양보
        }
    }
}
/*
        static void MainThread()
        {
            Console.WriteLine(Thread.CurrentThread.Name);
            while(true)
            {
                Console.WriteLine("Hello Thread");
            }
        }

        static void Main(string[] args)
        {
            Thread t = new Thread(MainThread);
            t.IsBackground = true;
            t.Name = "thread name";
            t.Start();

            Console.WriteLine("Hello World");

            t.Join();
        }
 */

/*
        static void MainThread(object state)
        {
            for (int i = 0; i < 5; i++)
            {
                Console.WriteLine("Hello Thread");
            }
        }

        static void Main(string[] args)
        {
            ThreadPool.SetMinThreads(1, 1);
            ThreadPool.SetMaxThreads(5, 5);

            for (int i = 0; i < 5; i++)
            {
                //Thread t = new Thread(MainThread);
                //t.IsBackground = true;
                //t.Start();

                //ThreadPool.QueueUserWorkItem(obj =>{ while (true){ } });
                Task t = new Task(() => { while (true) { } }, TaskCreationOptions.LongRunning);
                t.Start();
            }
            ThreadPool.QueueUserWorkItem(MainThread);

            while (true)
            {

            }
        }
 */

/* 
        static volatile bool _stop = false;

        static void MainThread()
        {
            Console.WriteLine("Start Thread....");

            if(_stop == false)
            {
                while (_stop == false)
                {

                }
            }

            while (_stop == false)
            {

            }
            Console.WriteLine("Stop Thread");
        }
        static void Main(string[] args)
        {
            Task t = new Task(MainThread);
            t.Start();

            Thread.Sleep(1000);
            _stop = true;

            Console.WriteLine("calling stop!");
            Console.WriteLine("Waiting for stop...");
            t.Wait();

            Console.WriteLine("Stop complete!");
        }
 */
/*
        static int number;

        static object _obj = new object();

        static void Thread_1()
        {
            for (int i = 0; i < 100000; i++)
            {
                lock(_obj)
                {
                    number++;
                }
            }
        }

        static void Thread_2()
        {
            for (int i = 0; i < 100000; i++)
            {
                lock(_obj)
                {
                    number--;
                }
            }
        }
 */
/*
    class SpinLock
    {
        int _locked = 0;

        public void Acquire()
        {
            while (true)
            {

                int original = Interlocked.CompareExchange(ref _locked, 1, 0);
                if(original == 0)
                {
                    break;
                }
            }
        }

        public void Release()
        {
            _locked = 0;
        }
    }

 static int number;

        static SpinLock _lock = new SpinLock();

        static void Thread_1()
        {
            for (int i = 0; i < 100000; i++)
            {
                _lock.Acquire();
                number++;
                _lock.Release();
            }
        }

        static void Thread_2()
        {
            for (int i = 0; i < 100000; i++)
            {
                _lock.Acquire();
                number--;
                _lock.Release();
            }
        }

        static void Main(string[] args)
        {
            Task t1 = new Task(Thread_1);
            Task t2 = new Task(Thread_2);

            t1.Start();
            t2.Start();

            Task.WaitAll(t1, t2);
            Console.WriteLine($"Exit!! : {number}");


            Thread.Sleep(1); //무조건 휴식
            Thread.Sleep(0); //우선순위가 높은 쓰레드에게 양보
            Thread.Yield(); //실행가능한 쓰레드에게 양보
        }
 */
/*
        static int number;

        static void Thread_1()
        {
            for(int i = 0; i < 100000; i++)
            {
                //number++;
                Console.WriteLine(number);
                int next = Interlocked.Increment(ref number);
                Console.WriteLine(next);
            }
        }

        static void Thread_2()
        {
            for (int i = 0; i < 100000; i++)
            {
                //number--;
                Interlocked.Decrement(ref number);
            }
        }

        static void Main(string[] args)
        {
            Task t1 = new Task(Thread_1);
            Task t2 = new Task(Thread_2);

            t1.Start();
            t2.Start();

            Task.WaitAll(t1, t2);            
            Console.WriteLine($"Exit!! : {number}");
        }
 */
