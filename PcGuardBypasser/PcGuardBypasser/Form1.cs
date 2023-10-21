using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms; // Windows Forms를 사용하기 위해 추가

namespace PcGuardBypasser
{
    public partial class Form1 : Form
    {
        private void button1_Click(object sender, EventArgs e)
        {
            
            textBox1.AppendText("닥쳐 스크립트 키디." + Environment.NewLine);

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
                    // 지정된 ID에 해당하는 프로세스가 없음
                    return null;
                }
            }
            else
            {
                Process[] processes = Process.GetProcessesByName(nameOrId);
                return processes.Length > 0 ? processes[0] : null;
            }
        }

        // 스레드 핸들을 닫는 함수
        [DllImport("kernel32.dll")]
        static extern bool CloseHandle(IntPtr hObject);

        private void button2_Click(object sender, EventArgs e)
        {
            // 시작 프로그램에 등록할 실행 파일 경로를 설정합니다.
            string executablePath = Application.ExecutablePath;

            // 시작 프로그램 레지스트리 키를 엽니다.
            RegistryKey startupKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            try
            {
                // 시작 프로그램 레지스트리에 등록합니다.
                startupKey.SetValue("MyApp", executablePath);
                MessageBox.Show("프로그램이 시작 프로그램에 등록되었습니다.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("시작 프로그램 등록에 실패했습니다. 오류 메시지: " + ex.Message);
            }
            finally
            {
                // 레지스트리 키를 닫습니다.
                startupKey.Close();
            }
        }
    }
}
