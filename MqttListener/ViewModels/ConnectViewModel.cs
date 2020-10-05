using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MqttListener.Configuration;

namespace MqttListener.ViewModels
{
    public class ConnectViewModel : BaseViewModel
    {
        private readonly IServiceProvider _serviceProvider;
        private ICommand _cancelConmmand;
        private CancellationTokenSource _cancellationTokenSource;
        private Action<ConnectionItem> _connectAction;
        private ICommand _connectCommand;
        private ObservableCollection<ConnectionItem> _connections;
        private string _errorMessage;
        private bool _isConnecting;
        private ConnectionItem _selectedItem;

        public ConnectViewModel(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public ICommand CancelConmmand => _cancelConmmand ??= new RelayCommand(x => Cancel());

        public ICommand ConnectCommand => _connectCommand ??= new RelayCommand(async x =>
                {
                    _cancellationTokenSource = new CancellationTokenSource();
                    await Connect();
                }, x => SelectedItem != null);

        public IEnumerable Connections
        {
            get => _connections;
        }

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

        public ConnectionItem SelectedItem
        {
            get => _selectedItem;
            set => SetProperty(ref _selectedItem, value);
        }

        public void Initialize(Action<ConnectionItem> connectAction)
        {
            _connectAction = connectAction;
            var config = _serviceProvider.GetService<IOptions<AppConfiguration>>().Value;
            _connections = new ObservableCollection<ConnectionItem>(config.Connections);
            if (_connections.Count > 0)
            {
                SelectedItem = _connections[0];
            }

            OnPropertyChanged(null);
        }

        private void Cancel()
        {
            _cancellationTokenSource.Cancel(true);
            IsConnecting = false;
        }

        private async Task Connect()
        {
            IsConnecting = true;
            ErrorMessage = null;
            await Task.Run(async () =>
            {
                try
                {
                    await Task.Delay(150, _cancellationTokenSource.Token);

                    _connectAction(SelectedItem);
                    SaveAppSettings();
                    IsConnecting = false;
                }
                catch (Exception e)
                {
                    ErrorMessage = e.Message;
                }
            }, _cancellationTokenSource.Token);
        }

        private void SaveAppSettings()
        {
        }
    }
}