using Hansen.Kafka.Utility.Annotations;
using Hansen.Kafka.Utility.Configuration;
using Hansen.Kafka.Utility.Models;
using Hansen.Kafka.Utility.Windows;
using log4net;
using log4net.Config;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Hansen.Kafka.Utility.Providers;

namespace Hansen.Kafka.Utility
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private const string DEFAULT_HOST = "localhost";
        private const string DEFAULT_PORT = "9092";

        public ObservableCollection<Topic> Topics { get; set; } = new ObservableCollection<Topic>();
        public ObservableCollection<Message> Messages { get; set; } = new ObservableCollection<Message>();
        private string _host;
        public string Host
        {
            get
            {

                if (_host == null) return DEFAULT_HOST;
                return _host;
            }
            set
            {
                _host = value;
                if (string.IsNullOrEmpty(_host))
                {
                    _host = DEFAULT_HOST;
                }
                OnPropertyChanged("Host");
            }
        }

        private string _port;

        public string Port
        {
            get { return string.IsNullOrEmpty(_port) ? DEFAULT_PORT : _port; }
            set
            {
                _port = value;
                if (string.IsNullOrEmpty(_port))
                {
                    _port = DEFAULT_PORT;
                }
                OnPropertyChanged("Port");
            }
        }

        private string _topicName;

        public string TopicName
        {
            get { return _topicName; }
            set
            {
                _topicName = value;
                OnPropertyChanged("TopicName");
            }
        }

        private System.Windows.Visibility _loadingVisibility = Visibility.Hidden;

        public System.Windows.Visibility LoadingVisibility
        {
            get { return _loadingVisibility; }
            set
            {
                _loadingVisibility = value;
                OnPropertyChanged("LoadingVisibility");
            }
        }

        private System.Windows.Media.SolidColorBrush _btnLoadColor;

        public System.Windows.Media.SolidColorBrush BtnLoadColor
        {
            get
            {
                return _btnLoadColor;
            }
            set
            {
                _btnLoadColor = value;
                OnPropertyChanged("BtnLoadColor");
            }
        }

        private bool _enableLoad;
        public bool EnableLoad
        {
            get => _enableLoad;
            set
            {
                _enableLoad = value;
                OnPropertyChanged("EnableLoad");
            }
        }

        private ObservableCollection<string> _logs = new ObservableCollection<string>();

        public ObservableCollection<string> Logs
        {
            get => _logs;
            set
            {
                _logs = value;
                OnPropertyChanged("Logs");
            }
        }

        private bool _isTopicsTabSelected = true;

        public bool IsTopicsTabSelected
        {
            get { return _isTopicsTabSelected; }
            set
            {
                _isTopicsTabSelected = value;
                OnPropertyChanged("IsTopicsTabSelected");
            }
        }
        private bool _isMessagesTabSelected;

        public bool IsMessagesTabSelected
        {
            get { return _isMessagesTabSelected; }
            set
            {
                _isMessagesTabSelected = value;
                OnPropertyChanged("IsMessagesTabSelected");
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private string _newTopicName;

        public string NewTopicName
        {
            get { return _newTopicName; }
            set
            {
                _newTopicName = value;
                OnPropertyChanged("NewTopicName");
            }
        }

        private ILog _logger = null;
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            XmlConfigurator.Configure();
            _logger = LogManager.GetLogger("KafkaConnectLog");
        }
        #region command bindings
        // command handler class
        public class CommandHandler : ICommand
        {
            private Action _action;
            private Func<bool> _canExecute;

            /// <summary>
            /// Creates instance of the command handler
            /// </summary>
            /// <param name="action">Action to be executed by the command</param>
            /// <param name="canExecute">A boolean property to containing current permissions to execute the command</param>
            public CommandHandler(Action action, Func<bool> canExecute)
            {
                _action = action;
                _canExecute = canExecute;
            }

            /// <summary>
            /// Wires CanExecuteChanged event 
            /// </summary>
            public event EventHandler CanExecuteChanged
            {
                add => CommandManager.RequerySuggested += value;
                remove => CommandManager.RequerySuggested -= value;
            }

            /// <summary>
            /// Forcess checking if execute is allowed
            /// </summary>
            /// <param name="parameter"></param>
            /// <returns></returns>
            public bool CanExecute(object parameter)
            {
                return _canExecute.Invoke();
            }

            public void Execute(object parameter)
            {
                _action();
            }
        }

        // command consumers
        private ICommand _contextMenuClickCommand;
        public ICommand ContextMenuClickCommand
        {
            get
            {
                return _contextMenuClickCommand ?? (_contextMenuClickCommand = new CommandHandler(ClearLogs, () => true));
            }
        }

        private ICommand _addTopicCommand;
        public ICommand AddTopicCommand
        {
            get
            {
                return _addTopicCommand ?? (_addTopicCommand = new CommandHandler(() => ShowAddNewTopicWindow(NewTopicName), () => true));
            }
        }

        #endregion

        #region helper methods

        public void ShowAddNewTopicWindow(string name)
        {
            if (!Validate())
            {
                return;
            }
            AddNewTopicWindow antw = new AddNewTopicWindow();
            antw.OnOk += async (sender, args) =>
            {
                await AddNewTopic(args.TopicName);
                antw.Close();
                await LoadTopics();
            };
            antw.ShowDialog();
        }

        public async Task AddNewTopic(string topicName)
        {
            try
            {
                ConnectionConfiguration config = new ConnectionConfiguration()
                {
                    BootstrapServer = $"{Host}:{Port}"
                };
                KafkaDataProvider dataProvider = new KafkaDataProvider(config, _logger);
                await dataProvider.AddNewTopicAsync(topicName, 1, -1);
            }
            catch (Exception e)
            {
                Logs.Add(e.Message);
            }
        }
        public void ClearLogs()
        {
            Logs.Clear();
        }

        private async Task LoadTopics()
        {
            Topics.Clear();
            var configuration = new ConnectionConfiguration()
            {
                BootstrapServer = $"{Host}:{Port}"
            };
            var dataProvider = new KafkaDataProvider(configuration, _logger);
            Task<IEnumerable<Topic>> task = null;
            try
            {
                task = Task.Run(() => dataProvider.GetTopicsAsync().Result);
                await task;
            }
            catch (Exception e)
            {
                Logs.Add(e.Message);
                return;
            }
           
            foreach (var topic in task.Result)
            {
                Topics.Add(topic);
            }
        }

        private async Task LoadMessages()
        {
            Messages.Clear();
            var configuration = new ConnectionConfiguration()
            {
                BootstrapServer = $"{Host}:{Port}"
            };
            var dataProvider = new KafkaDataProvider(configuration, _logger);
            Task<IEnumerable<Message>> task = null;
            try
            {
                task = Task.Run(() => dataProvider.GetMessagesAsync(TopicName));
                await task;
            }
            catch (Exception e)
            {
                Logs.Add(e.Message);
                return;
            }
           
            foreach (var message in task.Result)
            {
                Messages.Add(message);
            }
        }

        private bool Validate()
        {
            bool validServerInfoForTopicsRetrieval = !string.IsNullOrEmpty(Host) && !string.IsNullOrEmpty(Port);
            bool validServerInfoForMessagesRetrieval = validServerInfoForTopicsRetrieval
                                                      && (IsMessagesTabSelected && !string.IsNullOrEmpty(TopicName));
            bool toggleBtnLoad = (validServerInfoForTopicsRetrieval && IsTopicsTabSelected) || validServerInfoForMessagesRetrieval;
            return toggleBtnLoad;
        }

        #endregion

        #region event handlers
        private async void BtnLoadClick(object sender, EventArgs e)
        {
            if (!Validate())
            {
                return;
            }
            LoadingVisibility = Visibility.Visible;

            if (IsMessagesTabSelected)
            {
                await LoadMessages();
            }
            else if (IsTopicsTabSelected)
            {
                await LoadTopics();
            }
            LoadingVisibility = Visibility.Hidden;
        }

        private void MainWindow_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            Keyboard.ClearFocus();
        }

        private void DataGridRow_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataGridRow row = sender as DataGridRow;
            Topic topic = row.Item as Topic;
            if (topic == null)
            {
                return;
            }
            TopicName = topic.Name;
            IsMessagesTabSelected = true;
        }
        #endregion
    }
}
