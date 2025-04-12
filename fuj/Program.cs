using System;
using System.Diagnostics;
using System.Security.Principal;
using System.Windows.Forms;

namespace fuj
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (!IsAdministrator())
            {
                RequestAdminPrivileges();
                return;
            }

            Application.Run(new Form1());
        }

        private static void RequestAdminPrivileges()
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = Process.GetCurrentProcess().MainModule.FileName,
                UseShellExecute = true,
                Verb = "runas"
            };

            try
            {
                Process.Start(startInfo);
                MessageBox.Show("�����������Ĺ���Ա�����в���",
                    "Ȩ����ʾ",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch
            {
                MessageBox.Show("��Ҫ����ԱȨ�޲��ܼ�������",
                    "Ȩ�޲���",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
            Environment.Exit(0);
        }

        private static bool IsAdministrator()
        {
            using (var identity = WindowsIdentity.GetCurrent())
            {
                var principal = new WindowsPrincipal(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
        }
    }
}