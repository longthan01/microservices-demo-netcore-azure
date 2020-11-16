using Hansen.Kafka.Utility.Windows.Events.Messages;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Hansen.Kafka.Utility.Windows
{
    /// <summary>
    /// Interaction logic for AddNewMessageWindow.xaml
    /// </summary>
    public partial class AddNewMessageWindow : Window, INotifyPropertyChanged
    {
        private string _topicName;

        public string TopicName
        {
            get { return _topicName; }
            set { _topicName = value; OnPropertyChanged("TopicName"); }
        }

        public string Message { get; set; }

        public event OnOk OnOk;
        public event PropertyChangedEventHandler PropertyChanged;
        [Annotations.NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public AddNewMessageWindow(string topicName)
        {
            InitializeComponent();
            DataContext = this;
            this.TopicName = topicName;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OnOk?.Invoke(this, new OnOkEventArgs(TopicName, Message));
        }
    }
}
