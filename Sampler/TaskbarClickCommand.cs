using System;
using System.Windows;
using System.Windows.Input;

namespace Sampler
{
    public class TaskbarClickCommand : ICommand
    {
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            if (!Application.Current.MainWindow.IsVisible)
            {
                Application.Current.MainWindow.Show();
            }
            Application.Current.MainWindow.WindowState=WindowState.Normal;

            Application.Current.MainWindow.Activate();
        }

        public event EventHandler CanExecuteChanged;
    }
}
