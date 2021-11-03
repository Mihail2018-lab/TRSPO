using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

//namespace Lab2_TRSPO
//{
//    class Program
//    {
//        private static AutoResetEvent waitHandle = new AutoResetEvent(false);

//        static void Main(string[] args)
//        {

//            Stopwatch stopwatch = new Stopwatch();

//            // Begin timing.
//            //stopwatch.Start();


//            int[,] array;
//            Console.WriteLine("Columns:");
//            int columns = Convert.ToInt32(Console.ReadLine());
//            Console.WriteLine("Rows:");
//            int rows = Convert.ToInt32(Console.ReadLine());
//            array = new int[rows, columns];
//            Console.WriteLine("Number of thread:");
//            int number_thread = Convert.ToInt32(Console.ReadLine());
//            Random random_array = new Random();


//            StreamWriter sr = new StreamWriter("test1.txt");
//            //StreamWriter sr1 = new StreamWriter("result1.txt");
//            Thread[] Array_thread = new Thread[number_thread];


//            Console.WriteLine("Start array:");
//            for (int i = 0; i < rows; i++)
//            {
//                for (int j = 0; j < columns; j++)
//                {
//                    array[i, j] = random_array.Next(-100, 100);
//                    //Console.Write("{0}\t", array[i, j]);
//                    // запись в файл
//                    sr.Write(array[i, j].ToString() + " ");
//                }
//               sr.WriteLine();
//               //Console.WriteLine();
//            }
//            sr.Close();
//            StreamWriter sr1 = new StreamWriter("result1.txt");
//            Sort[] sortarray = new Sort[columns];
//            int y = 0;
//            //sortarray.columns = 0;
//            //sortarray.array = array;
//            //sortarray.rows = rows;

//            Console.WriteLine("Sort array:");
//            stopwatch.Start();


//            int f = 0;


//           Queue<Sort> queue = new Queue<Sort>();
//            for (int i = 0; i < columns; i++)
//            {
//                sortarray[i] = new Sort();
//                sortarray[i].columns = y;
//                sortarray[i].array = array;
//                sortarray[i].rows = rows;
//                sortarray[i].sr1 = sr1;
//                queue.Enqueue(sortarray[i]);
//                y++;
//            }



//            while (true)
//            {
//                if(f >= (number_thread-1))
//                {
//                    f = 0;
//                }

//                if (queue.Count != 0)
//                {
//                    //Array_thread[f] = new Thread(new ParameterizedThreadStart(sortingarray));
//                    //Array_thread[f].IsBackground = true;
//                    //Array_thread[f].Start(sortarray);
//                    Array_thread[f] = new Thread(sortingarray);
//                    Array_thread[f].IsBackground = true;
//                    Array_thread[f].Start(queue);
//                }
//                else
//                    break;

//                waitHandle.WaitOne();
//                //sortarray.columns += 1;
//                f+=1;
//                //Console.WriteLine("Все потоки завершились");
//            }
//            sr1.Close();


//            stopwatch.Stop();

//            // Write result.
//            Console.WriteLine("Time elapsed: {0}", stopwatch.Elapsed);

//            Console.ReadKey();
//        }



//        public static void sortingarray(object obj)
//        {
//            Queue<Sort> queue1 = (Queue<Sort>)obj;
//            Sort sort_array = (Sort)queue1.Dequeue();
//            //Sort sort_array = (Sort)obj;
//            int key = 0, f = 0;

//            for (int m = 1; m < sort_array.rows; m++)
//            {
//                key = sort_array.array[m, sort_array.columns];
//                f = m - 1;
//                while (f >= 0 && sort_array.array[f, sort_array.columns] > key)
//                {
//                    sort_array.array[f + 1, sort_array.columns] = sort_array.array[f, sort_array.columns];
//                    f--;
//                }
//                sort_array.array[f + 1, sort_array.columns] = key;
//            }
//            //Console.Write("Sort array:\n");
//            for (int j = 0; j < sort_array.rows; j++)
//            {
//                sort_array.sr1.Write(sort_array.array[j, sort_array.columns].ToString() + " ");
//                //Console.Write("{0}\t", sort_array.array[j, sort_array.columns]);
//            }
//            //Console.WriteLine();
//            sort_array.sr1.WriteLine();
//            waitHandle.Set();
//        }

