using System;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;

namespace Autodrive.RS232
{
    public class SerialMessenger : IDisposable
    {
        public SerialPort Port;
        private byte[] _buffer = new byte[1024*4];
        private int _bufferFilled;

        #region START AND STOP

        public void Start()
        {
            IsRunning = true;
            Port.Open();
            Port.DataReceived += port_DataReceived;
        }

        public void Stop()
        {
            Port.Close();
            IsRunning = false;
        }

        #endregion

        public SerialMessenger(string commName, string messageDelimeter = "\r\n")
        {
            CommName = commName;
            Delimeter = messageDelimeter;
            Port = new SerialPort();
        }

        public string Delimeter { get; private set; }

        public string CommName { get; private set; }

        public bool IsRunning { get; private set; }

        public void Dispose()
        {
            Port.Dispose();
        }

        /// <summary>
        ///     Since all the data may not come in at one time, this begins to fill a buffer for later processing. There is another
        ///     process
        ///     "ProcessBuffer" that starts to look for complete messages from within the buffer.
        /// </summary>
        private void port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            ProcessBuffer();
        }

        private void ReadBytes()
        {
            lock (_buffer)
            {
                int bytesReceived = Port.Read(_buffer, _bufferFilled, Port.BytesToRead);
                if (bytesReceived > 0)
                {
                    _bufferFilled += bytesReceived;
                    if (_bufferFilled >= _buffer.Length)
                    {
                        //Buffer is full - clear buffer
                        _buffer = new byte[1024*4];
                        _bufferFilled = 0;
                    }
                }
            }
        }

        private void ProcessBuffer()
        {
            string completeMessage = string.Empty;
            Thread.Sleep(Port.ReadTimeout);
            completeMessage = Port.ReadExisting();
            OnMessageReceived(this.CommName, completeMessage);
        }

        public void SendMessage(string message)
        {
            byte[] msg = Encoding.ASCII.GetBytes(message + Delimeter);
            var hex = BitConverter.ToString(msg);
            Port.Write(msg, 0, msg.Length);
            Debug.Print($"{CommName} OUT : {message} ({hex})");
        }

        public void SendMessage(byte[] message)
        {
            var msg = Encoding.ASCII.GetString(message);
            SendMessage(msg);
        }

        public void SendMessage(byte message)
        {
            SendMessage(new byte[] { message });
        }

        public void SendMessage(byte message, Action<string> responseCallBack, int msTimeout = 2000)
        {
            ManualResetEvent mr = new ManualResetEvent(false);
            var responseHandler = new MessageReceivedHandler((com, resp) =>
            {
                responseCallBack(resp);
                byte[] msg = Encoding.ASCII.GetBytes(resp);
                var hex = BitConverter.ToString(msg);
                Debug.Print($"{CommName} IN : {resp} ({hex})");
                mr.Set();
            });

            MessageReceived += responseHandler;
            SendMessage(message);
            mr.WaitOne(msTimeout);
        }

        public void SendMessage(string message, Action<string> responseCallBack, int msTimeout = 2000)
        {
            ManualResetEvent mr = new ManualResetEvent(false);
            var responseHandler = new MessageReceivedHandler((com, resp) =>
            {
                responseCallBack(resp);
                byte[] msg = Encoding.ASCII.GetBytes(resp);
                var hex = BitConverter.ToString(msg);
                Debug.Print($"{CommName} IN : {resp} ({hex})");
                mr.Set();
            });

            MessageReceived += responseHandler;
            SendMessage(message);
            mr.WaitOne(msTimeout);
        }

        #region EVENT SUBSCRIPTION CODE

        public delegate void MessageReceivedHandler(string comName, string message);

        public event MessageReceivedHandler MessageReceived;

        protected virtual void OnMessageReceived(string comName, string message)
        {
            if (MessageReceived != null)
                MessageReceived(CommName, message);
        }

        #endregion
    }
}