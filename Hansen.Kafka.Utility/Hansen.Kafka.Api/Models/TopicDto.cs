using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hansen.Kafka.Api.Models
{
    public class TopicDto
    {
        public string Name { get; set; }
        public int NumOfPartitions { get; set; }
        public string QueueType { get; set; }
        public string LastRefreshed { get; set; }
        public bool IsSupportPurge { get; set; }
        public bool IsSupportRetry { get; set; }
        public int MessageCount { get; set; }
    }
}
