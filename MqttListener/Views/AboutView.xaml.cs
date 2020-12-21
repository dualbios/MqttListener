using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace MqttListener.Views
{
    /// <summary>
    /// Interaction logic for AboutView.xaml
    /// </summary>
    public partial class AboutView : UserControl
    {
        public AboutView()
        {
            InitializeComponent();
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            // for .NET Core you need to add UseShellExecute = true
            // see https://docs.microsoft.com/dotnet/api/system.diagnostics.processstartinfo.useshellexecute#property-value
            Process process = new Process();
            process.StartInfo.FileName = e.Uri.AbsoluteUri;
            process.StartInfo.UseShellExecute = true;
            process.Start();

            e.Handled = true;
        }
    }
}