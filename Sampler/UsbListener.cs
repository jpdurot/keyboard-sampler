#if !DOTNETCORE
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LibUsbDotNet;
using LibUsbDotNet.LibUsb;
using LibUsbDotNet.Main;

namespace Sampler
{
    public class UsbListener : INotifyPropertyChanged
    {

        private IUsbDevice _device;
        private UsbEndpointReader _reader;

        private ReadEndpointID _endpointId;
        private bool _isOpen;
        private bool _communicationOk;

        private readonly object _lock = new object();

        private Thread _thread;

        public UsbListener()
        {
            EndPointId = ReadEndpointID.Ep01;
            _thread = new Thread(Communication);
        }

        public bool IsOpen
        {
            get
            {
                return _isOpen;
            }
            set
            {
                if (_isOpen != value)
                {
                    _isOpen = value;
                    OnPropertyChanged("IsOpen");
                }
            }
        }

        public bool CommunicationOk
        {
            get
            {
                return _communicationOk;
            }
            set
            {
                if (_communicationOk != value)
                {
                    _communicationOk = value;
                    OnPropertyChanged("CommunicationOk");
                }
            }
        }

        public ReadEndpointID EndPointId
        {
            get
            {
                return _endpointId;
            }
            private set
            {
                if (_endpointId != value)
                {
                    _endpointId = value;
                    OnPropertyChanged("EndPointId");
                }
            }
        }

        private void Communication()
        {
            while (true)
            {
                byte[] buffer = new byte[8];
                int length;
                ErrorCode ret = _reader.Read(buffer, 500, out length);
                if (ret == ErrorCode.None)
                {
                    OnReaderDataReceived(buffer);
                }
            }
        }

        private ReadEndpointID GetEndpoint()
        {
            bool found = false;
            byte readerId = (byte)ReadEndpointID.Ep01;
            while (!found && (byte)readerId <= (byte)ReadEndpointID.Ep15)
            {
                _reader = _device.OpenEndpointReader((ReadEndpointID)readerId);
                byte[] buffer = new byte[8];
                int length;
                ErrorCode ret = _reader.Read(buffer, 500, out length);
                found = (ret != ErrorCode.Win32Error);
                if (!found)
                {
                    readerId++;
                }
            }
            return (ReadEndpointID)readerId;
        }

        public void Start()
        {
            lock (_lock)
            {
                UsbRegDeviceList allLibUsbDevices = UsbDevice.AllLibUsbDevices;
                if (allLibUsbDevices.Count > 0)
                {
                    UsbDevice dev;
                    IsOpen = ((LibUsbRegistry)allLibUsbDevices.First()).Open(out dev);
                    _device = dev as IUsbDevice;
                    // Select config
                    bool configuration = _device.SetConfiguration(1);

                    // Claim interface
                    bool claimInterface = _device.ClaimInterface(0);
                    /*bool found = false;
                byte readerId = (byte) ReadEndpointID.Ep01;
                while (!found && (byte)readerId <= (byte)ReadEndpointID.Ep15)
                {
                    _reader = _device.OpenEndpointReader((ReadEndpointID)readerId);
                    byte[] buffer = new byte[1024];
                    int length;
                    ErrorCode ret = _reader.Read(buffer, 100, out length);
                    found = (ret != ErrorCode.Win32Error);
                    readerId++;
                }*/
                    EndPointId = GetEndpoint();
                    _reader = _device.OpenEndpointReader(EndPointId);
                    _thread.Start();
                    //_reader.DataReceivedEnabled = true;
                    //_reader.ReadBufferSize = 8;
                    //_reader.DataReceived += OnReaderDataReceived;
                }
            }
        }


        void OnReaderDataReceived(byte[] buffer)
        {
            CommunicationOk = true;
            if (buffer[2] != 0)
            {
                if (KeyDown != null)
                {
                    KeyDown(this, new KeyDownEventArgs(buffer));
                }
            }
        }

        public void Stop()
        {
            lock (_lock)
            {
                IsOpen = false;
                EndPointId = ReadEndpointID.Ep01;
                CommunicationOk = false;
                if (_reader != null)
                {
                    _thread.Abort();
                    _thread.Join();
                    //_reader.DataReceivedEnabled = false;
                    //_reader.DataReceived -= OnReaderDataReceived;
                    _reader.ReadFlush();
                    _reader.Abort();
                    _reader = null;
                    
                }
                if (_device != null)
                {
                    _device.ReleaseInterface(0);
                    _device.Close();
                    _device = null;
                    UsbDevice.Exit();
                }
                

            }
        }

        public event EventHandler<KeyDownEventArgs> KeyDown;
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
#endif
