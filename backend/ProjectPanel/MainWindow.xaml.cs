using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ProjectPanel
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            //workDir = 
        }

        private string workDir = "";

        private Dictionary<string, ProcessCommand> _store =
            new Dictionary<string, ProcessCommand>
            {
                { "api", new ProcessCommand(name: "API", "dotnet run --project Parus.API --no-build --launch-profile Development.Localhost") },
                { "webui", new ProcessCommand(name: "WebUI", "dotnet run --project Parus.WebUI --no-build --launch-profile Development.Localhost") },
                { "cdn", new ProcessCommand(name: "CDN", "dotnet run --project Parus.CDN --no-build --launch-profile Development.Localhost") }
            };

        private void Button_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender != null) {

                Button btn = (Button)sender;

                ProcessCommand command;
                if (_store.TryGetValue(btn.Name, out command))
                {
                    command.Switch();
                }
            }
        }

        public class ProcessCommand
        {
            private readonly string _title;
            private readonly string command;

            ProcessStartInfo _startInfo = new ProcessStartInfo();
            private Process _process;

            public bool Running { get; set; }

            public ProcessCommand(string name, string command)
            {
                this._title = name;
                this.command = command;

                _startInfo.RedirectStandardInput = true;
                _startInfo.RedirectStandardOutput = true;
                //_startInfo.CreateNoWindow = true;
                _startInfo.UseShellExecute = false;

                _startInfo.WindowStyle = ProcessWindowStyle.Normal;
                _startInfo.FileName = @"C:\Windows\System32\cmd.exe";
                _startInfo.Arguments = command;
            }

            public void Switch()
            {
                if (Running)
                {
                    _process.StandardInput.Close();
                    _process.StandardInput.Dispose();

                    _process.Kill();

                    Running = false;

                    _process = null;
                }
                else
                {
                    _process = new Process { StartInfo = _startInfo };

                    _process.Start();

                    _process.StandardInput.WriteLine("cd \"C:\\Users\\Ivan\\Desktop\\Sensorium\\NET Projects\\ASPNET\\Parus\\backend\"");
                    
                    //_process.StandardInput.WriteLine(command);

                    _process.StandardInput.WriteLine(command);

                    _process.StandardInput.WriteLine($"title {_title}");

                    _process.StandardInput.Flush();

                    _process.WaitForExit();

                    string a = _process.StandardOutput.ReadToEnd();


                    Running = true;
                }
            }
        }
    }
}