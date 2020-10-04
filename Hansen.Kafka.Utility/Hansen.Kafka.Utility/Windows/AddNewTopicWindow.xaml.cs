using System;
using System.Collections.Generic;
using System.Linq;
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
    /// Interaction logic for AddNewTopicWindow.xaml
    /// </summary>
    public partial class AddNewTopicWindow : Window
    {
        public string TopicName { get; set; }
        public event OnOk OnOk;
        public AddNewTopicWindow()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            this.OnOk?.Invoke(this, new OnOkEventArgs(this.TopicName));
        }
    }
}
