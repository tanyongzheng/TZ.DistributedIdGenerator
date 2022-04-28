using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TZ.DistributedIdGenerator;

namespace TZ.RedisIdGenerator
{
    public partial class RedisIdService : IDistributedIdService
    {
        private static int callCount = 0;
        private static List<ConnectionMultiplexer> redisList;
        private static readonly object lockObj = new object();

        /// <summary>
        /// idKey 前缀
        /// </summary>
        private static string idKeyPrefix;

        //递增长度
        private static int increment = 0;

        private readonly RedisIdOptions _redisIdOptions;

        /// <summary>
        /// 程序中最好使用单例模式
        /// 如要新建对象实例，请在程序最开始的地方先实例化一个对象
        /// </summary>
        /// <param name="options"></param>
        public RedisIdService(IOptions<RedisIdOptions> options)
        {
            if (options == null || options.Value == null)
            {
                throw new Exception("please set RedisIdOptions!");
            }
            else if (options.Value != null)
            {
                _redisIdOptions = options.Value;
            }
            if (_redisIdOptions == null)
            {
                throw new Exception("please set RedisIdOptions!");
            }
            if (_redisIdOptions.DefaultDatabase<0)
            {
                throw new Exception("please set RedisIdOptions->DefaultDatabase !");
            }
            if (_redisIdOptions.RedisConfigList==null||_redisIdOptions.RedisConfigList.Count==0)
            {
                throw new Exception("please set RedisIdOptions->RedisConfigList!");
            }
            var sameHost = _redisIdOptions.RedisConfigList.GroupBy(x => x.Host+":"+x.Port).Where(x=>x.Count()>1).ToList();
            if (sameHost.Any())
            {
                throw new Exception($"have same host [{sameHost.FirstOrDefault().Key}] RedisIdOptions->RedisConfigList!");
            }
            if (redisList == null||redisList.Count==0)
            {
                lock (lockObj)
                {
                    if (redisList == null || redisList.Count == 0)
                    {
                        //初始化redis列表
                        InitRedisList();
                    }
                }
            }
        }


        /// <summary>
        /// 初始化起始Id
        /// Redis Id算法使用
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="startId">起始Id</param>
        /// <returns></returns>
        public void InitStartId(string tableName, long startId)
        {
            lock (lockObj)
            {
                // 判断是否已初始化
                var hasInit = true;

                for (var i = 0; i < redisList.Count; i++)
                {
                    var redis = redisList[i];
                    var idKey = idKeyPrefix + tableName;
                    if (!redis.GetDatabase().KeyExists(idKey))
                    {
                        hasInit = false;
                        break;
                    }
                }

                //已初始化且开始Id为1
                if (hasInit && startId == 1)
                {
                    return;
                }

                //未初始化且开始Id为1
                if (!hasInit && startId == 1)
                {
                    for (var i = 0; i < redisList.Count; i++)
                    {
                        var initId = i - increment + 1;
                        var redis = redisList[i];
                        var idKey = idKeyPrefix + tableName;
                        redis.GetDatabase().StringSet(idKey, initId);
                    }
                    return;
                }

                startId = startId - increment + 1;
                foreach (var redis in redisList)
                {
                    var idKey = idKeyPrefix + tableName;
                    redis.GetDatabase().StringSet(idKey, startId);
                    startId++;
                }
            }
        }

        public long GetDistributedId(string tableName)
        {
            if (string.IsNullOrEmpty(tableName))
            {
                throw new ArgumentNullException($"please set param {nameof(tableName)}!");
            }
            //var redis = GetConnectionMultiplexer();
            lock (lockObj)
            {
                var idKey = idKeyPrefix + tableName;
                if (callCount == int.MaxValue)
                    callCount = 0;
                //按调用次数平均分配到每个Redis服务器
                int callRedisIndex = callCount % redisList.Count;
                var redis = redisList[callRedisIndex];
                callCount++;
                //调用Redis的INCRBY递增命令
                var id= redis.GetDatabase().StringIncrement(idKey, increment);
                return id;
            }
        }

        /*
        private ConnectionMultiplexer GetConnectionMultiplexer()
        {
            lock (lockObj)
            {
                if (callCount == int.MaxValue)
                    callCount = 0;
                //按调用次数平均分配到每个Redis服务器
                int callRedisIndex = callCount % redisList.Count;
                var redis = redisList[callRedisIndex];
                callCount++;
                return redis;
            }
        }*/

        /// <summary>
        /// 初始化Redis连接集合
        /// </summary>
        private void InitRedisList()
        {
            redisList = new List<ConnectionMultiplexer>();
            foreach(var redisConfig in _redisIdOptions.RedisConfigList)
            {
                ConfigurationOptions configurationOptions = new ConfigurationOptions();
                configurationOptions.AbortOnConnectFail = false;//超时不重试
                configurationOptions.EndPoints.Add(redisConfig.Host, redisConfig.Port);
                if (!string.IsNullOrEmpty(redisConfig.Password))
                    configurationOptions.Password = redisConfig.Password;
                configurationOptions.DefaultDatabase = _redisIdOptions.DefaultDatabase;
                //ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("server1:6379,server2:6379,abortConnect= false");
                ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(configurationOptions);
                redisList.Add(redis);
            }
            increment = redisList.Count;
            idKeyPrefix = _redisIdOptions.IdKeyPrefix;
        }

        public long CreateDistributedId()
        {
            throw new NotImplementedException();
        }
    }
}
