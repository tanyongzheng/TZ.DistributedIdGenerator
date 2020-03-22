using Microsoft.Extensions.Options;
using System;
using TZ.DistributedIdGenerator;
using TZ.SnowflakeIdGenerator.SnowflakeId;

namespace TZ.SnowflakeIdGenerator
{
    public class SnowflakeIdService:IDistributedIdService
    {
        private static IdWorker worker = null;
        private static readonly object lockObj = new object();

        private readonly SnowflakeIdOptions _snowflakeIdOptions;

        public SnowflakeIdService(IOptions<SnowflakeIdOptions> options)
        {
            if (options == null || options.Value == null)
            {
                throw new Exception("please set SnowflakeIdOptions!");
            }
            else if (options.Value != null)
            {
                _snowflakeIdOptions = options.Value;
            }
        }

        private long CreateDistributedId()
        {
            if (worker == null)
            {
                lock (lockObj)
                {
                    // 如果类的实例不存在则创建
                    if (worker == null)
                    {
                        worker = new IdWorker(_snowflakeIdOptions.WorkerId, _snowflakeIdOptions.DatacenterId);//
                    }
                }
            }

            lock (lockObj)
            {
                return worker.NextId();
            }
        }

        public long GetDistributedId(string tableName)
        {
            return CreateDistributedId();
        }

        public void InitStartId(string tableName, long startId)
        {
            throw new NotImplementedException();
        }
    }
}
