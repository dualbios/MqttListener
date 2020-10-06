using MqttListener.Interfaces;

namespace MqttListener.ViewModels
{
    public class _TestViewModel : BaseViewModel
    {
        private readonly IDialogHost _dialogHost = null;

        public _TestViewModel(IDialogHost dialogHost)
        {
            _dialogHost = dialogHost;
            _dialogHost.Show(new OpenConnectionDialogViewModel(), OpenOkAction, CancelAction);
        }

        private void CancelAction()
        {
            
        }

        private void OpenOkAction()
        {

        }
    }
}