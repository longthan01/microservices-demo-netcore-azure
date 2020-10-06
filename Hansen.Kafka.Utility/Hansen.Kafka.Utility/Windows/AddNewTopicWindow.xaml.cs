using System.Windows;

namespace Hansen.Kafka.Utility.Windows
{
    /// <summary>
    /// Interaction logic for AddNewTopicWindow.xaml
    /// </summary>
    public partial class AddNewTopicWindow : Window
    {
        public string TopicName { get; set; }
        private string _numOfPartitions;

        public string NumOfPartitions
        {
            get => string.IsNullOrEmpty(_numOfPartitions) ? "3" : _numOfPartitions;
            set => _numOfPartitions = value;
        }
        private string _replicationFactor;

        public string ReplicationFactor
        {
            get => string.IsNullOrEmpty(_replicationFactor) ? "-1" : _replicationFactor;
            set => _replicationFactor = value;
        }

        public event OnOk OnOk;
        public AddNewTopicWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            OnOk?.Invoke(this, new OnOkEventArgs(TopicName, int.Parse(NumOfPartitions), short.Parse(ReplicationFactor)));
        }
    }
}
