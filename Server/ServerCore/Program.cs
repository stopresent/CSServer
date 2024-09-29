using System;

namespace Server
{
    class Program
    {
        static void MainThread(object? state)
        {
            for (int i = 0; i < 5; i++)
                Console.WriteLine("Hello Thread");
        }

        static void Main(string[] args)
        {
            ThreadPool.SetMinThreads(1, 1);
            ThreadPool.SetMaxThreads(5, 5);

            for (int i = 0; i < 5; i++)
            {
                Task task = new Task(() => { while (true) ; }/*, TaskCreationOptions.LongRunning*/);
                task.Start();
            }

            //for (int i = 0; i < 4; i++)
            //{
            //    ThreadPool.QueueUserWorkItem((obj) =>
            //    {
            //        while (true)
            //        {

            //        }
            //    });
            //}

            ThreadPool.QueueUserWorkItem(MainThread);

            //for (int i = 0; i < 1000; ++i)
            //{
            //    Thread t = new Thread(MainThread);
            //    t.Name = "Test Thread";
            //    t.IsBackground = true;
            //    t.Start();

            //}

            //Console.WriteLine("Wait");

            //t.Join();
            //Console.WriteLine("Hello world");

            while (true)
            {

            }
        }
    }
}