using System.Collections.Generic;

namespace Hansen.Kafka.Utility.Models
{
    public class Message
    {
        public string Data { get; set; }
        public string Topic { get; set; }
        public int PartitionOffset { get; set; }
    }
}