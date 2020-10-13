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
            string name = args.Length > 0 ? args[0] : "Default";
            IProducer pr = new Producer(new ProducerConfig()
            {
                BootstrapServers = "localhost:9092"
            }, args[1]);
            string msgPrefix = args[2];
            CancellationTokenSource cts = new CancellationTokenSource();
            Console.CancelKeyPress += (s, e) =>
            {
                cts.Cancel();
            };
            Task loopTask = new Task(() =>
            {
                while (true)
                {
                    var task = pr.Publish(new MessageValue($"Producer {name}, {msgPrefix} - {DateTime.Now.Second}"));
                    task.Wait();
                    Console.WriteLine($"Result {task.Result.TopicPartition} - {task.Result.TopicPartitionOffset}");
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
