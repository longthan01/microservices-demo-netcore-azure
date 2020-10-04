using System.Collections.Generic;

namespace Hansen.Kafka.Utility.Models
{
    public class Topic
    {
        public string Name { get; set; }
        public List<Partition> Partitions { get; set; } = new List<Partition>();
    }

    public class Partition
    {
        public int Id { get; set; }
    }
}