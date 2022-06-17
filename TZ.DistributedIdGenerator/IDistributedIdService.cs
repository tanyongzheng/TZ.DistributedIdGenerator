using System;
using System.Collections.Generic;
using System.Text;

namespace TZ.DistributedIdGenerator
{
    public interface IDistributedIdService
    {

        /// <summary>
        /// 获取分布式Id
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <returns></returns>
        long GetDistributedId(string tableName);

        /// <summary>
        /// 初始化起始Id
        /// Redis、Mongo Id等有序自增算法使用
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="startId">起始Id</param>
        /// <param name="skipInitialized">是否跳过已初始化（默认跳过，防止重启多次执行）</param>
        /// <returns></returns>
        void InitStartId(string tableName, long startId, bool skipInitialized = true);
    }
}
