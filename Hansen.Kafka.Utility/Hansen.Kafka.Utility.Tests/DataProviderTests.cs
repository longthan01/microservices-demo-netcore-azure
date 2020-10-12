using Hansen.Kafka.Utility.Configuration;
using Hansen.Kafka.Utility.Providers;
using log4net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace Hansen.Kafka.Utility.Tests
{
    [TestClass]
    public class DataProviderTests
    {
        [TestMethod]
        public void GetListTopics_ReturnList()
        {
            ConnectionConfiguration config = new ConnectionConfiguration()
            {
                BootstrapServer = "localhost:9092"
            };

            KafkaDataProvider dataProvider = new KafkaDataProvider(config, LogManager.GetLogger(typeof(DataProviderTests)));
            var topics = dataProvider.GetTopicsAsync().Result.ToList();
            Assert.IsTrue(topics.Count > 0);
        }
        [TestMethod]
        public void GetListMessagePerTopics_ReturnList()
        {
            ConnectionConfiguration config = new ConnectionConfiguration()
            {
                BootstrapServer = "localhost:9092"
            };
            KafkaDataProvider dataProvider = new KafkaDataProvider(config, LogManager.GetLogger(typeof(DataProviderTests)));
            var messages = dataProvider.GetMessagesAsync("sm-message").Result.ToList();
            Assert.IsTrue(messages.Count > 0);
        }
        [TestMethod]
        public void CreateNewTopicWith1Partition_ReturnSuccess()
        {
            ConnectionConfiguration config = new ConnectionConfiguration()
            {
                BootstrapServer = "localhost:9092"
            };
            KafkaDataProvider dataProvider = new KafkaDataProvider(config, LogManager.GetLogger(typeof(DataProviderTests)));
            string newTopic = $"UnitTest-{Guid.NewGuid().ToString()}-{DateTime.Now.Ticks}";
            var task = dataProvider.AddNewTopicAsync(newTopic, 1, -1);
            task.Wait();
            // get all topics
            var topics = dataProvider.GetTopicsAsync().Result.ToList();
            Assert.IsTrue(topics.Count > 0);
            var nt = topics.FirstOrDefault(x => x.Name == newTopic);
            Assert.IsTrue(nt != null);
            Assert.IsTrue(nt.Partitions.First().Id == 0);
            // create topic with 2 partition
        }
        [TestMethod]
        public void CreateNewTopicWith3Partition_ReturnSuccess()
        {
            ConnectionConfiguration config = new ConnectionConfiguration()
            {
                BootstrapServer = "localhost:9092"
            };
            KafkaDataProvider dataProvider = new KafkaDataProvider(config, LogManager.GetLogger(typeof(DataProviderTests)));
            string newTopic = $"UnitTest-{Guid.NewGuid().ToString()}-{DateTime.Now.Ticks}";
            var task = dataProvider.AddNewTopicAsync(newTopic, 3, -1);
            task.Wait();
            // get all topics
            var topics = dataProvider.GetTopicsAsync().Result.ToList();
            Assert.IsTrue(topics.Count > 0);
            var nt = topics.FirstOrDefault(x => x.Name == newTopic);
            Assert.IsTrue(nt != null);
            Assert.IsTrue(nt.Partitions.Count() == 3);
            // create topic with 2 partition
        }
    }
}
