
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Threading;
using System.Windows.Data;

namespace POS_Editor {

    public partial class MainWindow : INotifyPropertyChanged {

        private const int Profiles = 5;

        private readonly ObservableCollection<string> _comPorts;
        private string _comName;

        public string ComName {
            get {
                return _comName;
            }
            set {
                _comName = value;
                OnPropertyChanged("ComName");
                OnPropertyChanged("IsReady");
            }
        }

        public List<CustomMessageBox> ProfileList {
            get;
            set;
        }

        public bool IsReady {
            get {
                return PosManager.DoesDeviceExist(ComName);
            }
        }

        public ObservableCollection<string> ComPorts {
            get {
                return _comPorts;
            }
        }

        public MainWindow() {

            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += CurrentDomainOnUnhandledException;

            DataContext = this;
            _comPorts = new ObservableCollection<string>();

            InitializeComponent();
            ComBox.ItemsSource = _comPorts;

            Main();

            StartThreads();
        }

        private void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs unhandledExceptionEventArgs) {

            Exception e = (Exception) unhandledExceptionEventArgs.ExceptionObject;

            string message = "";

            message += "\nCrashed on: " + e.Message;

            File.AppendAllText("com.silvenga.pos-editor.crashreport.log", message);

        }

        public void Main() {

            Save save = Save.OpenObject();

            ProfileList = new List<CustomMessageBox>();

            for(int i = 0; i < Profiles; i++) {

                ProfileList.Add(new CustomMessageBox());
                ProfileList[i].Sent += CustomMessageBox_OnSent;

                Binding binding = new Binding("IsReady") {
                    Source = this
                };
                BindingOperations.SetBinding(ProfileList[i], IsEnabledProperty, binding);

                ProfilePanel.Children.Add(ProfileList[i]);

                if(save != null && save.Profiles != null && save.Profiles.Count > i && save.Profiles[i] != null)
                    ProfileList[i].MessageText.Text = save.Profiles[i];
            }

            if(save != null && save.Port != null) {

                _comPorts.Add(save.Port);


                int a = ComBox.Items.IndexOf(save.Port);

                if(ComBox.Items.Contains(save.Port))
                    ComBox.SelectedIndex = ComBox.Items.IndexOf(save.Port);
            }
        }

        public void StartThreads() {

            Fork(() => {
                while(true) {

                    string[] currentPorts = SerialPort.GetPortNames();

                    for(int i = 0; i < currentPorts.Length; i++) {

                        if(!_comPorts.Contains(currentPorts[i])) {
                            string port1 = currentPorts[i];
                            Dispatcher.Invoke(new Action(() => _comPorts.Add(port1)));
                        }
                    }

                    for(int i = 0; i < _comPorts.Count; i++) {

                        if(!currentPorts.Contains(_comPorts[i])) {
                            string port = _comPorts[i];
                            Dispatcher.Invoke(new Func<bool>(() => _comPorts.Remove(port)));
                        }
                    }

                    Thread.Sleep(1000);
                }
            });
        }

        public void Fork(Action task) {

            Thread thread = new Thread(task.Invoke) {
                IsBackground = true
            };
            thread.Start();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName) {
            PropertyChangedEventHandler handler = PropertyChanged;
            if(handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }


        private void CustomMessageBox_OnSent(CustomMessageBox sender, string message) {

            PosDisplay display;
            bool success = PosDisplay.TryParse(sender.MessageText.Text, out display);

            if(success) {

                PosManager manager = new PosManager(ComName);

                manager.Send(display);

                manager.Send(PosCommands.HideCursor);

                manager.Close();
            } else {

                PosManager manager = new PosManager(ComName);

                PosDisplay.TryParse("Error: Message too long...", out display);

                manager.Send(display);

                manager.Send(PosCommands.HideCursor);

                manager.Close();
            }
        }

        private void MainWindow_OnClosing(object sender, CancelEventArgs e) {

            Save save = new Save {
                Port = ComName,
                Profiles = new List<string>()
            };

            foreach(CustomMessageBox box in ProfileList) {

                save.Profiles.Add(box.MessageText.Text);
            }

            Save.SaveObject(save);
        }

    }
}
