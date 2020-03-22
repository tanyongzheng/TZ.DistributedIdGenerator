using System;
using System.Collections.Generic;
using System.Text;

namespace TZ.SnowflakeIdGenerator
{
    public class SnowflakeIdOptions
    {
        public long WorkerId { get; set; }

        public long DatacenterId { get; set; }
    }
}
