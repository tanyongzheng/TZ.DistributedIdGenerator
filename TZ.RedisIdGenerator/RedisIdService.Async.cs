using System;
using System.Threading;
using System.Threading.Tasks;
using Nito.AsyncEx;

namespace TZ.RedisIdGenerator
{
    public partial class RedisIdService
    {
        private static readonly SemaphoreSlim SyncSemaphore = new SemaphoreSlim(1, 1);
        
        public async Task<long> GetDistributedIdAsync(string tableName, CancellationToken token = default)
        {
            if (string.IsNullOrEmpty(tableName))
            {
                throw new ArgumentNullException($"please set param {nameof(tableName)}!");
            }
            //var redis = GetConnectionMultiplexer();
            using (await SyncSemaphore.LockAsync(token))
            {
                var idKey = idKeyPrefix + tableName;
                if (callCount == int.MaxValue)
                    callCount = 0;
                //按调用次数平均分配到每个Redis服务器
                int callRedisIndex = callCount % redisList.Count;
                var redis = redisList[callRedisIndex];
                callCount++;
                //调用Redis的INCRBY递增命令
                var id =await redis.GetDatabase().StringIncrementAsync(idKey, increment);
                return id;
            }
        }
    }
}