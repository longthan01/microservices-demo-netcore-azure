using System.Threading;

namespace SM.Kafka.Consumer
{
    public class ConsumerConfig
    {
        public Confluent.Kafka.ConsumerConfig KafkaConsumerConfig { get; set; }
        public CancellationTokenSource CancellationTokenSource { get; set; }
        public string Topic { get; set; }
    }
}