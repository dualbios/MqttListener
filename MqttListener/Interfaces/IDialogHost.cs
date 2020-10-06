using System;
using System.Threading.Tasks;
using MqttListener.ViewModels;

namespace MqttListener.Interfaces
{
    public interface IDialogHost
    {
        void CloseDialog(bool result);

        Task Show(IDialog dialog, Action okAction, Action cancelAction);
    }
}