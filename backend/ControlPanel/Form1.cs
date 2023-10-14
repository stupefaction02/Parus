using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ControlPanel
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            runBackend.Text = "Run Backend";

            runBackendButton = new CommandletButton
            {
                RunCommandlet = CreateRunSingleProjectCommandlet("dotnet run Naturistic.Backend"),
                DestroyCommandlet = CreateDestroyCommandlet(),
                CheckBox = checkBox1
            };

            runWebui.Text = "Run WebUI";

            runWebUIButton = new CommandletButton
            {
                RunCommandlet = CreateRunSingleProjectCommandlet("Naturistic.WebUI.csproj"),
                DestroyCommandlet = CreateDestroyCommandlet(),
                CheckBox = checkBox2
            };

            runDummyUser.Text = "Run Dummy User";

            runDummyUserButton = new CommandletButton
            {
                RunCommandlet = CreateRunSingleProjectCommandlet(""),
                DestroyCommandlet = CreateDestroyCommandlet(),
                CheckBox = checkBox3
            };
        }

        CommandletButton runBackendButton;
        CommandletButton runWebUIButton;
        CommandletButton runDummyUserButton;

        private string commandLine => "/c dotnet run --project \"C:\\Users\\Ivan\\Desktop\\sensorium\\NET Projects\\ASPNET\\NatureForYou\\backend\\Naturistic.Backend\\";

        private Action<List<Process>> CreateRunSingleProjectCommandlet(string projectName)
        {
            return new Action<List<Process>>(processes =>
            {
                var process = new Process();
                process.StartInfo.FileName = "CMD.exe";
                process.StartInfo.WorkingDirectory = "C:\\Users\\Ivan\\Desktop\\sensorium\\NET Projects\\ASPNET\\NatureForYou\\backend";
                process.StartInfo.Arguments = "/c dotnet run --project Naturistic.Backend";
                process.Start();
                processes.Add(process);
            });
        }

        private Action<List<Process>> CreateDestroyCommandlet()
        {
            return new Action<List<Process>>(processes =>
            {
                foreach (Process process in processes)
                {
                    process.Kill();
                }

                processes.Clear();
            });
        }

        private void runBackend_Click(object sender, EventArgs e)
        {
            runBackendButton.DoAction();
        }

        private void runWebui_Click(object sender, EventArgs e)
        {

        }
    }

    public class CommandletButton
    {
        public CheckBox CheckBox { get; set; }

        private bool isActive;

        private List<Process> processes = new List<Process>();

        public Action<List<Process>> RunCommandlet { get; set; }

        public Action<List<Process>> DestroyCommandlet { get; set; }

        public void DoAction()
        {
            if (isActive)
            {
                DestroyCommandlet.Invoke(processes);

                CheckBox.Checked = false;

                isActive = false;
            }
            else
            {
                RunCommandlet.Invoke(processes);

                CheckBox.Checked = true;

                isActive = true;
            }
        }
    }
}
