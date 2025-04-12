using System;
using System.Diagnostics;
using System.IO;
using System.Security.Principal;
using System.Windows.Forms;

namespace fuj
{
    public partial class Form1 : Form
    {
        private const string TargetDomain = "douyin.com";
        private const string RedirectIP = "127.0.0.1";

        public Form1()
        {
            InitializeComponent();
            InitializeApplication();
        }

        private void InitializeApplication()
        {
            // ���¼�������
            this.Load += Form1_Load;
            this.button1.Click += Button1_Click;

            // ��ʼ״̬���
            CheckAdminStatus();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // �������ʱ�ĳ�ʼ���߼�
            labelStatus.Text = "����";
        }

        private void CheckAdminStatus()
        {
            if (!IsAdministrator())
            {
                labelStatus.Text = "? ���Թ���Ա������б�����";
                button1.Enabled = false;
            }
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            try
            {
                string hostsPath = GetHostsPath();

                if (chkBackup.Checked)
                    CreateBackup(hostsPath);

                if (ModifyHostsFile(hostsPath))
                {
                    RefreshDnsCache();
                    labelStatus.Text = "? �޸ĳɹ���";
                }
            }
            catch (Exception ex)
            {
                ShowErrorDialog($"����ʧ��: {ex.Message}");
            }
        }

        #region ���Ĺ��ܷ���
        private string GetHostsPath()
        {
            return Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.System),
                @"drivers\etc\hosts");
        }

        private void CreateBackup(string hostsPath)
        {
            string backupPath = $"{hostsPath}.{DateTime.Now:yyyyMMddHHmmss}.bak";
            File.Copy(hostsPath, backupPath, true);
            labelStatus.Text = $"�Ѵ�������: {Path.GetFileName(backupPath)}";
        }

        private bool ModifyHostsFile(string hostsPath)
        {
            string[] lines = File.ReadAllLines(hostsPath);
            if (EntryExists(lines)) return false;

            using (var writer = File.AppendText(hostsPath))
            {
                writer.WriteLine($"\n# Modified by FujTool {DateTime.Now}");
                writer.WriteLine($"{RedirectIP}\t{TargetDomain}");
            }
            return true;
        }

        private bool EntryExists(string[] lines)
        {
            foreach (string line in lines)
            {
                if (line.Trim().StartsWith(RedirectIP) &&
                    line.Contains(TargetDomain) &&
                    !line.Trim().StartsWith("#"))
                {
                    return true;
                }
            }
            return false;
        }

        private void RefreshDnsCache()
        {
            ExecuteCommand("ipconfig", "/flushdns", true);
        }

        private void ExecuteCommand(string fileName, string arguments, bool hidden = true)
        {
            var psi = new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = arguments,
                WindowStyle = hidden ? ProcessWindowStyle.Hidden : ProcessWindowStyle.Normal,
                CreateNoWindow = hidden
            };

            using (var process = Process.Start(psi))
            {
                process?.WaitForExit();
            }
        }
        #endregion

        #region ��������
        private static bool IsAdministrator()
        {
            using (var identity = WindowsIdentity.GetCurrent())
            {
                var principal = new WindowsPrincipal(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
        }

        private void ShowErrorDialog(string message)
        {
            MessageBox.Show(this,
                message,
                "����",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
        #endregion
    }
}