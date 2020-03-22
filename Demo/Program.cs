using Microsoft.Extensions.Options;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using TZ.DistributedIdGenerator;
using TZ.RedisIdGenerator;

namespace Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            var options = new RedisIdConfigOptions();
            options.SetConfig();
            IDistributedIdService idService = new RedisIdService(options);
            var tableName = "User";
            idService.InitStartId(tableName, 0);
            for (var i = 0; i < 50; i++)
            {
                Task.Run(()=> {
                    GetIds(100);
                });
            }
            Console.ReadKey();
        }

        static void GetIds(int count)
        {
            var options = new RedisIdConfigOptions();
            options.SetConfig();
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
