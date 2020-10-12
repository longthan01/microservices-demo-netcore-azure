using Confluent.Kafka;
using Confluent.Kafka.Admin;
using Hansen.Kafka.Utility.Configuration;
using Hansen.Kafka.Utility.Models;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Partition = Hansen.Kafka.Utility.Models.Partition;

namespace Hansen.Kafka.Utility.Providers
{
    public class KafkaDataProvider
    {
        private readonly ConnectionConfiguration _connectionConfiguration;
        private readonly ILog _logger;
        private readonly string _topicName;

        public KafkaDataProvider(ConnectionConfiguration connectionConfiguration, ILog logger)
        {
            _connectionConfiguration = connectionConfiguration;
            _logger = logger;
        }
        public Task<IEnumerable<Topic>> GetTopicsAsync()
        {
            List<Topic> result = new List<Topic>();
            try
            {
                using (var adminClient = new AdminClientBuilder(_connectionConfiguration.ToPairs()).Build())
                {
                    var metadata = adminClient.GetMetadata(TimeSpan.FromSeconds(5));
                    foreach (var metadataTopic in metadata.Topics)
                    {
                        result.Add(new Topic()
                        {
                            Name = metadataTopic.Topic,
                            Partitions = metadataTopic.Partitions.Select(x => new Partition
                            {
                                Id = x.PartitionId,
                                PartitionLeader = x.Leader,
                                Replicas = x.Replicas.Select(y => y).ToList()
                            }).ToList(),
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                _logger?.Error(ex);
                throw;
            }

            return Task.FromResult((IEnumerable<Topic>)result);
        }
        private Metadata GetMetadataPerTopic(string topicName)
        {
            using (var adminClient = new AdminClientBuilder(_connectionConfiguration.ToPairs()).Build())
            {
                var metadata = adminClient.GetMetadata(topicName, TimeSpan.FromSeconds(5));
                return metadata;
            }
        }
        public Task<IEnumerable<Message>> GetMessagesAsync(string topic)
        {
            return this.GetMessagesAsync(topic, AutoOffsetReset.Earliest);
        }
        public Task<IEnumerable<Message>> GetStreamMessagesAsync(string topic)
        {
            return this.GetMessagesAsync(topic, AutoOffsetReset.Latest);
        }
        private Task<IEnumerable<Message>> GetMessagesAsync(string topic, AutoOffsetReset offsetResetSetting)
        {
            List<Message> result = new List<Message>();
            CancellationTokenSource cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            try
            {
                ConsumerConfig consumerConfig = new ConsumerConfig()
                {
                    BootstrapServers = _connectionConfiguration.BootstrapServer,
                    GroupId = Guid.NewGuid().ToString() + DateTime.Now.Ticks,
                    AutoOffsetReset = offsetResetSetting,
                    EnableAutoCommit = false
                };
                using (var consumer = new ConsumerBuilder<Null, string>(consumerConfig).Build())
                {
                    consumer.Subscribe(topic);
                    while (!cts.IsCancellationRequested)
                    {
                        var cr = consumer.Consume(cts.Token);
                        result.Add(new Message()
                        {
                            Data = cr.Message.Value,
                            Topic = cr.Topic,
                            PartitionOffset = cr.Partition.Value
                        });
                    }
                }
            }
            catch (OperationCanceledException oce)
            {
                // cancellation token canceled mean that we have no more message coming in, return
                _logger?.Error($"Operation canceled, probably due to time out which mean there's no message coming in", oce);
            }
            catch (Exception ex)
            {
                _logger?.Error(ex);
                throw;
            }
            return Task.FromResult((IEnumerable<Message>)result);
        }
        public async Task AddNewTopicAsync(string topicName, int numOfPartition, short replicationFactor)
        {
            using (var adminClient = new AdminClientBuilder(new AdminClientConfig { BootstrapServers = _connectionConfiguration.BootstrapServer }).Build())
            {
                try
                {
                    await adminClient.CreateTopicsAsync(new TopicSpecification[] {
                        new TopicSpecification { Name = topicName, ReplicationFactor = replicationFactor, NumPartitions = numOfPartition} });
                }
                catch (CreateTopicsException e)
                {
                    _logger.Error($"Topic creation failed with error: {e}");
                    throw;
                }
            }
        }
    }
}
