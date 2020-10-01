using System.Threading;

namespace SM.Kafka.Consumer
{
    public interface IConsumer
    {
        void Consume();
    }
}