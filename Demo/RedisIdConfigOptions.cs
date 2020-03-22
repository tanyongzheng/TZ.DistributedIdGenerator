using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using TZ.RedisIdGenerator;

namespace Demo
{
    public class RedisIdConfigOptions : IOptions<RedisIdOptions>
    {
		private RedisIdOptions redisIdOptions;
		public RedisIdOptions Value
		{
			get
			{
				return redisIdOptions;
			}
		}

		public void SetConfig()
		{
			redisIdOptions = new RedisIdOptions();
			redisIdOptions.DefaultDatabase = 10;
			redisIdOptions.IdKeyPrefix = "DistributedId_";
			redisIdOptions.RedisConfigList = new List<RedisConfig>();
			var redisConfig1 = new RedisConfig();
			redisConfig1.Host = "127.0.0.1";
			redisConfig1.Port = 6379;
			redisIdOptions.RedisConfigList.Add(redisConfig1);
			/*
			var redisConfig2 = new RedisConfig();
			redisConfig2.Host = "192.168.1.123";
			redisConfig2.Port = 6379;
			redisConfig2.Password = "123456";
			redisIdOptions.RedisConfigList.Add(redisConfig2);
			*/
			var redisConfig3 = new RedisConfig();
			redisConfig3.Host = "127.0.0.1";
			redisConfig3.Port = 6378;
			redisIdOptions.RedisConfigList.Add(redisConfig3);
		}
	}
}
