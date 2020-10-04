using System;
using Confluent.Kafka;
using System.Threading.Tasks;

namespace SM.Kafka.Producer
{
    public class Producer : IProducer
    {
        private readonly ProducerConfig _config;
        private readonly string _topicName;

        public Producer(ProducerConfig config, string topicName)
        {
            _config = config;
            _topicName = topicName;
        }
        public async Task<ProduceResult> Publish(MessageValue mv)
        {
            try
            {
                using (var p = new ProducerBuilder<Null, string>(_config).Build())
                {
                    Console.WriteLine($"Produce message {mv.Value}");
                    var dr = await p.ProduceAsync(this._topicName, new Message<Null, string>
                    {
                        Value = mv.Value
                    });
                    return new ProduceResult()
                    {
                        TopicPartitionOffset = dr.TopicPartitionOffset.Partition.Value,
                        TopicPartition = dr.TopicPartition.Partition.Value
                    };
                }
            }
            catch (System.Exception e)
            {
                throw;
            }
        }
    }

    public class MessageValue
    {
        public string Value { get; set; }

        public MessageValue(string value)
        {
            this.Value = value;
        }
    }
}
