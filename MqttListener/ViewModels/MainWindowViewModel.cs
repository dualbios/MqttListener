using System;
using System.Collections;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using MqttListener.Core;
using MqttListener.Dialogs;
using MqttListener.Interfaces;

namespace MqttListener.ViewModels
{
    public class MainWindowViewModel : BaseViewModel, IDialogHost
    {
        private readonly Stack _dialogsQueue = new Stack();
        private readonly IServiceProvider _serviceProvider;
        private Action _cancelAction = null;
        private IDialog _dialogViewModel;
        private Action _okAction = null;
        private ViewType _selectedView;
        private ServerListViewModel _serverListViewModel;
        private TreeViewModel _treeViewModel;
        private object _view;
        private HistoryViewModel _historyVew;

        public MainWindowViewModel(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _serverListViewModel =  serviceProvider.GetService<ServerListViewModel>();
            _treeViewModel = serviceProvider.GetService<TreeViewModel>();
            _historyVew = serviceProvider.GetService<HistoryViewModel>();

            View = _serverListViewModel;

            Listener = serviceProvider.GetService<Listener>();
            Listener.OnConnectedEventHandler += (sender, args) => SelectedView = ViewType.Tree;
            Listener.OnDisconnectedEventHandler += (sender, args) => SelectedView = ViewType.Tree;
        }

        public IDialog DialogViewModel
        {
            get => _dialogViewModel;
            private set => SetProperty(ref _dialogViewModel, value);
        }

        public Listener Listener { get; private set; }

        public ViewType SelectedView
        {
            get => _selectedView;
            set
            {
                if (SetProperty(ref _selectedView, value))
                {
                    View = GetView(value);
                }
            }
        }

        private object GetView(ViewType viewType)
        {
            return viewType switch
            {
                ViewType.Server => _serverListViewModel,
                ViewType.Tree => _treeViewModel,
                ViewType.History => _historyVew,
                _ => null,
            };
        }


        public ServerListViewModel ServerListViewModel
        {
            get => _serverListViewModel;
            set => SetProperty(ref _serverListViewModel, value);
        }

        public object ServerViewModel
        {
            get => _treeViewModel;
        }

        public object View
        {
            get => _view;
            private set => SetProperty(ref _view, value);
        }

        public void CloseDialog(bool result)
        {
            DialogItem dialogItem = _dialogsQueue.Pop() as DialogItem;
            DialogViewModel = dialogItem?.ViewModel;
            var oldOkAction = _okAction;
            var oldCancelAction = _cancelAction;

            _okAction = dialogItem?.OkAction;
            _cancelAction = dialogItem?.CancelAction;

            if (result)
            {
                oldOkAction?.Invoke();
            }
            else
            {
                oldCancelAction?.Invoke();
            }
        }

        public Task Show(IDialog dialog, Action okAction, Action cancelAction)
        {
            dialog.OnOpen(this);

            _dialogsQueue.Push(new DialogItem(DialogViewModel, _okAction, _cancelAction));

            DialogViewModel = dialog;
            _okAction = okAction;
            _cancelAction = cancelAction;

            return Task.CompletedTask;
        }
    }
}