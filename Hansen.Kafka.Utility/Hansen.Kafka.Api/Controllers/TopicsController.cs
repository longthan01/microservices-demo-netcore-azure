﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using FizzWare.NBuilder;
using Hansen.Kafka.Api.Models;
using Hansen.Kafka.Utility.Configuration;
using Hansen.Kafka.Utility.Providers;

namespace Hansen.Kafka.Api.Controllers
{
   // [EnableCors("*", "*", "*")]
    public class TopicsController : ApiController
    {
        // GET api/topics
        public async Task<IHttpActionResult> Get([FromUri] TopicsQueryRequest request)
        {
            //ConnectionConfiguration config = new ConnectionConfiguration()
            //{
            //    BootstrapServer = "localhost:9092"
            //};
            //var dataProvider = new KafkaDataProvider(config, null);
            //var topics = await dataProvider.GetTopicsAsync();
            //Random r = new Random();
            //var response = new TopicsQueryResponse()
            //{
            //    Topics = topics.Select(x => new TopicDto()
            //    {
            //        Name = x.Name,
            //        QueueType = "SM Dummy queue type",
            //        MessageCount = r.Next(),
            //        NumOfPartitions = x.Partitions.Count()
            //    })
            //};
            var res = Builder<TopicsQueryResponse>.CreateNew().With(x => x.Topics = Builder<TopicDto>.CreateListOfSize(100).Build()).Build();
            return Ok(res);
        }

        public async Task<IHttpActionResult> Post([FromBody] TopicsCreateRequest data)
        {
            ConnectionConfiguration config = new ConnectionConfiguration()
            {
                BootstrapServers = "localhost:9092"
            };
            var dataProvider = new KafkaDataProvider(config, null);
            await dataProvider.AddNewTopicAsync(data.TopicName, 3, -1);
            return Ok();
        }
    }
}
