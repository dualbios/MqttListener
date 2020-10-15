using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using MqttListener.Configuration;
using MqttListener.Interfaces;

namespace MqttListener.ViewModels
{
    public class OpenConnectionDialogViewModel : BaseViewModel, IDialog
    {
        private readonly Action<ConnectionItem, CancellationToken> _connectAction;
        private readonly IServiceProvider _serviceProvider;
        private RelayCommand _cancelCommand;
        private ObservableCollection<ConnectionItem> _connections;
        private IDialogHost _dialogHost;

        private RelayCommand _selectConnectionCommand;
        private ConnectionItem _selectedItem;
        private IWritableOptions<ConnectionsList> _connectionListOptions;
        private IWritableOptions<AppConfiguration> _appConfigurationOptions;

        private string _clientId;

        public OpenConnectionDialogViewModel(IServiceProvider serviceProvider, Action<ConnectionItem, CancellationToken> connectAction)
        {
            _connectAction = connectAction;

            _serviceProvider = serviceProvider;
            _connectionListOptions = _serviceProvider.GetService<IWritableOptions<ConnectionsList>>();
            _appConfigurationOptions = _serviceProvider.GetService<IWritableOptions<AppConfiguration>>();
        }

        public ICommand CancelCommand => _cancelCommand ??= new RelayCommand(x => _dialogHost.CloseDialog(false));

        public IEnumerable Connections
        {
            get => _connections;
        }

        public ICommand SelectConnectionCommand => _selectConnectionCommand ??= new RelayCommand(x => SelectConnection());

        public ConnectionItem SelectedItem
        {
            get => _selectedItem;
            set => SetProperty(ref _selectedItem, value);
        }

        public void OnOpen(IDialogHost dialogHost)
        {
            _dialogHost = dialogHost;

            var connectionsList = _connectionListOptions.Value;
            ClientId = _appConfigurationOptions.Value.ClientId;

            _connections = new ObservableCollection<ConnectionItem>(connectionsList.Connections);
            if (_connections.Count > 0)
            {
                SelectedItem = _connections[0];
            }

            OnPropertyChanged(null);
        }

        public string ClientId
        {
            get => _clientId;
            set => SetProperty(ref _clientId, value);
        }

        private void Connected()
        {
            _connectionListOptions.Update(x =>
            {
                x.Connections = _connections.ToArray();
            });

            _dialogHost.CloseDialog(true);
        }

        private void SelectConnection()
        {
            if (ClientId != _appConfigurationOptions.Value.ClientId)
            {
                _appConfigurationOptions.Update(x =>
                {
                    x.ClientId = ClientId;
                });
            }

            _dialogHost.Show(new ConnectDialogViewModel(SelectedItem, _connectAction), Connected, null);
        }
    }
}