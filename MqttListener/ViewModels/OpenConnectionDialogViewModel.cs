using System.Windows.Input;
using MqttListener.Interfaces;

namespace MqttListener.ViewModels
{
    public class OpenConnectionDialogViewModel : BaseViewModel, IDialog
    {
        private RelayCommand _cancelCommand;
        private IDialogHost _dialogHost;
        private RelayCommand _selectConnectionCommand;

        public OpenConnectionDialogViewModel()
        {
        }

        public ICommand CancelCommand => _cancelCommand ??= new RelayCommand(x => _dialogHost.CloseDialog(false));
        public ICommand SelectConnectionCommand => _selectConnectionCommand ??= new RelayCommand(x => SelectConnection());

        public void Initialize(IDialogHost dialogHost)
        {
            _dialogHost = dialogHost;
        }

        private void SelectConnection()
        {
            _dialogHost.Show(new ConnectDialogViewModel(), () => _dialogHost.CloseDialog(true), null);
        }
    }
}