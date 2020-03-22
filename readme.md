# TZ.DistributedIdGenerator

TZ分布式Id生成器

#### 介绍
用于分布式系统的Id生成，目前已实现SnowflakeId、RedisId。


#### Redis Id使用说明

1. Install-Package TZ.RedisIdGenerator
2. 注入服务：
```
    services.AddRedisId();
```
3. 配置Redis
```
    "RedisIdOptions": {
    "DefaultDatabase": 10,
    "IdKeyPrefix": "DistributedId_",
    "RedisConfigList": [
      {
        "Host": "127.0.0.1",
        "Port": 6379,
        "Password": ""
      },
      {
        "Host": "127.0.0.1",
        "Port": 6378,
        "Password": ""
      }
    ]
  }
```
4. 使用见项目Demo
