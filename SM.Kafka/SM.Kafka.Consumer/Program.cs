using System;
using System.Threading;
using Confluent.Kafka;

namespace SM.Kafka.Consumer
{
    class Program
    {
        static void Main(string[] args)
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            Console.CancelKeyPress += (sender, eventArgs) =>
            {
                cts.Cancel();
            };
            IConsumer consumer = new Consumer(new ConsumerConfig()
            {
                CancellationTokenSource = cts,
                Topic = args[0],
                KafkaConsumerConfig = new Confluent.Kafka.ConsumerConfig()
                {
                    GroupId = Groups.SM_Group1,
                    BootstrapServers = "localhost:9092",
                    AutoOffsetReset = AutoOffsetReset.Earliest
                }
            });
            consumer.Consume();
        }
    }

    public class Groups
    {
        public const string SM_Group1 = "sm-group-1";
    }
}
