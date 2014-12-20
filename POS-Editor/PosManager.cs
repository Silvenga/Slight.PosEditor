#region Usings

using System;
using System.IO.Ports;
using System.Linq;

#endregion

namespace POS_Editor {

    public class PosManager {

        public PosManager(string interfaceName) {

            if(string.IsNullOrWhiteSpace(interfaceName)) {
                throw new ArgumentException("Interface Name must have a value.");
            }

            if(!DoesDeviceExist(interfaceName)) {
                throw new ArgumentException("COM port does not exist.");
            }

            InterfaceName = interfaceName;
            Device = new SerialPort(InterfaceName);

            Device.Open();

            if(!Device.IsOpen) {
                throw new ArgumentException("Could not open give COM port.");
            }
        }

        public string InterfaceName {
            get;
            private set;
        }

        public SerialPort Device {
            get;
            private set;
        }

        public bool IsReady {
            get {
                return Device != null && Device.IsOpen;
            }
        }

        public void Close() {

            if(!IsReady) {
                throw new Exception("COM is not open or used.");
            }

            Device.Close();
            Device = null;
        }

        public void Send(PosDisplay display) {

            Send(PosCommands.OverwriteMode);
            Send(PosCommands.HideCursor);
            Send(PosCommands.Clear);

            for(var i = 0; i < display.Rows; i++) {

                Send(display[i]);

                if(i != display.Rows - 1) {

                    Send(PosCommands.Return);
                    Send(PosCommands.Feed);
                }
            }
        }

        public void Send(PosCommands command) {

            Send(command.Stringify());
        }

        private void Send(string text) {

            Device.Write(text);
        }

        public static bool DoesDeviceExist(string name) {

            if(string.IsNullOrWhiteSpace(name)) {
                return false;
            }

            var knownPortNames = SerialPort.GetPortNames();
            return knownPortNames.Select(x => x.Equals(name)).Any();
        }

    }

}