using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
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
        private IWritableOptions<ConnectionsList> _writableOptions;

        public OpenConnectionDialogViewModel(IServiceProvider serviceProvider, Action<ConnectionItem, CancellationToken> connectAction)
        {
            _connectAction = connectAction;

            _serviceProvider = serviceProvider;
            _writableOptions = _serviceProvider.GetService<IWritableOptions<ConnectionsList>>();
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

            var config = _serviceProvider.GetService<IOptions<AppConfiguration>>().Value;
            _connections = new ObservableCollection<ConnectionItem>(config.ConnectionsList.Connections);
            if (_connections.Count > 0)
            {
                SelectedItem = _connections[0];
            }

            OnPropertyChanged(null);
        }

        private void Connected()
        {
            _writableOptions.Update(x =>
            {
                x.Connections = _connections.ToArray();
            });

            _dialogHost.CloseDialog(true);
        }

        private void SelectConnection()
        {
            _dialogHost.Show(new ConnectDialogViewModel(SelectedItem, _connectAction), Connected, null);
        }
    }
}