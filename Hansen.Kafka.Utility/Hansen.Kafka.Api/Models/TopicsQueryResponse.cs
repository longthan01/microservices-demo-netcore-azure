using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Hansen.Kafka.Utility.Models;

namespace Hansen.Kafka.Api.Models
{
    public class TopicsQueryResponse
    {
        public IEnumerable<TopicDto> Topics { get; set; }
    }
}