﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using MqttListener.Configuration;
using MqttListener.Core;
using MqttListener.Interfaces;

namespace MqttListener.ViewModels
{
    public class ServerListViewModel : BaseViewModel, IDialogHost
    {
        private IWritableOptions<AppConfiguration> _appConfigurationOptions;
        private RelayCommand _cancelCommand;
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private string _clientId;
        private RelayCommand _connectCommand;
        private IWritableOptions<ConnectionsList> _connectionListOptions;
        private ObservableCollection<ConnectionItem> _connections;
        private string _errorMessage;
        private bool _isConnecting;
        private ConnectionItem _selectedItem;
        private IServiceProvider _serviceProvider;
        private RelayCommand _disConnectCommand;
        private object _dialog;
        private Action _cancelDialogAction;
        private Action _okDialogAction;
        private RelayCommand _topicsEditCommand;

        public ServerListViewModel(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _connectionListOptions = _serviceProvider.GetService<IWritableOptions<ConnectionsList>>();
            _appConfigurationOptions = _serviceProvider.GetService<IWritableOptions<AppConfiguration>>();

            var connectionsList = _connectionListOptions.Value;
            ClientId = _appConfigurationOptions.Value.ClientId;

            _connections = new ObservableCollection<ConnectionItem>(connectionsList.Connections);
            if (_connections.Count > 0)
            {
                SelectedItem = _connections[0];
            }

            Listener = serviceProvider.GetService<Listener>();
        }

        public ICommand CancelCommand => _cancelCommand ??= new RelayCommand(x => CancelAction());

        public string ClientId
        {
            get => _clientId;
            set => SetProperty(ref _clientId, value);
        }

        public ICommand ConnectCommand => _connectCommand ??= new RelayCommand(async x => await ConnectAction());

        public IEnumerable Connections
        {
            get => _connections;
        }

        public ICommand DisconnectCommand => _disConnectCommand ??= new RelayCommand(x => DisconnectAction());

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

        public Listener Listener { get; private set; }

        public ConnectionItem SelectedItem
        {
            get => _selectedItem;
            set => SetProperty(ref _selectedItem, value);
        }

        private void CancelAction()
        {
            _cancellationTokenSource.Cancel();
            IsConnecting = false;
        }

        private async Task ConnectAction()
        {
            _cancellationTokenSource = new CancellationTokenSource();

            IsConnecting = true;
            ErrorMessage = null;

            try
            {
                await Listener.ConnectAction(SelectedItem, _cancellationTokenSource.Token);
                if (_cancellationTokenSource.IsCancellationRequested)
                    return;

                IsConnecting = false;
                _connectionListOptions.Save();
            }
            catch (Exception e)
            {
                ErrorMessage = e.Message;
            }
        }

        private void DisconnectAction()
        {
            Listener.Disconnect();
        }

        public object Dialog
        {
            get => _dialog;
            private set => SetProperty(ref _dialog, value);
        }

        public void CloseDialog(bool result)
        {
            Dialog = null;
            if (result)
            {
                _okDialogAction?.Invoke();
            }
            else
            {
                _cancelDialogAction?.Invoke();
            }

            _okDialogAction = null;
            _cancelCommand = null;
        }

        public ICommand TopicsEditCommand => _topicsEditCommand ??= 
            new RelayCommand(x => 
                this.Show(new TopicsViewModel(SelectedItem), OkAction, null));

        private void OkAction()
        {
            _connectionListOptions.Save();
            this.OnPropertyChanged(null);
        }

        public Task Show(IDialog dialog, Action okAction, Action cancelAction)
        {
            Dialog = dialog;
            dialog.OnOpen(this);
            _okDialogAction = okAction;
            _cancelDialogAction = cancelAction;

            return Task.CompletedTask;
        }

    }
}