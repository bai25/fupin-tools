using System;
using System.Diagnostics;
using System.Security.Principal;

class Program
{
    static void Main(string[] args)
    {
        // 检查管理员权限
        if (!IsAdministrator())
        {
            Console.WriteLine("请以管理员权限重新运行程序！");
            RestartAsAdmin();
            return;
        }

        // 要终止的进程列表
        string[] processesToKill = { "seewocore", "seewoability" };

        foreach (var processName in processesToKill)
        {
            try
            {
                foreach (Process process in Process.GetProcessesByName(processName))
                {
                    try
                    {
                        process.Kill();
                        process.WaitForExit();
                        Console.WriteLine($"已成功终止进程：{processName}.exe");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"终止进程 {processName} 失败：{ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"查找进程 {processName} 失败：{ex.Message}");
            }
        }

        Console.WriteLine("操作完成，按任意键退出...");
        Console.ReadKey();
    }

    // 检查是否具有管理员权限
    static bool IsAdministrator()
    {
        WindowsIdentity identity = WindowsIdentity.GetCurrent();
        WindowsPrincipal principal = new WindowsPrincipal(identity);
        return principal.IsInRole(WindowsBuiltInRole.Administrator);
    }

    // 重新以管理员权限启动
    static void RestartAsAdmin()
    {
        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            FileName = Process.GetCurrentProcess().MainModule.FileName,
            UseShellExecute = true,
            Verb = "runas" // 请求管理员权限
        };

        try
        {
            Process.Start(startInfo);
        }
        catch (Exception)
        {
            // 用户可能拒绝了UAC提示
        }
        Environment.Exit(0);
    }
}