using Microsoft.Extensions.Options;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using TZ.DistributedIdGenerator;
using TZ.RedisIdGenerator;

namespace Demo
{
    class Program
    {
        //private static IDistributedIdService idService;

        private static RedisIdConfigOptions  options = new RedisIdConfigOptions();
        static void Main(string[] args)
        {
            
            int minWorker, minIOC;
            // Get the current settings.
            ThreadPool.GetMinThreads(out minWorker, out minIOC);
            // Change the minimum number of worker threads to four, but
            // keep the old setting for minimum asynchronous I/O 
            // completion threads.
            if (ThreadPool.SetMinThreads(200, 200))
            {

            }
        
            //var options = new RedisIdConfigOptions();
            options.SetConfig();
            IDistributedIdService idService = new RedisIdService(options);
            idService = new RedisIdService(options);
            var tableName = "User";
            idService.InitStartId(tableName, 0);
            for (var i = 0; i < 150; i++)
            {
                Task.Run(()=> {
                    GetIds(100);
                });
            }
            Console.ReadKey();
        }

        static void GetIds(int count)
        {
            //var options = new RedisIdConfigOptions();
            //options.SetConfig();
            IDistributedIdService idService = new RedisIdService(options);
            var tableName = "User";
            Stopwatch watch = new Stopwatch();
            watch.Start();
            for (var i = 0; i < count; i++)
            {
                var id = idService.GetDistributedId(tableName);
                //Console.WriteLine($"第{(i + 1)}个Id:{id}");
                Console.WriteLine(id);
            }
            watch.Stop();
            Console.WriteLine($"总用时{watch.ElapsedMilliseconds}毫秒");
        }
    }
}
