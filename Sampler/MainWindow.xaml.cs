using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Microsoft.Owin.Hosting;
using NAudio.Wave;
using Sampler.Server.Services;

namespace Sampler
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : INotifyPropertyChanged
    {
        private readonly UsbListener _listener;

        private bool _communicationEstablished = false;

        private Sampler1 _sampler1;

        private int _lastKeyCode = -1;

        private ICommand _taskBarClickCommand;
        private ObservableCollection<DirectSoundDeviceInfo> _devices;
        private DirectSoundDeviceInfo _device;

        private IDisposable _webServer;

        public ICommand TaskbarClickCommand
        {
            get
            {
                return (_taskBarClickCommand ?? (_taskBarClickCommand = new TaskbarClickCommand()));
            }
        }



        public UsbListener UsbListener
        {
            get
            {
                return _listener;
            }
        }

        public int LastKeyCode
        {
            get
            {
                return _lastKeyCode;
            }
            private set
            {
                if (_lastKeyCode != value)
                {
                    _lastKeyCode = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<DirectSoundDeviceInfo> Devices
        {
            get
            {
                return _devices;
            }
            private set
            {
                if (_devices != value)
                {
                    _devices = value;
                    OnPropertyChanged();
                }
            }
        }

        public DirectSoundDeviceInfo CurrentDevice
        {
            get
            {
                return _device;
            }
            private set
            {
                if (_device != value)
                {
                    _device = value;
                    OnPropertyChanged();
                }
            }
        }

        public MainWindow()
        {
            _listener = new UsbListener();
            InitializeComponent();

            DbConnectionService.Current.Connect();

            string baseAddress = "http://+:9000/";

            // Start OWIN host 
            _webServer = WebApp.Start<Startup>(url: baseAddress);

            Loaded += MainWindow_Loaded;
            
            Application.Current.Exit += OnApplicationExit;
        }

        void OnApplicationExit(object sender, ExitEventArgs e)
        {
            DbConnectionService.Current.Dispose();
            
            StopListener();
            if (_webServer != null)
            {
                _webServer.Dispose();
            }
        }

        void OnUsbKeyDown(object sender, KeyDownEventArgs e)
        {
            LastKeyCode = e.KeyCode;

            if (e.KeyCode == 41)
            {
                _sampler1.IsMuted = !_sampler1.IsMuted;
            }
            else
            {
                _sampler1.PlaySound(e.KeyCode, e.IsShiftPressed);
            }
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Devices = new ObservableCollection<DirectSoundDeviceInfo>(DirectSoundOut.Devices);
            this.PropertyChanged += MainWindow_PropertyChanged;
            _sampler1 = Sampler1.Current;
            ResetListener();
        }

        void MainWindow_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "CurrentDevice")
            {
                _sampler1.ChangeDevice(CurrentDevice.Guid);
            }
        }

        private void ResetListener()
        {
            StopListener();
            _listener.KeyDown += OnUsbKeyDown;
            _listener.Start();
        }

        private void StopListener()
        {
            _listener.KeyDown -= OnUsbKeyDown;
            _listener.Stop();
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private void RefreshClick(object sender, RoutedEventArgs e)
        {
            ResetListener();
        }
    }
}
