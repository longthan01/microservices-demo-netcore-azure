using Confluent.Kafka;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Kafka.Producer
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 4)
            {
                Console.WriteLine($"Sm.Kafka.producer server-info topic-name message");
            }
            string serverInfo = args[0];
            string topicName = args[1];
            string msg = args[2];
            
            IProducer pr = new Producer(new ProducerConfig()
            {
                BootstrapServers = serverInfo
            }, topicName);
            CancellationTokenSource cts = new CancellationTokenSource();
            Console.CancelKeyPress += (s, e) =>
            {
                cts.Cancel();
            };
            Task loopTask = new Task(() =>
            {
                while (true)
                {
                    var task = pr.Publish(new MessageValue($"{msg}"));
                    task.Wait();
                    //Console.WriteLine($"Result {task.Result.TopicPartition} - {task.Result.TopicPartitionOffset}");
                    Thread.Sleep(500);
                    if (cts.IsCancellationRequested)
                    {
                        break;
                    }
                }
            }, cts.Token);
            loopTask.Start();
            loopTask.Wait();
        }
    }
}
