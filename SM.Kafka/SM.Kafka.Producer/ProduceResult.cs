namespace SM.Kafka.Producer
{
    public class ProduceResult
    {
        public int TopicPartition { get; set; }
        public int TopicPartitionOffset { get; set; }
    }
}