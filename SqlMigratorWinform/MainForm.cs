using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Diagnostics;

namespace SqlMigratorWinform
{


    public partial class MainForm : Form
    {
        public Process process = null;
        private string cmdCommand;

        public MainForm()
        {
            InitializeComponent();
            SqlHelper.Instance.CurrentSettings = new NameValueCollection();
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {

            clearForm();
            SqlHelper.Instance.CurrentSettings["ConnectionString"] = txtDBConnection.Text.Trim();
            cmdCommand = @"tools\Migrate.exe /connection " +
                       SqlHelper.Instance.CurrentSettings["ConnectionString"] +
                       " /db SqlServer2008 /target SqlMigratorCore.dll";
            var tables = SqlHelper.Instance.GetTableNames();
            if (tables.Count == 0)
            {
                lblMessage.Text = @"请先加载有效的资源";
            }
            else if (!tables.Contains("VersionInfo"))
            {
                lblMessage.Text = @"当前数据库没有进行版本管理";
            }
            else
            {
                loadVersion();
            }
        }

        private void btnMigrate_Click(object sender, EventArgs e)
        {
            if (checkConn())
            {
                if (string.IsNullOrWhiteSpace(cmdCommand))
                {
                    lblMessage.Text = @"请先加载链接";
                    return;
                }
                runCmd(cmdCommand);
                txtLog.Text = @"运行中";
            }
        }

        private void btnRollbackOnce_Click(object sender, EventArgs e)
        {
            if (checkConn())
            {
                if (string.IsNullOrWhiteSpace(cmdCommand))
                {
                    lblMessage.Text = @"请先加载链接";
                    return;
                }
                runCmd(cmdCommand + " -t rollback --step 1");
                txtLog.Text = @"运行中";
            }
        }

        private void btnRollbackAll_Click(object sender, EventArgs e)
        {
            if (checkConn())
            {
                if (string.IsNullOrWhiteSpace(cmdCommand))
                {
                    lblMessage.Text = @"请先加载链接";
                    return;
                }
                runCmd(cmdCommand + " -t rollback:all");
                txtLog.Text = @"运行中";
            }
        }

        private void loadVersion()
        {
            var versions = SqlHelper.Instance.GetAllVersion();
            if (versions.Count != 0)
            {
                txtLastVersion.Text = versions.OrderByDescending(x => x.AppliedOn).ThenByDescending(x => x.Version).First().Version.ToString();
            }
            else
            {
                lblMessage.Text = @"当前数据库没有进行版本管理";
            }
            dgViewInfo.DataSource = versions;
        }

        private void clearForm()
        {
            txtLastVersion.Text = "";
            txtLog.Text = "";
            lblMessage.Text = "";

            dgViewInfo.DataSource = null;
        }

        private void runCmd(string command)
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            process = new Process();
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.WorkingDirectory = ".";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.CreateNoWindow = true;
            //Process.Start("cmd.exe");
            process.OutputDataReceived += new DataReceivedEventHandler(OutputHandler);
            process.Start();
            process.StandardInput.WriteLine(command);
            //process.StandardInput.WriteLine("exit");
            process.BeginOutputReadLine();
        }

        private void OutputHandler(object sendingProcess, DataReceivedEventArgs outLine)
        {
            if (!String.IsNullOrEmpty(outLine.Data))
            {
                StringBuilder sb = new StringBuilder(this.txtLog.Text);
                this.txtLog.Text = sb.AppendLine(outLine.Data).ToString();
                this.txtLog.SelectionStart = this.txtLog.Text.Length;
                this.txtLog.ScrollToCaret();
            }
        }

        private void OutPutForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (process != null)
                process.Close();
        }

        private bool checkConn()
        {
            if (SqlHelper.Instance.CurrentSettings["ConnectionString"] == null)
            {
                if (!string.IsNullOrWhiteSpace(txtDBConnection.Text.Trim()))
                {
                    SqlHelper.Instance.CurrentSettings["ConnectionString"] = txtDBConnection.Text.Trim();
                }
                else
                {
                    lblMessage.Text = @"请先加载有效的资源";
                    return false;
                }
            }
            return true;
        }


    }
}
