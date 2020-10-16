using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using MqttListener.Configuration;
using MqttListener.Interfaces;

namespace MqttListener.ViewModels
{
    public class ConnectDialogViewModel : BaseViewModel, IDialog
    {
        private readonly ConnectionItem _selectedItem;
        private readonly Func<ConnectionItem, CancellationToken, Task> _connectAction;
        private ICommand _cancelCommand;
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private IDialogHost _dialogHost = null;
        private string _errorMessage;

        private bool _isConnecting;

        public ConnectDialogViewModel(ConnectionItem selectedItem, Func<ConnectionItem, CancellationToken, Task> connectAction)
        {
            _selectedItem = selectedItem;
            _connectAction = connectAction;
        }

        public ICommand CancelCommand => _cancelCommand ??= new RelayCommand(x => OnCancel());

        public string ErrorMessage
        {
            get => _errorMessage;
            private set => SetProperty(ref _errorMessage, value);
        }

        public bool IsConnecting
        {
            get => _isConnecting;
            private set => SetProperty(ref _isConnecting, value);
        }

        public void OnOpen(IDialogHost dialogHost)
        {
            _dialogHost = dialogHost;
            Task.Run(async () =>
            {
                IsConnecting = true;
                ErrorMessage = null;

                try
                {
                    await _connectAction?.Invoke(_selectedItem, _cancellationTokenSource.Token);

                    if (_cancellationTokenSource.IsCancellationRequested)
                        return;

                    _dialogHost.CloseDialog(true);
                    IsConnecting = false;
                }
                catch (Exception e)
                {
                    ErrorMessage = e.Message;
                }
            }
                , _cancellationTokenSource.Token);
        }

        private void OnCancel()
        {
            _cancellationTokenSource.Cancel();
            _dialogHost.CloseDialog(false);
        }
    }
}