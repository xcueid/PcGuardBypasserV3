using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms; // Windows Forms�� ����ϱ� ���� �߰�

namespace PcGuardBypasser
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        [DllImport("kernel32.dll")]
        static extern IntPtr OpenThread(ThreadAccess dwDesiredAccess, bool bInheritHandle, uint dwThreadId);

        [DllImport("kernel32.dll")]
        static extern uint SuspendThread(IntPtr hThread);

        [DllImport("kernel32.dll")]
        static extern int ResumeThread(IntPtr hThread);

        [Flags]
        public enum ThreadAccess : int
        {
            SUSPEND_RESUME = 0x0002
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.AppendText("Made By DevaAndSage" + Environment.NewLine);
            string[] processNamesOrIds = { "iAgent", "iAgent32", "iService", "iWatcher" }; // ���μ��� �̸� �Ǵ� ID ���

            try
            {
                foreach (string processNameOrId in processNamesOrIds)
                {
                    // ���μ����� ã���ϴ�.
                    Process targetProcess = GetProcessByNameOrId(processNameOrId);

                    if (targetProcess != null)
                    {
                        // ���μ����� ��� �����带 �Ͻ� �ߴ��մϴ�.
                        foreach (ProcessThread thread in targetProcess.Threads)
                        {
                            IntPtr threadHandle = OpenThread(ThreadAccess.SUSPEND_RESUME, false, (uint)thread.Id);
                            SuspendThread(threadHandle);
                            CloseHandle(threadHandle);
                        }

                        textBox1.AppendText($"���μ��� {processNameOrId}�� ����ȭ ���׽��ϴ�." + Environment.NewLine);
                    }
                    else
                    {
                        textBox1.AppendText($"���μ��� {processNameOrId}�� ã�� �� �����ϴ�." + Environment.NewLine);
                    }
                }
            }
            catch (Exception ex)
            {
                textBox1.AppendText($"����: {ex.Message}" + Environment.NewLine);
            }
            textBox1.AppendText("PC���� ��ȸ�� �Ϸ�Ǿ����ϴ�." + Environment.NewLine);

        }

        static Process GetProcessByNameOrId(string nameOrId)
        {
            int processId;
            if (int.TryParse(nameOrId, out processId))
            {
                try
                {
                    return Process.GetProcessById(processId);
                }
                catch (ArgumentException)
                {
                    // ������ ID�� �ش��ϴ� ���μ����� ����
                    return null;
                }
            }
            else
            {
                Process[] processes = Process.GetProcessesByName(nameOrId);
                return processes.Length > 0 ? processes[0] : null;
            }
        }

        // ������ �ڵ��� �ݴ� �Լ�
        [DllImport("kernel32.dll")]
        static extern bool CloseHandle(IntPtr hObject);

        private void button2_Click(object sender, EventArgs e)
        {
            // ���� ���α׷��� ����� ���� ���� ��θ� �����մϴ�.
            string executablePath = Application.ExecutablePath;

            // ���� ���α׷� ������Ʈ�� Ű�� ���ϴ�.
            RegistryKey startupKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            try
            {
                // ���� ���α׷� ������Ʈ���� ����մϴ�.
                startupKey.SetValue("MyApp", executablePath);
                MessageBox.Show("���α׷��� ���� ���α׷��� ��ϵǾ����ϴ�.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("���� ���α׷� ��Ͽ� �����߽��ϴ�. ���� �޽���: " + ex.Message);
            }
            finally
            {
                // ������Ʈ�� Ű�� �ݽ��ϴ�.
                startupKey.Close();
            }
        }
    }
}
