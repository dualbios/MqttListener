using System;
using MqttListener.Interfaces;

namespace MqttListener.Dialogs
{
    internal class DialogItem
    {
        public DialogItem(IDialog viewModel, Action okAction, Action cancelAction)
        {
            OkAction = okAction;
            CancelAction = cancelAction;
            ViewModel = viewModel;
        }

        public Action CancelAction { get; }
        public Action OkAction { get; }
        public IDialog ViewModel { get; }
    }
}