
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Data;

namespace POS_Editor {

    public partial class MainWindow : INotifyPropertyChanged {

        private const int Profiles = 5;

        private readonly ObservableCollection<string> _comPorts;
        private string _comName;
        private int _rows;
        private int _columns;

        public int Rows {
            get {
                return _rows;
            }
            set {

                _rows = value;
                OnPropertyChanged("Rows");
                OnPropertyChanged("IsReady");

                if(Rows <= 0)
                    throw new ApplicationException();
            }
        }

        public int Columns {
            get {
                return _columns;
            }
            set {
                _columns = value;
                OnPropertyChanged("Columns");
                OnPropertyChanged("IsReady");

                if(Columns <= 0)
                    throw new ApplicationException();
            }
        }

        public string ComName {
            get {
                return _comName;
            }
            set {
                _comName = value;
                OnPropertyChanged("ComName");
                OnPropertyChanged("IsReady");

                if(!PosManager.DoesDeviceExist(ComName))
                    throw new ApplicationException();
            }
        }

        public List<CustomMessageBox> ProfileList {
            get;
            set;
        }

        public bool IsReady {
            get {
                return PosManager.DoesDeviceExist(ComName) && Rows > 0 && Columns > 0;
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

            Window.Title = "POS Editor - " + Assembly.GetEntryAssembly().GetName().Version;

            Rows = 4;
            Columns = 20;

            ComBox.ItemsSource = _comPorts;

            Main();

            StartThreads();

        }

        private void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs unhandledExceptionEventArgs) {

            Exception e = (Exception) unhandledExceptionEventArgs.ExceptionObject;

            string message = "";

            message += "\r\nCrashed on: " + e.Message + "\r\nSource: " + e.Source + "\r\nTrace: " + e.StackTrace + "\r\nDue to: " + e.InnerException;

            File.AppendAllText("com.silvenga.pos-editor.crashreport.log", message);

        }

        public void Main() {

            Save save = Save.OpenObject();

            ProfileList = new List<CustomMessageBox>();

            for(int i = 0; i < Profiles; i++) {

                ProfileList.Add(new CustomMessageBox(this));
                ProfileList[i].Sent += CustomMessageBox_OnSent;

                Binding binding = new Binding("IsReady") {
                    Source = this
                };
                BindingOperations.SetBinding(ProfileList[i], IsEnabledProperty, binding);

                ProfilePanel.Children.Add(ProfileList[i]);

                if(save != null && save.Profiles != null && save.Profiles.Count > i && save.Profiles[i] != null)
                    ProfileList[i].Text = save.Profiles[i];
            }

            if(save != null && save.Port != null) {

                _comPorts.Add(save.Port);

                if(ComBox.Items.Contains(save.Port))
                    ComBox.SelectedIndex = ComBox.Items.IndexOf(save.Port);
            }

            if(save != null && save.Columns != null && save.Columns > 0) {

                Columns = (int) save.Columns;
            }

            if(save != null && save.Rows != null && save.Rows > 0) {

                Rows = (int) save.Rows;
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
            bool success = PosDisplay.TryParse(sender.Text, out display, Rows, Columns);

            if(success) {

                PosManager manager = new PosManager(ComName);

                manager.Send(display);

                manager.Send(PosCommands.HideCursor);

                manager.Close();
            } else {

                PosManager manager = new PosManager(ComName);

                PosDisplay.TryParse("Error: Message too long...", out display, Rows, Columns);

                manager.Send(display);

                manager.Send(PosCommands.HideCursor);

                manager.Close();
            }
        }

        private void MainWindow_OnClosing(object sender, CancelEventArgs e) {

            Save save = new Save {
                Port = ComName,
                Columns = Columns,
                Rows = Rows,
                Profiles = new List<string>()
            };

            foreach(CustomMessageBox box in ProfileList) {

                save.Profiles.Add(box.Text);
            }

            Save.SaveObject(save);
        }
    }
}
