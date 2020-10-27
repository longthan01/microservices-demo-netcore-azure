using System.Collections.Generic;

namespace Hansen.Kafka.Utility.Models
{
    public class Topic
    {
        public string Name { get; set; }
        public IEnumerable<Partition> Partitions { get; set; } = new List<Partition>();
    }

    public class Partition
    {
        public int Id { get; set; }
        /// <summary>
        /// Partitions might be split across brokers so there might be 2 different leaders for 2 partitions 
        /// </summary>
        public int PartitionLeader { get; set; }

        public IEnumerable<int> Replicas { get; set; } = new List<int>();
    }
}