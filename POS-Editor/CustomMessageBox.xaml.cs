using POS_Editor.Annotations;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace POS_Editor {

    public partial class CustomMessageBox : UserControl, INotifyPropertyChanged {

        public delegate void Message(CustomMessageBox sender, string message);

        public event Message Sent;

        protected virtual void OnSent(string message) {

            Message handler = Sent;
            if(handler != null)
                handler(this, message);
        }

        public CustomMessageBox() {

            InitializeComponent();
            IsEnabledChanged += delegate {
                SendButton.IsEnabled = IsEnabled;
                MessageText.IsEnabled = IsEnabled;
            };
        }

        private void Send(object sender, RoutedEventArgs e) {

            OnSent(MessageText.Text.Trim());
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
