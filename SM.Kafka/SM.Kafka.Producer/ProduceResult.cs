namespace SM.Kafka.Producer
{
    public class ProduceResult
    {
        public long TopicPartition { get; set; }
        public long TopicPartitionOffset { get; set; }
    }
}