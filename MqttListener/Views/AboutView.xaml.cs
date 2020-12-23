using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Timer = System.Timers.Timer;

namespace MqttListener.Views
{
    /// <summary>
    /// Interaction logic for AboutView.xaml
    /// </summary>
    public partial class AboutView : UserControl
    {
        private class RecParam
        {
            public Rectangle Rectangle { get; set; }
            public double XSpeed { get; set; }
            public double YSpeed { get; set; }
        }

        private List<RecParam> _recParams = new List<RecParam>();
        private const int maxSpeed = 5;
        private const int objectCount = 50;
        private const int objectMinSize = 10;
        private const int objectMaxSize = 30;

        private const int offsetMin = 5;
        private const int offsetMax = 9;

        private const int opacityMin = 2;
        private const int opacityMax = 5;
        private Color color = Color.FromArgb(0xFF, 0x53, 0xB3, 0xFF);

        private readonly Timer timer = new Timer(100);
        private Random _random = new Random((int)DateTime.Now.Ticks);

        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        public AboutView()
        {
            InitializeComponent();
            this.Loaded += AboutView_Loaded;
            timer.Elapsed += Timer_Elapsed;
            this.Unloaded += AboutView_Unloaded;

            Assembly executingAssembly = Assembly.GetExecutingAssembly();

            VersionRun.Text = executingAssembly.GetName().Version.ToString();

            var resourceStream = executingAssembly.GetManifestResourceStream("MqttListener.Media.license.txt");
            using (var reader = new StreamReader(resourceStream, Encoding.UTF8))
            {
                LicenseTextBox.Text = reader.ReadToEnd();
            }
        }

        private void AboutView_Unloaded(object sender, RoutedEventArgs e)
        {
            timer.Stop();
            _cancellationTokenSource.Cancel(false);
            timer.Dispose();
        }

        private void AboutView_Loaded(object sender, RoutedEventArgs e)
        {
            for (int index = 0; index < objectCount; index++)
            {
                int size = _random.Next(objectMinSize, objectMaxSize);
                Rectangle rec = new Rectangle() { Width = size, Height = size, RadiusX = size, RadiusY = size };

                GradientStop gradientStop0 = new GradientStop(Color.Multiply(color, (float)_random.NextDouble()), _random.Next(offsetMin, offsetMax) / 10.0);
                GradientStop gradientStop1 = new GradientStop(Color.Multiply(Colors.White, (float)_random.NextDouble()), 1);
                rec.Fill = new RadialGradientBrush(new GradientStopCollection(new[] { gradientStop0, gradientStop1, }));
                rec.Opacity = _random.Next(opacityMin, opacityMax) / 10.0;

                var xSpeed = _random.Next(-maxSpeed, maxSpeed);
                while (xSpeed == 0)
                {
                    xSpeed = _random.Next(-maxSpeed, maxSpeed);
                }

                var ySpeed = _random.Next(-maxSpeed, maxSpeed);
                while (ySpeed == 0)
                {
                    ySpeed = _random.Next(-maxSpeed, maxSpeed);
                }

                _recParams.Add(new RecParam() { Rectangle = rec, XSpeed = xSpeed, YSpeed = ySpeed });
            }

            int doubleSpeed = maxSpeed * 2;

            foreach (RecParam recParam in _recParams)
            {
                BackCanvas.Children.Add(recParam.Rectangle);

                Canvas.SetLeft(recParam.Rectangle, _random.Next(doubleSpeed, (int)(BackCanvas.ActualWidth - recParam.Rectangle.Width - doubleSpeed)));
                Canvas.SetTop(recParam.Rectangle, _random.Next(doubleSpeed, (int)(BackCanvas.ActualHeight - recParam.Rectangle.Height - doubleSpeed)));
            }

            _cancellationTokenSource = new CancellationTokenSource();
            timer.Start();
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

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (_cancellationTokenSource.Token.IsCancellationRequested)
                return;

            try
            {
                this.Dispatcher.Invoke(() =>
                    {
                        if (_cancellationTokenSource.Token.IsCancellationRequested)
                            return;

                        foreach (RecParam recParam in _recParams)
                        {
                            if (_cancellationTokenSource.Token.IsCancellationRequested)
                                break;

                            double left = Canvas.GetLeft(recParam.Rectangle);
                            double top = Canvas.GetTop(recParam.Rectangle);

                            if (left + recParam.Rectangle.Width + 1 >= BackCanvas.ActualWidth)
                            {
                                recParam.XSpeed = -recParam.XSpeed;
                                left= BackCanvas.ActualWidth - recParam.Rectangle.Width - 1;
                            }

                            if (top + recParam.Rectangle.Height + 1 >= BackCanvas.ActualHeight)
                            {
                                recParam.YSpeed = -recParam.YSpeed;
                                top = BackCanvas.ActualHeight - recParam.Rectangle.Height - 1;
                            }

                            if (left <= 0)
                            {
                                recParam.XSpeed = -recParam.XSpeed;
                                left = recParam.XSpeed;
                            }

                            if (top <= 0)
                            {
                                recParam.YSpeed = -recParam.YSpeed;
                                top = recParam.YSpeed;
                            }

                            Canvas.SetLeft(recParam.Rectangle, left + recParam.XSpeed);
                            Canvas.SetTop(recParam.Rectangle, top + recParam.YSpeed);
                        }
                    }
                    , DispatcherPriority.Render
                    , _cancellationTokenSource.Token);
            }
            catch (TaskCanceledException)
            {
                
            }
        }
    }
}