//    }
//}




using System;
using System.Threading;
using System.Collections.Concurrent;
using System.Collections.Generic;


namespace Lab2_TRSPO
{
    class Program
    {
        private class TaskItem
        {
            public int index;
            public bool stop;
            public int[,] array;
            public int columns;
            public int rows;
            public StreamWriter sr1;


            public TaskItem(int index, bool stop, int[,] array, int rows, StreamWriter writer)
            {
                this.index = index;
                this.stop = stop;
                this.columns = index;
                this.array = array;
                this.rows = rows;
                this.sr1 = writer;
            }

            public TaskItem(int index, bool stop)
            {
                this.index = index;
                this.stop = stop;
            }



            public static TaskItem WorkItem(int index, int[,] array, int rows, StreamWriter writer)
            {
                return new TaskItem(index, false, array, rows, writer);
            }

            public static TaskItem StopItem()
            {
                return new TaskItem(0, true);
            }
        };

        private static void ThreadProc(object data)
        {
            ConcurrentQueue<TaskItem> queue = (ConcurrentQueue<TaskItem>)data;
            TaskItem task_item;
            while (true)
            {
                if (queue.TryDequeue(out task_item))
                {
                    if (task_item.stop)
                    {
                        break;
                    }
                    UserProc(task_item);
                }
                else
                {
                    Thread.Sleep(10);
                }
            }
        }

        private static void UserProc(TaskItem sort_array)
        {
            int key = 0, f = 0;
            for (int m = 1; m < sort_array.rows; m++)
            {
                key = sort_array.array[m, sort_array.columns];
                f = m - 1;
                while (f >= 0 && sort_array.array[f, sort_array.columns] > key)
                {
                    sort_array.array[f + 1, sort_array.columns] = sort_array.array[f, sort_array.columns];
                    f--;
                }
                sort_array.array[f + 1, sort_array.columns] = key;
            }
            //Console.Write("Sort array:\n");
            for (int j = 0; j < sort_array.rows; j++)
            {
                sort_array.sr1.WriteAsync(sort_array.array.ToString() + " ");
                //Console.Write("{0}\t", sort_array.array[j, sort_array.columns]);
            }
            //Console.WriteLine();
            sort_array.sr1.WriteLineAsync();
            Thread.Sleep(200);
            //waitHandle.Set();
        }

        public static void Main(string[] args)
        {

            int[,] array;
            Console.WriteLine("Columns:");
            int columns = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Rows:");
            int rows = Convert.ToInt32(Console.ReadLine());
            array = new int[rows, columns];
            Console.WriteLine("Number of thread:");
            int number_thread = Convert.ToInt32(Console.ReadLine());
            Random random_array = new Random();


            StreamWriter sr = new StreamWriter("test1.txt");
            //StreamWriter sr1 = new StreamWriter("result1.txt");


            Console.WriteLine("Start array:");
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    array[i, j] = random_array.Next(-100, 100);
                    //Console.Write("{0}\t", array[i, j]);
                    // запись в файл
                    sr.Write(array[i, j].ToString() + " ");
                }
                sr.WriteLine();
                //Console.WriteLine();
            }
            sr.Close();
            StreamWriter sr1 = new StreamWriter("result1.txt", true);

            ConcurrentQueue<TaskItem> queue = new ConcurrentQueue<TaskItem>();
            Thread[] threads = new Thread[number_thread];

            for (int i = 0; i < number_thread; i++)
            {
                threads[i] = new Thread(ThreadProc);
                threads[i].Start(queue);
            }



            for (int i = 0; i < columns; i++)
            {
                queue.Enqueue(TaskItem.WorkItem(i, array, rows, sr1));
            }

            for (int i = 0; i < number_thread; i++)
            {
                queue.Enqueue(TaskItem.StopItem());
            }
            Console.WriteLine("Join:");
            for (int i = 0; i < number_thread; i++)
            {
                threads[i].Join();
            }
            Console.WriteLine("Join.end:");
            sr1.Close();
            Console.ReadKey();
        }
    }
}

