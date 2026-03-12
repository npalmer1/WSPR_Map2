/*namespace Wspr_Map
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new Form1());
        }
    }
}*/
using System.Runtime.CompilerServices;

namespace Wspr_Map
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            if (!IsWebView2Installed())
            {
                InstallWebView2();
                return;
            }
            RunApp();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        static void RunApp()
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new Form1());
        }

        static bool IsWebView2Installed()
        {
            string[] keys = new string[]
            {
                @"SOFTWARE\Microsoft\EdgeUpdate\Clients\{F3017226-FE2A-4295-8BDF-00C3A9A7E4C5}",
                @"SOFTWARE\WOW6432Node\Microsoft\EdgeUpdate\Clients\{F3017226-FE2A-4295-8BDF-00C3A9A7E4C5}"
            };
            foreach (var key in keys)
            {
                using (var reg = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(key) ??
                                 Microsoft.Win32.Registry.CurrentUser.OpenSubKey(key))
                {
                    if (reg != null) return true;
                }
            }
            return false;
        }

        static void InstallWebView2()
        {
            string installer = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "MicrosoftEdgeWebview2Setup.exe");

            if (!File.Exists(installer))
            {
                MessageBox.Show(
                    "WebView2 installer not found in application folder.\n" +
                    "Please download from:\nhttps://developer.microsoft.com/microsoft-edge/webview2/",
                    "WSPRmap2", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var result = MessageBox.Show(
                "WebView2 Runtime is required but not installed.\n\n" +
                "Click Yes to install it now.",
                "WSPRmap2 - WebView2 Required",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                var proc = System.Diagnostics.Process.Start(
                    new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = installer,
                        UseShellExecute = true,
                        Verb = "runas"
                    });
                proc?.WaitForExit();

                // Restart the app after install
                System.Diagnostics.Process.Start(
                    new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = Path.Combine(
                            AppDomain.CurrentDomain.BaseDirectory, "Wspr_Map2.exe"),
                        UseShellExecute = true
                    });
            }
        }
    }
}