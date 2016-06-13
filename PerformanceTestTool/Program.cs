using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PerformanceTestTool
{
    class Program
    {
        //原子计数
        public static int Count;
        //客户端连接池的数量
        public static int ClientCount = 10;
        //使用WebClient作为一个示例
        public static WebClient[] WebClients;
        static void Main(string[] args)
        {
            ClientInit();
            PerformanceTest.Initialize();
            PerformanceTest.Time("name-1", 5, 1000, (Run));
            PerformanceTest.Time("name-1",2,100,(() => { int a = 1; Thread.Sleep(5); }));
            PerformanceTest.Time("name-2", 2, 100, (() => { int a = 1; Thread.Sleep(5); }));
            PerformanceTest.Time("name-3", 2, 100, (() => { int a = 1; Thread.Sleep(5); }));
            Console.Read();
        }

        static void Run()
        {
            var current = Interlocked.Increment(ref Count);
            //用取余的方试，平均分布到各个客户端
            var clientNum = current%ClientCount;
            //
            var webClient = WebClients[clientNum];
            webClient.DownloadString("http://localhost:1337/");
            //
            if (current%ClientCount == 0)
            {
                //
            }
        }
        /// <summary>
        /// 初始化客户端
        /// </summary>
        static void ClientInit()
        {
            WebClients=new WebClient[ClientCount];
            for (int i = 0; i < ClientCount; i++)
            {
                WebClients[i]=new WebClient();
            }
        }
    }
}
