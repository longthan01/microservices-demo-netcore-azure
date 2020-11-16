using Hansen.Kafka.Utility.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hansen.Kafka.Utility.Windows.Events.Messages
{
    public class OnOkEventArgs : EventArgs
    {
        public string TopicName { get; set; }
        public string Message { get; private set; }

        public OnOkEventArgs(string topicName, string message)
        {
            TopicName = topicName;
            Message = message;
        }
    }
    public delegate void OnOk(object sender, OnOkEventArgs e);
}
