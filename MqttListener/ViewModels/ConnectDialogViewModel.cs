using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using MqttListener.Interfaces;

namespace MqttListener.ViewModels
{
    public class ConnectDialogViewModel : BaseViewModel, IDialog
    {
        private IDialogHost _dialogHost = null;
        private ICommand _cancelCommand;
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        public void Initialize(IDialogHost dialogHost)
        {
            _dialogHost = dialogHost;
            Task.Run(async () =>
            {
                await Task.Delay(2000);
                if (_cancellationTokenSource.Token.IsCancellationRequested)
                    return;

                _dialogHost.CloseDialog(true);
            }
                , _cancellationTokenSource.Token);
        }

        public ICommand CancelCommand => _cancelCommand ??= new RelayCommand(x=>OnCancel());

        private void OnCancel()
        {
            _cancellationTokenSource.Cancel();
            _dialogHost.CloseDialog(false);
        }
    }
}