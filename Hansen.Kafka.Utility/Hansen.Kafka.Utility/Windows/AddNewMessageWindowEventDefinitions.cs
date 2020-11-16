using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hansen.Kafka.Utility.Windows.Events.Topics
{
    public class OnOkEventArgs : EventArgs
    {
        public string TopicName { get; }
        public int NumOfPartitions { get; set; }
        public short ReplicationFactor { get; set; }

        public OnOkEventArgs(string topicName, int numOfPartitions, short replicationFactor)
        {
            TopicName = topicName;
            this.NumOfPartitions = numOfPartitions;
            this.ReplicationFactor = replicationFactor;
        }
    }
    public delegate void OnOk(object sender, OnOkEventArgs e);
}
