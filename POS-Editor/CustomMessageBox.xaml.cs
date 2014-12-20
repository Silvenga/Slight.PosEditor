#region Usings

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

using POS_Editor.Annotations;

#endregion

namespace POS_Editor {

    public partial class CustomMessageBox : INotifyPropertyChanged {

        public delegate void Message(CustomMessageBox sender, string message);

        private string _text;

        public CustomMessageBox(MainWindow window) {

            InitializeComponent();
            IsEnabledChanged += delegate {
                SendButton.IsEnabled = IsEnabled;
                MessageBox.IsEnabled = IsEnabled;
            };

            ParentWindow = window;
            DataContext = this;
        }

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
                }
                SendButton.IsEnabled = true;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public event Message Sent;

        protected virtual void OnSent(string message) {

            var handler = Sent;
            if(handler != null) {
                handler(this, message);
            }
        }

        private void Send(object sender, RoutedEventArgs e) {

            OnSent(MessageBox.Text.Trim());
        }

        private void MessageText_OnKeyDown(object sender, KeyEventArgs e) {

            if(e.Key == Key.Enter) {
                Send(sender, null);
            }
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName) {

            var handler = PropertyChanged;
            if(handler != null) {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

    }

}