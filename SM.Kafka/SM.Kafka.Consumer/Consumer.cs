using System;
using System.Threading;
using Confluent.Kafka;

namespace SM.Kafka.Consumer
{
    public class Consumer : IConsumer
    {
        private readonly ConsumerConfig _consumerConfig;
        private readonly string _topic;
        private readonly CancellationTokenSource _cancellationTokenSource;

        public Consumer(ConsumerConfig consumerConfig)
        {
            _consumerConfig = consumerConfig;
        }
        public void Consume()
        {
            using (var c = new ConsumerBuilder<Ignore, string>(this._consumerConfig.KafkaConsumerConfig).Build())
            {
                try
                {
                    while (true)
                    {
                        c.Subscribe(this._consumerConfig.Topic);
                        var cr = c.Consume(this._consumerConfig.CancellationTokenSource.Token);
                        Console.WriteLine($"Consumed {cr.Message.Value} at {cr.TopicPartitionOffset}");
                    }
                }
                catch (Exception e)
                {
                    c.Close();
                }
            }
        }
    }
}