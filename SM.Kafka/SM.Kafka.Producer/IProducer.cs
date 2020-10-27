using System.Threading.Tasks;

namespace SM.Kafka.Producer
{
    public interface IProducer
    {
        Task<ProduceResult> Publish(MessageValue mv);
    }
}