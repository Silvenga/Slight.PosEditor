using POS_Editor.Annotations;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace POS_Editor {

    public partial class CustomMessageBox : INotifyPropertyChanged {

        private string _text;

        public delegate void Message(CustomMessageBox sender, string message);

        public event Message Sent;

        public MainWindow ParentWindow {
            get;
            set;
        }

        public string Text {
            get {

                return _text;
            }
            set {
                _text = value;
                OnPropertyChanged(Text);

                PosDisplay display;
                if(!PosDisplay.TryParse(value, out display, ParentWindow.Rows, ParentWindow.Columns)) {
                    SendButton.IsEnabled = false;
                    throw new ApplicationException();
                } else {
                    SendButton.IsEnabled = true;
                }
            }
        }

        public CustomMessageBox(MainWindow window) {

            InitializeComponent();
            IsEnabledChanged += delegate {
                SendButton.IsEnabled = IsEnabled;
                MessageBox.IsEnabled = IsEnabled;
            };

            ParentWindow = window;
            DataContext = this;
        }

        protected virtual void OnSent(string message) {

            Message handler = Sent;
            if(handler != null)
                handler(this, message);
        }

        private void Send(object sender, RoutedEventArgs e) {

            OnSent(MessageBox.Text.Trim());
        }

        private void MessageText_OnKeyDown(object sender, KeyEventArgs e) {

            if(e.Key == Key.Enter)
                Send(sender, null);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName) {

            PropertyChangedEventHandler handler = PropertyChanged;
            if(handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
