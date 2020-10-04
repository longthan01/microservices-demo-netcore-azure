using Hansen.Kafka.Utility.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using Hansen.Kafka.Utility.Configuration;
using Hansen.Kafka.Utility.Providers;
using log4net;

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
            Assert.AreEqual(topics.First().Name, "sm-message");
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
    }
}
