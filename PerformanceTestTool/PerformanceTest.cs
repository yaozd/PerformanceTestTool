using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace PerformanceTestTool
{
    /// <summary>
    /// 性能测试
    /// </summary>
    public class PerformanceTest
    {
        /// <summary>
        /// 初始化
        /// </summary>
        public static void Initialize()
        {
            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High;
            Thread.CurrentThread.Priority = ThreadPriority.Highest;
        }
        /// <summary>
        /// 创建对象
        /// </summary>
        /// <param name="name">测试名称</param>
        /// <param name="threads">线程数</param>
        /// <param name="iteration">循环数</param>
        /// <param name="action">测试方法</param>
        public static void Time(string name, int threads, int iteration, Action action)
        {
            WorkerStat[] statArray = new WorkerStat[threads];
            Task[] taskArray=new Task[threads];
            WorkerStat mainStat = new WorkerStat {RunCount = threads*iteration};
            Stopwatch watch = Stopwatch.StartNew();
            for (int i = 0; i < threads; i++)
            {
                statArray[i]=new WorkerStat()
                {
                    RunCount = iteration
                };
                taskArray[i]= Task.Factory.StartNew(new TaskExt(statArray[i],action).Run);
            }
            Task.WaitAll(taskArray);
            watch.Stop();
            mainStat.Timespan = watch.ElapsedMilliseconds;
            WorkerStat totalStat = new WorkerStat();
            for (int i = 0; i < threads; i++)
            {
                totalStat.RunCount = totalStat.RunCount + statArray[i].RunCount;
                totalStat.Timespan = totalStat.Timespan + statArray[i].Timespan;
            }
            //---
            DisplayTestResult(name, threads, iteration, mainStat, totalStat);
        }
        /// <summary>
        /// 显示测试结果
        /// </summary>
        /// <param name="name"></param>
        /// <param name="threads"></param>
        /// <param name="iteration"></param>
        /// <param name="mainStat"></param>
        /// <param name="totalStat"></param>
        private static void DisplayTestResult(string name, int threads, int iteration, WorkerStat mainStat, WorkerStat totalStat)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Display(StringPadLeft("TestName:" + name) + StringPadLeft("threads:" + threads) +
                    StringPadLeft("iteration:" + iteration));
            Display("");
            //---
            Display("Test over!!");
            //---
            if (mainStat.Timespan == 0 || totalStat.Timespan == 0)
            {
                Display("mainStat.Timespan=0 or totalStat.Timespan = 0 测试数据太少！！");
                return;
            }
            //---
            Display("-------Test Result-------");
            Display(StringPadLeft("Total Thread") + StringPadLeft("Total Count") + StringPadLeft("Total Time(ms)") +
                    StringPadLeft("TPS"));
            Console.ForegroundColor = ConsoleColor.Red;
            Display(StringPadLeft(threads) + StringPadLeft(totalStat.RunCount) + StringPadLeft(totalStat.Timespan/threads) +
                    StringPadLeft(Math.Round(1000 * (totalStat.RunCount*threads/(double) totalStat.Timespan))));
            Console.ForegroundColor = ConsoleColor.White;
            Display("Detail Info：");
            Display("--------------Total WorkerStat");
            Display(StringPadLeft("Thread(" + threads + ")") + StringPadLeft("runs") + StringPadLeft("time(ms)"));
            Display(StringPadLeft("Total") + StringPadLeft(totalStat.RunCount) + StringPadLeft(totalStat.Timespan));
            Display(StringPadLeft("Avg") + StringPadLeft(iteration) + StringPadLeft(totalStat.Timespan/threads));
            Display(StringPadLeft("TPS") + StringPadLeft("--") + StringPadLeft(1000*(totalStat.RunCount/ (double)totalStat.Timespan)));
            Display("--------------Main WorkerStat");
            Display(StringPadLeft("Main") + StringPadLeft(mainStat.RunCount) + StringPadLeft(mainStat.Timespan));
            Display(StringPadLeft("TPS") + StringPadLeft("--") + StringPadLeft(1000*(mainStat.RunCount/ (double)mainStat.Timespan)));
            Display("--------------------------------END--------------------------------");
        }

        private static void Display(object obj)
        {
            Console.WriteLine(obj);
        }

        private static string StringPadLeft(object obj)
        {
            return obj.ToString().PadRight(20, ' ');
        }
    }
    /// <summary>
    /// 任务线程扩展
    /// </summary>
    public class TaskExt
    {
        private readonly WorkerStat _workerStat;
        private readonly Action _action;
        public TaskExt(WorkerStat workerStat, Action action)
        {
            _workerStat = workerStat;
            _action = action;
        }

        public void Run()
        {
            Stopwatch watch = Stopwatch.StartNew();
            for (int i = 0; i < _workerStat.RunCount; i++)
            {
                _action();
            }
            watch.Stop();
            _workerStat.Timespan = watch.ElapsedMilliseconds;
        }
    }
    /// <summary>
    /// 任务线程状态信息
    /// </summary>
    public class WorkerStat
    {
        public int RunCount;
        public long Timespan;

        public WorkerStat()
        {
            RunCount = 0;
            Timespan = 0;
        }
    }
}