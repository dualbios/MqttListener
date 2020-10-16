using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using MqttListener.Configuration;
using MqttListener.Interfaces;

namespace MqttListener.ViewModels
{
    public class OpenConnectionDialogViewModel : BaseViewModel, IDialog
    {
        private readonly Func<ConnectionItem, CancellationToken, Task> _connectAction;
        private readonly IServiceProvider _serviceProvider;
        private IWritableOptions<AppConfiguration> _appConfigurationOptions;
        private RelayCommand _cancelCommand;
        private string _clientId;
        private IWritableOptions<ConnectionsList> _connectionListOptions;
        private ObservableCollection<ConnectionItem> _connections;
        private IDialogHost _dialogHost;

        private RelayCommand _selectConnectionCommand;
        private ConnectionItem _selectedItem;
        private RelayCommand _topicsEditCommand;

        public OpenConnectionDialogViewModel(IServiceProvider serviceProvider, Func<ConnectionItem, CancellationToken, Task> connectAction)
        {
            _connectAction = connectAction;

            _serviceProvider = serviceProvider;
            _connectionListOptions = _serviceProvider.GetService<IWritableOptions<ConnectionsList>>();
            _appConfigurationOptions = _serviceProvider.GetService<IWritableOptions<AppConfiguration>>();
        }

        public ICommand CancelCommand => _cancelCommand ??= new RelayCommand(x => _dialogHost.CloseDialog(false));

        public string ClientId
        {
            get => _clientId;
            set => SetProperty(ref _clientId, value);
        }

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

        public ICommand TopicsEditCommand => _topicsEditCommand ??= new RelayCommand(x => EditTopics());

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

        private void Connected()
        {
            _connectionListOptions.Update(x =>
            {
                x.Connections = _connections.ToArray();
            });

            _dialogHost.CloseDialog(true);
        }

        private void EditTopics()
        {
            TopicsViewModel topicsViewModel = new TopicsViewModel(SelectedItem);
            _dialogHost.Show(topicsViewModel, () =>
                {
                    SelectedItem.Topics = topicsViewModel.Topics.Where(x => !(x is NewSettingTopicItem)).Select(x => x.Name).ToList();
                },
                null);
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