using System;
using System.Collections;
using System.Threading.Tasks;
using MqttListener.Dialogs;
using MqttListener.Interfaces;

namespace MqttListener.ViewModels
{
    public class MainWindowViewModel : BaseViewModel, IDialogHost
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly Stack _dialogsQueue = new Stack();
        private Action _cancelAction = null;
        private IDialog _dialogViewModel;
        private Action _okAction = null;
        private object _serverViewModel;

        public MainWindowViewModel(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            ServerViewModel = new ServerViewModel(serviceProvider, this);
        }

        public IDialog DialogViewModel
        {
            get => _dialogViewModel;
            private set => SetProperty(ref _dialogViewModel, value);
        }

        public object ServerViewModel
        {
            get => _serverViewModel;
            private set => SetProperty(ref _serverViewModel, value);
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