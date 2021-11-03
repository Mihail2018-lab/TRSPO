using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace Lab2_TRSPO
{
    class Program
    {
        static int[,] Array1;
        private class TaskItem
        {
            public int index;
            public bool stop;
            public int[,] array;
            public int columns;
            public int rows;


            public TaskItem(int index, bool stop, int[,] array, int rows)
            {
                this.index = index;
                this.stop = stop;
                this.columns = index;
                this.array = array;
                this.rows = rows;
            }

            public TaskItem(int index, bool stop)
            {
                this.index = index;
                this.stop = stop;
            }



            public static TaskItem WorkItem(int index, int[,] array, int rows)
            {
                return new TaskItem(index, false, array, rows);
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

            Array1 = new int[rows, columns];
            StreamWriter sr = new StreamWriter("test1.txt");


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
            StreamWriter sw = new StreamWriter("result1.txt");


            //        Console.WriteLine ("Hello Mono World");
            ConcurrentQueue<TaskItem> queue = new ConcurrentQueue<TaskItem>();
            Thread[] threads = new Thread[number_thread];

            for (int i = 0; i < number_thread; i++)
            {
                threads[i] = new Thread(ThreadProc);
                threads[i].Start(queue);
            }



            for (int i = 0; i < columns; i++)
            {
                queue.Enqueue(TaskItem.WorkItem(i, array, rows));
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
            for (int i = 0; i < rows; i++)
            {
                //Сonsole.Writeline();
                for (int j = 0; j < columns; j++)
                {
                    //Console.Write("{0}\t", array[i, j]);
                    sw.Write(array[i, j].ToString() + "\t");
                }
                sw.WriteLine();
            }
            sw.Close();

            Console.ReadKey();
        }
    }
}
