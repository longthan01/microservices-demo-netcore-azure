using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hansen.Kafka.Utility.Windows
{
    public class OnOkEventArgs : EventArgs
    {
        public string TopicName { get; }

        public OnOkEventArgs(string topicName)
        {
            TopicName = topicName;
        }
    }
    public delegate void OnOk(object sender, OnOkEventArgs e);
}
