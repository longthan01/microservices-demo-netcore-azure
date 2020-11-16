using Hansen.Kafka.Utility.Annotations;
using Hansen.Kafka.Utility.Configuration;
using Hansen.Kafka.Utility.Models;
using Hansen.Kafka.Utility.Providers;
using Hansen.Kafka.Utility.Windows;
using Hansen.Kafka.Utility.Windows.Events;
using log4net;
using log4net.Config;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Timer = System.Timers.Timer;

namespace Hansen.Kafka.Utility
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        #region properties - variables
        private readonly string DEFAULT_HOST = ConfigurationManager.AppSettings["DEFAULT_HOST"];
        private readonly string DEFAULT_PORT = ConfigurationManager.AppSettings["DEFAULT_PORT"];
        private readonly string DEFAULT_SERVERS = ConfigurationManager.AppSettings["DEFAULT_SERVERS"];

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

        private string _servers;

        public string Servers
        {
            get { return string.IsNullOrEmpty(_servers) ? DEFAULT_SERVERS : _servers; }
            set
            {
                _servers = value;
                if (string.IsNullOrEmpty(_servers))
                {
                    _servers = DEFAULT_SERVERS;
                }
                OnPropertyChanged("Servers");
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

        private CancellationTokenSource streamCancellationTokenSource = null;
        private System.Windows.Visibility _streamingMode = Visibility.Hidden;

        public System.Windows.Visibility StreamingMode
        {
            get { return _streamingMode; }
            set
            {
                _streamingMode = value; 
                this.OnPropertyChanged("StreamingMode");
            }
        }

        #endregion
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
        private ICommand _addMessageCommand;
        public ICommand AddMessageCommand
        {
            get
            {
                return _addMessageCommand ?? (_addMessageCommand = new CommandHandler(() => ShowAddNewMessageWindow(TopicName), () => true));
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
                await AddNewTopic(args.TopicName, args.NumOfPartitions, args.ReplicationFactor);
                antw.Close();
                await LoadTopics();
            };
            antw.ShowDialog();
        }
        private string GetBootstrapServers()
        {
            return this.Servers;
        }
        public async Task AddNewTopic(string topicName, int numOfPartitions, short replicationFactor)
        {
            LoadingVisibility = Visibility.Visible;
            try
            {
                ConnectionConfiguration config = new ConnectionConfiguration()
                {
                    BootstrapServers = GetBootstrapServers()
                };
                KafkaDataProvider dataProvider = new KafkaDataProvider(config, _logger);
                await dataProvider.AddNewTopicAsync(topicName, numOfPartitions, replicationFactor);
            }
            catch (Exception e)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Logs.Add(e.Message);
                });
            }
            LoadingVisibility = Visibility.Hidden;
        }
        public void ShowAddNewMessageWindow(string topicName)
        {
            if (!Validate())
            {
                return;
            }
            AddNewMessageWindow window = new AddNewMessageWindow(topicName);
            window.OnOk += async (sender, args) =>
            {
                LoadingVisibility = Visibility.Visible;
                string message = PreProcessMessage(args.Message);
                await AddNewMessageAsync(args.TopicName, message);
                window.Close();
                await LoadMessages();
                LoadingVisibility = Visibility.Hidden;
            };
            window.ShowDialog();
        }
        private bool IsJson(string strInput)
        {
            if (string.IsNullOrWhiteSpace(strInput)) { return false; }
            strInput = strInput.Trim();
            if ((strInput.StartsWith("{") && strInput.EndsWith("}")) || //For object
                (strInput.StartsWith("[") && strInput.EndsWith("]"))) //For array
            {
                try
                {
                    var obj = JToken.Parse(strInput);
                    return true;
                }
                catch (JsonReaderException jex)
                {
                    //Exception in parsing json
                    return false;
                }
                catch (Exception ex) //some other exception
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        private string PreProcessMessage(string msg)
        {
            if(IsJson(msg))
            {
                return msg;
            }
            else
            {
                return  JsonConvert.SerializeObject(new
                {
                    message = msg
                });
            }
        }
        public async Task AddNewMessageAsync(string topicName, string message)
        {
            try
            {
                ConnectionConfiguration config = new ConnectionConfiguration()
                {
                    BootstrapServers = GetBootstrapServers()
                };
                KafkaDataProvider dataProvider = new KafkaDataProvider(config, _logger);
                await dataProvider.AddNewMessageAsync(topicName, message);
            }
            catch (Exception e)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Logs.Add(e.Message);
                });
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
                BootstrapServers = GetBootstrapServers()
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
                BootstrapServers = GetBootstrapServers()
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

        private void StreamMessages()
        {
            streamCancellationTokenSource = new CancellationTokenSource();
            var configuration = new ConnectionConfiguration()
            {
                BootstrapServers = GetBootstrapServers()
            };
            var dataProvider = new KafkaDataProvider(configuration, _logger);
            Timer timer = new Timer { Interval = 1000 };
            timer.Elapsed += async (o, args) =>
            {
                try
                {
                    var messages = await dataProvider.GetStreamMessagesAsync(TopicName);
                    foreach (var msg in messages)
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            Messages.Add(msg);
                        });
                    }
                }
                catch (Exception ex)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        Logs.Add(ex.Message);
                    });
                }

                if (streamCancellationTokenSource.IsCancellationRequested)
                {
                    timer.Stop();
                }
            };
            timer.Start();
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

        private void BtnStopStreamClick(object sender, RoutedEventArgs e)
        {
            streamCancellationTokenSource.Cancel();
            this.StreamingMode = Visibility.Hidden;
        }

        private void BtnStreamClick(object sender, RoutedEventArgs e)
        {
            if (!Validate())
            {
                return;
            }

            this.StreamingMode = Visibility.Visible;
            StreamMessages();
        }
        private void BtnAddMessageClick(object sender, RoutedEventArgs e)
        {

        }
        #endregion


    }
}
