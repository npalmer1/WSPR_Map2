
// removed: using GMap.NET;
// removed: using GMap.NET.WindowsForms;
// removed: using GMap.NET.WindowsForms.Markers;
using Maidenhead;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using MySql.Data.MySqlClient;
using Security;
using System;
using System.Data;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;          // .NET 6+ built-in; or use Newtonsoft.Json
using WSPR_Map;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;


namespace Wspr_Map
{
    public partial class Form1 : Form
    {
        public Form1()
        {

            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.MaximizeBox = true;
            this.MinimizeBox = true;
           
        }


        WebView2 webView;                // if not added in designer, create in code
        bool mapReady = false;           // true once WebView2 has finished loading
        
        TaskCompletionSource<bool> mapReadyTcs = new TaskCompletionSource<bool>(); 


        // Accumulate markers during a filter pass, then send as one JSON batch
        private List<object> rxPoints = new List<object>();
        private List<object> txPoints = new List<object>();

        //deduplicate before accumulating
        private HashSet<string> _rxSeen = new HashSet<string>();
        private HashSet<string> _txSeen = new HashSet<string>();

        MessageClass Msg = new MessageClass();

        public struct decoded_data
        {
            public DateTime datetime;
            public Int16 band;
            public string tx_sign;
            public string tx_loc;
            public double frequency;
            public Int16 power;
            public int snr;
            public Int16 drift;
            public int distance;
            public Int16 azimuth;
            public string reporter;
            public string reporter_loc;
            public float dt;
        }
        decoded_data DX = new decoded_data();


        public struct RX_data
        {
            public Int64 id;
            public DateTime time;
            public Int16 band;
            public string rx_sign;
            public float rx_lat;
            public float rx_lon;
            public string rx_loc;
            public string tx_sign;
            public float tx_lat;
            public float tx_lon;
            public string tx_loc;
            public int distance;
            public int azimuth;
            public int rx_azimuth;
            public int frequency;
            public int power;
            public int snr;
            public int drift;
            public string version;
            public int code;
        }
        RX_data RX = new RX_data();

        string header = "";

        string locator = "";
        string call = "";
        double mylat;
        double mylon;

        string server = "127.0.0.1";
        string user = "admin";
        string pass = "wspr";

        int dbRows = 300000;

        bool connectionError = false;

        bool gettingData = false;

        private Dictionary<string, (double lat, double lon)> _locatorCache  = new Dictionary<string, (double lat, double lon)>();


        private async void Form1_Load(object sender, EventArgs e)
        {
            webViewInstall();
            EnsureMapFiles();
           

            //loadinglabel.Text = "Initialising WSPR Scheduler Map... please wait";
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.MaximizeBox = true;
            this.MinimizeBox = true;

            string ver = "0.2.4";
            header = "WSPR Scheduler Map 2   V." + ver + "   GNU GPLv3             ";
            this.Text = header;
            string info = "...You must run WSPR Scheduler to display TX reports and WSPR Scheduler Live for RX reports";
            //Msg.TMessageBox("Initialising WSPR Scheduler Map ... please wait", "WS Map", 25000);
            bottomlabel.Text = info;
            passtextBox.Text = pass;
            radioButton1.Checked = true;
            bandlistBox.SelectedIndex = 0;
            periodlistBox.SelectedIndex = 5;
            clutterlistBox.SelectedIndex = 0;
            pathcheckBox.Checked = true;

            // ── WebView2 setup ───────────────────────────────────────────────────────
            gettingData = true;
            await InitWebView();
            EnsureIndexes(); //create indexes for database

            //webView.Dock = DockStyle.None;
            webView.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            rightPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            label1.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;




            int i = table_countRX();
           
            if (i > 0)
            {
                await find_reportedRX(5);
            }
            else
            {
                MessageBox.Show("Database error or no data in database");
            }
            
            /*int t = table_countTX();    //max rows in RTX table
            if (i > t)
            {
                dbRows = i;
            }
            else { dbRows = t; }*/

            if (string.IsNullOrEmpty(locator) || string.IsNullOrEmpty(call))
            {
                // Database failed or empty - update loading panel message
                loadinglabel.Text = "Unable to access database - press Apply to retry";
                loadinglabel.ForeColor = Color.Red;
                // Don't hide the panel - leave message visible
                return;  // exit Form1_Load early, map will be empty but visible
            }

            try
            {
                if (call != null || call != "")
                {                    
                    // Database loaded OK - continue normally         
                    this.Text += "Reports for station: " + call;
                    if (!string.IsNullOrEmpty(locator))
                        this.Text += "  at: " + locator;
                }
               
            }
            catch { }            

            if (!getUserandPassword())
                passtextBox.Text = "";

            await filter_results();

        }

        private void WebView_WebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            string msg = e.TryGetWebMessageAsString();

            if (msg == "MAP_READY")
            {
                mapReady = true;
                mapReadyTcs.TrySetResult(true);  
                Zoomlabel.Text = "Map detected";
                return;
            }

            if (msg.StartsWith("zoom:"))
            {
                Zoomlabel.Text = msg.Substring(5);
                return;
            }
        }


        private void webViewInstall()
        {
            // Check WebView2 is installed BEFORE anything else
            if (!IsWebView2Installed())
            {
                var result = MessageBox.Show(
                    "WebView2 Runtime is not installed on this PC.\n\n" +
                    "This is required for the map display.\n\n" +
                    "Click Yes to install it now,\n" +
                    "or No to exit and install manually from:\n" +
                    "https://developer.microsoft.com/microsoft-edge/webview2/",
                    "WSPRmap2 - WebView2 Required",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    string bundled = Path.Combine(Application.StartupPath,
                                                  "MicrosoftEdgeWebview2Setup.exe");
                    if (File.Exists(bundled))
                    {
                        System.Diagnostics.Process.Start(bundled);
                    }
                    else
                    {
                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                        {
                            FileName = "https://go.microsoft.com/fwlink/p/?LinkId=2124703",
                            UseShellExecute = true
                        });
                    }
                }
                Application.Exit();
                return;  // stop here - don't try to create WebView2
            }
        }

        private bool IsWebView2Installed()
        {
            // Check registry for WebView2 Runtime
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

        private void EnsureMapFiles()
        {
            string mapFolder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "WsprMap");
            Directory.CreateDirectory(mapFolder);

            // Copy each file from app folder if not already in WsprMap folder
            string[] files = { "wspr_map.html", "leaflet.js", "leaflet.css" };
            foreach (var file in files)
            {
                string dest = Path.Combine(mapFolder, file);
                string src = Path.Combine(Application.StartupPath, file);
                if (!File.Exists(dest) && File.Exists(src))
                    File.Copy(src, dest);
            }
        }






      
        private async Task InitWebView()
        {
            webView = new WebView2();
            webView.DefaultBackgroundColor = this.BackColor;
            webView.Location = new Point(-1, -15);
            webView.Size = new Size(1021, 754);
            webView.Anchor = AnchorStyles.Top | AnchorStyles.Bottom
                           | AnchorStyles.Left | AnchorStyles.Right;
            this.Controls.Add(webView);
            webView.BringToFront();

            try
            {
                string webViewUserData = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "WsprMap", "WebView2");
                Directory.CreateDirectory(webViewUserData);

                var env = await CoreWebView2Environment.CreateAsync(null, webViewUserData);
                await webView.EnsureCoreWebView2Async(env);
            }
            catch (Exception ex)
            {
                // WebView2 not installed - offer to install it
                var result = MessageBox.Show(
                    "WebView2 Runtime is not installed on this PC.\n\n" +
                    "This is required for the map display.\n\n" +
                    "Click Yes to install it now (requires internet connection),\n" +
                    "or No to exit and install manually from:\n" +
                    "https://developer.microsoft.com/microsoft-edge/webview2/",
                    "WSPRmap2 - WebView2 Required",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    // Try bundled installer first, then fall back to web download
                    string bundled = Path.Combine(Application.StartupPath, "MicrosoftEdgeWebview2Setup.exe");
                    if (File.Exists(bundled))
                    {
                        System.Diagnostics.Process.Start(bundled);
                    }
                    else
                    {
                        // Open download page in browser
                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                        {
                            FileName = "https://go.microsoft.com/fwlink/p/?LinkId=2124703",
                            UseShellExecute = true
                        });
                    }
                }
                Application.Exit();
                return;
            }

            // Enable messaging
            webView.CoreWebView2.Settings.IsWebMessageEnabled = true;

            // Attach handler BEFORE loading HTML
            webView.WebMessageReceived += WebView_WebMessageReceived;

            // Test message to confirm messaging works
            await webView.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync(@"
                chrome.webview.postMessage('TEST MESSAGE');
            ");

            await webView.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync(@"
                function waitForMap() {
                    if (window.map && typeof window.map.on === 'function') {
                        chrome.webview.postMessage('MAP_READY');
                        chrome.webview.postMessage('zoom:' + map.getZoom());
                        map.on('zoomend', function() {
                            chrome.webview.postMessage('zoom:' + map.getZoom());
                        });
                    } else {
                        setTimeout(waitForMap, 200);
                    }
                }
                waitForMap();
            ");

            // WebView2 initialised OK - continue
            string mapFolder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "WsprMap");
            Directory.CreateDirectory(mapFolder);

            webView.CoreWebView2.SetVirtualHostNameToFolderMapping(
                "wspr.local", mapFolder, CoreWebView2HostResourceAccessKind.Allow);

           // webView.CoreWebView2.NavigationCompleted += (s, e) => { mapReady = true; };
            webView.Source = new Uri("https://wspr.local/wspr_map.html");

            loadingPanel.Left = (this.ClientSize.Width - loadingPanel.Width) / 2;
            loadingPanel.Top = (this.ClientSize.Height - loadingPanel.Height) / 2;
            panel1.Left = (this.ClientSize.Width - loadingPanel.Width) / 2;
            panel1.Top = (this.ClientSize.Height - loadingPanel.Height) / 2;
            panel1.Visible = false;
            gettingData = false;
            loadingPanel.BringToFront();
            loadingPanel.Visible = true;

            /*while (!mapReady)
                await Task.Delay(100);
            await Task.Delay(100);*/
            //R waits for MAP_READY message from JS (Leaflet confirmed ready)
            await mapReadyTcs.Task;

        }


        private bool DatabaseAvailable()
        {
            bool ok = false;
            try
            {
                string connectionString = "server=" + server + ";user id=" + user +
                                          ";password=" + pass + ";database=wspr_rpt";
                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    connection.Close();
                    ok= true;
                }
            }
            catch
            {
                ok =false;
            }
            
            return ok;
        }

        private void EnsureIndexes()
        {
            try
            {
                string cs1 = "server=" + server + ";user id=" + user + ";password=" + pass + ";database=wspr_rpt";
                using (var con = new MySqlConnection(cs1))
                {
                    con.Open();
                    var cmds = new[]
                    {
                "CREATE INDEX IF NOT EXISTS idx_received_datetime  ON received (datetime)",
                "CREATE INDEX IF NOT EXISTS idx_received_frequency ON received (frequency)"
            };
                    foreach (var sql in cmds)
                    {
                        using (var cmd = new MySqlCommand(sql, con))
                            cmd.ExecuteNonQuery();
                    }
                }

                string cs2 = "server=" + server + ";user id=" + user + ";password=" + pass + ";database=wspr_rx";
                using (var con = new MySqlConnection(cs2))
                {
                    con.Open();
                    var cmds = new[]
                    {
                "CREATE INDEX IF NOT EXISTS idx_reported_time ON reported (time)",
                "CREATE INDEX IF NOT EXISTS idx_reported_band ON reported (band)"
            };
                    foreach (var cmd_str in cmds)
                    {
                        using (var cmd = new MySqlCommand(cmd_str, con))
                            cmd.ExecuteNonQuery();
                    }
                }
            }
            catch { }  // silently ignore - indexes are an optimisation, not critical
        }

        private void ShowDatabaseError(string message)
        {
            /*loadinglabel.Text = message;
            loadinglabel.ForeColor = Color.Red;
            loadingPanel.Visible = true;
            loadingPanel.BringToFront();*/
            Msg.TMessageBox(message, "WSPR Map", 3000);
        }


        private async Task RunJS(string script)
        {
            if (!mapReady) return;
            try { await webView.ExecuteScriptAsync(script); }
            catch { /* swallow if page not ready */ }
        }

        private async Task SendMapData()
        {
            // Serialise on background thread so UI stays responsive
            string json = await Task.Run(() => JsonSerializer.Serialize(new
            {
                home = new { lat = mylat, lon = mylon, call = call },
                rx = rxPoints,
                tx = txPoints,
                showPaths = pathcheckBox.Checked
            }));

            json = json.Replace("\\", "\\\\").Replace("`", "\\`");
            await RunJS($"window.wsprMap.loadData(`{json}`)");
            loadingPanel.Visible = false;
            panel1.Visible = false;
        }



        private void AccumulateMarker(double lat, double lon, string label, string type)
        {
            // Key on locator position, not call (same grid square = same dot)
            string key = $"{lat:F2},{lon:F2}";

            if (type == "rx")
            {
                if (_rxSeen.Add(key))  // Add returns false if already present
                    rxPoints.Add(new { lat, lon, label });
            }
            else
            {
                if (_txSeen.Add(key))
                    txPoints.Add(new { lat, lon, label });
            }
        }



        private async Task addOwn()
        {

            if (string.IsNullOrEmpty(locator)) return;  // don't plot 0,0
            LatLng latlong = MaidenheadLocator.LocatorToLatLng(locator);
            mylat = latlong.Lat;
            mylon = latlong.Long;
            // Home marker is sent as part of the JSON payload in SendMapData
        }

        private int table_countRX()
        {
            int count;
            string connectionString = "server=" + server + ";user id=" + user + ";password=" + pass + ";database=wspr_rpt";

            try
            {
                //string connectionString = "Server=server;Port=3306;Database=wspr;User ID=user;Password=pass;";

                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    using (var command = new MySqlCommand("SELECT COUNT(*) FROM received", connection))
                    {
                        count = Convert.ToInt32(command.ExecuteScalar());
                    }
                    connection.Close();
                }
                return count;

            }
            catch
            {
                return 0;
            }
        }
        private int table_countTX()
        {
            int count;
            string connectionString = "server=" + server + ";user id=" + user + ";password=" + pass + ";database=wspr_rx";

            try
            {
                //string connectionString = "Server=server;Port=3306;Database=wspr;User ID=user;Password=pass;";

                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    using (var command = new MySqlCommand("SELECT COUNT(*) FROM reported", connection))
                    {
                        count = Convert.ToInt32(command.ExecuteScalar());
                    }
                    connection.Close();
                }
                return count;

            }
            catch
            {
                return 0;
            }
        }

        private string find_band()
        {
            int s = bandlistBox.SelectedIndex;
            string b = "";
            switch (s)
            {
                case -1:
                    b = ""; //all
                    break;
                case 0:
                    b = ""; //all
                    break;
                case 1:
                    b = "0.136";
                    break;
                case 2:
                    b = "0.47";
                    break;
                case 3:
                    b = "1.8";
                    break;
                case 4:
                    b = "3.5";
                    break;
                case 5:
                    b = "5.";
                    break;
                case 6:
                    b = "7.";
                    break;
                case 7:
                    b = "10";
                    break;
                case 8:
                    b = "13";
                    break;
                case 9:
                    b = "14";
                    break;
                case 10:
                    b = "18";
                    break;
                case 11:
                    b = "21";
                    break;
                case 12:
                    b = "24";
                    break;
                case 13:
                    b = "28";
                    break;
                case 14:
                    b = "40";
                    break;
                case 15:
                    b = "50";
                    break;
                case 16:
                    b = "70";
                    break;
                case 17:
                    b = "144";
                    break;
                case 18:
                    b = "432";
                    break;
                case 19:
                    b = "1296";
                    break;
                default:
                    b = "";
                    break;
            }
            return b;
        }

        private int get_band(int bandno)
        {
            int b = -2; //all
            switch (bandno)
            {
                case 0:
                    b = -2; //all
                    break;
                case 1:
                    b = -1; //lf
                    break;
                case 2:
                    b = 0;  //mf
                    break;
                case 3:
                    b = 1;  //1.8
                    break;
                case 4:
                    b = 3;
                    break;
                case 5:
                    b = 5;
                    break;
                case 6:
                    b = 7;
                    break;
                case 7:
                    b = 10;
                    break;
                case 8:
                    b = 13;
                    break;
                case 9:
                    b = 14;
                    break;
                case 10:
                    b = 18;
                    break;
                case 11:
                    b = 21;
                    break;
                case 12:
                    b = 24;
                    break;
                case 13:
                    b = 28;
                    break;
                case 14:
                    b = 40;
                    break;
                case 15:
                    b = 50;
                    break;
                case 16:
                    b = 70;
                    break;
                case 17:
                    b = 144;
                    break;
                case 18:
                    b = 432;
                    break;
                case 19:
                    b = 1296;
                    break;
                default:
                    b = -2; //all
                    break;
            }
            return b;
        }
        private string get_reverse_band(int bandno)
        {
            string b = "all";
            switch (bandno)
            {
                case -1:
                    b = "LF"; //lf
                    break;
                case 0:
                    b = "MF";  //mf
                    break;
                case 1:
                    b = "160";  //1.8
                    break;
                case 3:
                    b = "80";
                    break;
                case 5:
                    b = "60";
                    break;
                case 7:
                    b = "40";
                    break;
                case 10:
                    b = "30";
                    break;
                case 13:
                    b = "22";
                    break;
                case 14:
                    b = "20";
                    break;
                case 18:
                    b = "17";
                    break;
                case 21:
                    b = "15";
                    break;
                case 24:
                    b = "12";
                    break;
                case 28:
                    b = "10";
                    break;
                case 40:
                    b = "8";
                    break;
                case 50:
                    b = "6";
                    break;
                case 70:
                    b = "4";
                    break;
                case 144:
                    b = "2";
                    break;
                case 432:
                    b = "70cm";
                    break;
                case 1296:
                    b = "23cm";
                    break;
                default:
                    b = "?";
                    break;
            }
            return b;
        }

        private double findPeriod() //find number of minutes/hours/days from selection
        {
            int s = periodlistBox.SelectedIndex;
            string t = "";
            switch (s)
            {
                case -1:
                    return 0;
                case 0:
                    return 0.1;
                case 1:
                    return 0.2;
                case 2:
                    return 0.5;
                case 3:
                    return 1;
                case 4:
                    return 2;
                case 5:
                    return 3;
                case 6:
                    return 6;
                case 7:
                    return 12;
                case 8:
                    return 24;
                case 9:
                    return 48;
                case 10:
                    return 96;
                case 11:
                    return 168;
                case 12:
                    return 240;
                case 13:
                    return 336;
                case 14:
                    return 504;
                case 15:
                    return 672;
                default:
                    return 0;
            }
        }


        private void filterbutton_Click(object sender, EventArgs e)
        {

            filter_button_action();

            //mForm.Dispose();
        }
        private async void filter_button_action()
        {
            if (gettingData)
            {
               
                Msg.TMessageBox("Please wait until current map updated", "Map updating", 2500);
                return;
            }
            int delay = 30000;
            //MessageForm mForm = new MessageForm();
            if (periodlistBox.SelectedIndex > 9)
            {
                delay = 50000;
            }
            //Msg.TMessageBox("Please wait....", "", delay);


            panel1.Visible = true;
            panel1.BringToFront();
            panel1.Refresh();
            gettingData = true;
            await filter_results();

            this.Text = header; // reset title before appending call/locator
            if (call != null || call != "")
            {

                this.Text += "Reports for station: " + call;
                if (!string.IsNullOrEmpty(locator))
                    this.Text += "  at: " + locator;
            }
        }

        private async void initial_map(int min)
        {
            DateTime end = DateTime.Now.ToUniversalTime();
            DateTime start = DateTime.Now.AddMinutes(-min);
            string to = end.ToString("yyyy-MM-dd HH:mm:00");
            string from = start.ToString("yyyy-MM-dd HH:mm:00");
            int rxrows = table_countRX();
            int txrows = table_countTX();
            int rows = dbRows;
            if (rxrows > 0 && txrows > 0 && !radioButton3.Checked && !radioButton2.Checked)
            {
                if (radioButton1.Checked)
                {
                    await Task.WhenAll(
                        find_selectedRX(from, to, "", rows),
                        find_selectedTX(from, to, -2, rows)
                    );
                }
            }
            else if (rxrows >0 && radioButton3.Checked) 
            {
                    await find_selectedRX(from, to, "", rows);
            }
            else if (txrows >0 && radioButton2.Checked)
            {
                await find_selectedTX(from, to, -2, rows); //-2 means all bands
            }

         
        }

        private async Task filter_results()
        {
            if (!DatabaseAvailable())
            {
                ShowDatabaseError("Database unavailable - press Apply to retry");
                return;
            }
            // If locator not yet found (DB was down on startup), try again now
            if (string.IsNullOrEmpty(locator) || string.IsNullOrEmpty(call))
            {
                int i = table_countRX();
                if (i > 0)
                {
                    await find_reportedRX(5);
                    if (!string.IsNullOrEmpty(locator))
                    {
                        // Successfully connected - hide the error panel
                        //loadingPanel.Visible = false;
                        this.Text = header; // reset title before appending call/locator
                        if (call != null || call != "")
                        {
                            this.Text += "Reports for station: " + call;
                          
                                this.Text += "  at: " + locator;
                        }
                    }
                }
            }

            // If still no locator, nothing to plot
            if (string.IsNullOrEmpty(locator))
            {
                MessageBox.Show("Cannot load map - locator not found in database.", "WSPR Map");
                return;
            }

            rxPoints.Clear();
            txPoints.Clear();
            _rxSeen.Clear();   
            _txSeen.Clear();  

            DateTime dtNow = DateTime.Now.ToUniversalTime();
            DateTime dtPrev = dtNow;
            double p = findPeriod();

            // --- same period logic as before ---
            if (p > 0 && p < 1) dtPrev = dtPrev.AddMinutes(-p * 60);
            else if (p >= 1 && p < 24) dtPrev = dtPrev.AddHours(-p);
            else if (p == 24) dtPrev = dtPrev.AddDays(-1);
            else if (p == 48) dtPrev = dtPrev.AddDays(-2);
            else if (p == 96) dtPrev = dtPrev.AddDays(-4);
            else if (p == 168) dtPrev = dtPrev.AddDays(-7);
            else if (p == 240) dtPrev = dtPrev.AddDays(-10);
            else if (p == 336) dtPrev = dtPrev.AddDays(-14);
            else if (p == 504) dtPrev = dtPrev.AddDays(-21);
            else if (p == 672) dtPrev = dtPrev.AddDays(-28);
            else dtPrev = dtNow;

            string now = dtNow.ToString("yyyy-MM-dd HH:mm:00");
            string prev = dtPrev.ToString("yyyy-MM-dd HH:mm:00");
            string mhz = find_band();
            int band = get_band(bandlistBox.SelectedIndex);

            await addOwn();   // sets mylat / mylon

            int rxrows = table_countRX();
            int txrows = table_countTX();
            int rows = dbRows;
            if (rxrows > 0 && txrows > 0 && !radioButton3.Checked && !radioButton2.Checked)
            {
                if (radioButton1.Checked)
                {
                    await Task.WhenAll(
                        find_selectedRX(prev, now, mhz, rows),
                        find_selectedTX(prev, now, band, rows)
                    );
                }
            }
            else if (rxrows > 0 && radioButton3.Checked)
            {
                await find_selectedRX(prev, now, mhz, rows);
            }
            else if (txrows > 0 && radioButton2.Checked)
            {
                await find_selectedTX(prev, now, band, rows); //-2 means all bands
            }           

            await SendMapData();   // ← single JS call with everything
            gettingData = false;
        }


        private async Task find_selectedRX(string datetime1, string datetime2, string mhz, int tablecount) //find a slot row for display in grid from the database corresponding to the date/time from the slot
        {   //received by own station of other station txns

            //gmap.Zoom = 3;
            double txlat = 0;
            double txlon = 0;

            DataTable Slots = new DataTable();
            //DateTime d = new DateTime();
            int i = 0;
            bool found = false;
            string myConnectionString = "server=" + server + ";user id=" + user + ";password=" + pass + ";database=wspr_rpt";

            //int maxrows = 1000; //max rows to return
            string and = "";
            string bandstr = "";
            string q = "";
            string distStr = "";
            if (mhz != "")
            {
                bandstr = " AND frequency LIKE '" + mhz + "%' ";
                
            }
            else
            {
                bandstr = "";
            }


                string fromstr = "0";
            double mls = 0.0;
            if (clutterlistBox.SelectedIndex > 0)
            {
                fromstr = clutterlistBox.SelectedItem.ToString();
                mls = Convert.ToDouble(fromstr);
                if (!kmcheckBox.Checked)    //miles
                {
                    double km = mls * 1.60934;
                    fromstr = km.ToString("F1");
                }
            }
            if (mls > 0)
            {
                distStr = "AND distance >= '" + fromstr + "'";
            }
            else
            {
                distStr = "";
            }


            try
            {
                MySqlConnection connection = new MySqlConnection(myConnectionString);

                connection.Open();

                MySqlCommand command = connection.CreateCommand();



                command.CommandText = "SELECT tx_sign, tx_loc, band, frequency, distance FROM received WHERE datetime >= '" + datetime1 + "' AND datetime <= '" + datetime2 + "'" + bandstr + distStr+ " ORDER BY datetime DESC LIMIT "+tablecount;
                /*command.CommandText = @"SELECT * FROM received WHERE datetime >= @from AND datetime <= @to AND (@mhz = '' OR frequency LIKE @mhzLike) AND distance >= @dist ORDER BY datetime DESC LIMIT " +tablecount;

                command.Parameters.AddWithValue("@from", datetime1);
                command.Parameters.AddWithValue("@to", datetime2);
                command.Parameters.AddWithValue("@mhz", mhz);
                command.Parameters.AddWithValue("@mhzLike", mhz + "%");
                command.Parameters.AddWithValue("@dist", mls);*/
                // Only fetch the columns you actually use
                //command.CommandText = @"SELECT tx_sign, tx_loc, band, frequency, distance FROM received WHERE datetime >= @from AND datetime <= @to " + bandstr + distStr;
                //command.CommandText += @" ORDER BY datetime DESC LIMIT " +tablecount;

                //command.Parameters.AddWithValue("@from", datetime1);
                //command.Parameters.AddWithValue("@to", datetime2);



                MySqlDataReader Reader;
                Reader = command.ExecuteReader();
                string bandS = "";
                string txloc = "";
                int index = bandlistBox.SelectedIndex;
                while (Reader.Read())
                {
                    found = true;

                    if (i < tablecount)   //only show first maxrows rows, or to length of reported table
                    {

                        //DX.datetime = (DateTime)Reader["datetime"];
                        DX.band = (Int16)Reader["band"];

                        DX.tx_sign = (string)Reader["tx_sign"];
                        DX.tx_loc = (string)Reader["tx_loc"];
                        txloc = DX.tx_loc;
                        DX.frequency = (double)Reader["frequency"];
                        //DX.power = (Int16)Reader["power"];
                        //DX.snr = (int)Reader["snr"];
                        //DX.drift = (Int16)Reader["drift"];
                        DX.distance = (int)Reader["distance"];
                        //DX.azimuth = (Int16)Reader["azimuth"];
                        //DX.reporter = (string)Reader["reporter"];
                       // DX.reporter_loc = (string)Reader["reporter_loc"];
                        //DX.dt = (float)Reader["dt"];


                        // pin removed - using Leaflet markers
                        bool special = false;
                        if (QcheckBox.Checked && specialCall(DX.tx_sign))
                        {
                            special = true;
                        }
                        
                        if (DX.tx_sign != "nil rcvd" && !special)
                        {
                            //LatLng latlong = MaidenheadLocator.LocatorToLatLng(DX.tx_loc);
                            if (!_locatorCache.TryGetValue(DX.tx_loc, out var pos))
                            {
                               var  latlong = MaidenheadLocator.LocatorToLatLng(DX.tx_loc);
                                pos = (latlong.Lat, latlong.Long);
                                _locatorCache[DX.tx_loc] = pos;
                            }
                           

                            //txlat = latlong.Lat;
                            //txlon = latlong.Long;
                            bandS = "";
                            if (index == 0) //if all bands selected
                            {
                                bandS = get_reverse_band(DX.band);
                                bandS = " (" + bandS + ")";
                            }
                            if (txloc != "")
                            {
                                AccumulateMarker(pos.lat, pos.lon, DX.tx_sign + bandS, "rx");
                            }
                            //AccumulateMarker(txlat, txlon, DX.tx_sign + bandS, "rx");
                        }
                        i++;
                    }
                    else
                    {
                        break;
                    }


                }
                Reader.Close();
                connection.Close();
                //gmap.Zoom = 2;

            }
            catch
            {
                //found = false;

            }

        }

        private bool specialCall(string call)
        {

            if (call.StartsWith("0"))
            {
                return true;
            }
            if (call.StartsWith("Q"))
            {
                return true;
            }

            if (call.StartsWith("1"))
            {
                return true;
            }
            if (call.Length > 1)
            {
                if (char.IsDigit(call[0]) && char.IsDigit(call[1]))
                {
                    return true;
                }
            }

            return false;

        }


        private async Task<bool> find_reportedRX(int tablecount) //find a slot row for display in grid from the database corresponding to the date/time from the slot
        {   //find own call and locator from database

            DataTable Slots = new DataTable();
            //DateTime d = new DateTime();
            int i = 0;
            bool found = false;
            string myConnectionString = "server=" + server + ";user id=" + user + ";password=" + pass + ";database=wspr_rpt";


            try
            {
                MySqlConnection connection = new MySqlConnection(myConnectionString);

                connection.Open();

                MySqlCommand command = connection.CreateCommand();


                command.CommandText = "SELECT reporter, reporter_loc FROM received ORDER BY datetime DESC LIMIT "+tablecount;
                MySqlDataReader Reader;
                Reader = command.ExecuteReader();

                while (Reader.Read() && !found)
                {

                    if (i < tablecount)    //only show first maxrows rows, or to length of reported table
                    {

                        if (locator == "")
                        {
                            locator = (string)Reader["reporter_loc"];
                        }
                        if (call == "")
                        {
                            call = (string)Reader["reporter"];
                        }

                        if (locator != "" && call != "")
                        {
                            found = true;
                        }

                    }
                    else
                    {
                        break;
                    }
                    i++;

                }
                Reader.Close();
                connection.Close();

            }
            catch
            {
                //databaseError = true; //stop wasting time trying to connect if database error - ignore for present
                found = false;
                MessageBox.Show("Error connecting to database");
            }
            return found;
        }


        private async Task find_selectedTX(string time1, string time2, int band, int tablecount) //find a slot row for display in grid from the database corresponding to the date/time from the slot
        { //reports from other stations about own txns
            //gmap.Zoom = 3;
            double rxlat = 0;
            double rxlon = 0;
            int maxrows = 1000;
            DataTable Slots = new DataTable();
            //DateTime d = new DateTime();
            int i = 0;
            bool found = false;
            string myConnectionString = "server=" + server + ";user id=" + user + ";password=" + pass + ";database=wspr_rx";

            string bandstr = "";
            string q = "";
            string distStr = "";
            string rxloc = "";
            if (band == -2) //all bands
            {
                bandstr = "";
               
            }
            else
            {
                bandstr = "AND band = '" + band.ToString() + "'";               
            }
            string fromstr = "0";
            double mls = 0.0;
            if (clutterlistBox.SelectedIndex > 0)
            {
                fromstr = clutterlistBox.SelectedItem.ToString();
                mls = Convert.ToDouble(fromstr);
                if (!kmcheckBox.Checked)    //miles
                {
                    double km = mls * 1.60934;                   
                    fromstr = km.ToString("F1");
                }                
            }
            if (mls > 0)
            {
                distStr = "AND distance >= '" + fromstr + "'";
            }
            else
            {
                distStr = "";
            }


            try
            {
                MySqlConnection connection = new MySqlConnection(myConnectionString);

                connection.Open();

                MySqlCommand command = connection.CreateCommand();


                command.CommandText = "SELECT rx_sign, rx_loc, band, frequency, distance FROM reported WHERE time >= '" + time1 + "' AND time <= '" + time2 + "' " + bandstr + " " + distStr + " ORDER BY time DESC LIMIT " + tablecount;
                // Instead of COUNT(*) + loop guard, just limit in SQL:
                //command.CommandText = "SELECT * FROM received WHERE datetime >= @from AND datetime <= @to"
                //    + bandstr + distStr
                //   + " ORDER BY datetime DESC LIMIT 1000";

                /*command.CommandText = @"SELECT rx_sign, rx_loc, band, frequency, distance FROM reported WHERE time >= @time1 AND time <= @time2 " + bandstr + distStr;
                command.CommandText += @" ORDER BY datetime DESC LIMIT "+tablecount;

                command.Parameters.AddWithValue("@from", time1);
                command.Parameters.AddWithValue("@to", time2);*/

                MySqlDataReader Reader;
                Reader = command.ExecuteReader();
                string bandS = "";
                int index = bandlistBox.SelectedIndex;
                while (Reader.Read())
                {
                    found = true;

                    if (i < tablecount)   //only show first maxrows rows, or to length of reported table
                    {

                        //RX.time = (DateTime)Reader["time"];
                        RX.band = (Int16)Reader["band"];
                        RX.rx_sign = (string)Reader["rx_sign"];
                        RX.rx_loc = (string)Reader["rx_loc"];
                        rxloc = RX.rx_loc;
                        //RX.tx_sign = (string)Reader["tx_sign"];
                        //RX.tx_loc = (string)Reader["tx_loc"];
                        RX.distance = (int)Reader["distance"];
                        //RX.azimuth = (int)Reader["azimuth"];
                        RX.frequency = (int)Reader["frequency"];
                        //RX.power = (Int16)Reader["power"];
                        //RX.snr = (Int16)Reader["snr"];
                        //RX.drift = (Int16)Reader["drift"];
                        //RX.version = (string)Reader["version"];


                        if (!_locatorCache.TryGetValue(RX.rx_loc, out var pos))
                        {
                            var latlong = MaidenheadLocator.LocatorToLatLng(RX.rx_loc);
                            pos = (latlong.Lat, latlong.Long);
                            _locatorCache[RX.rx_loc] = pos;
                        }


                        // pin removed - using Leaflet markers
                        bandS = "";
                        if (index == 0) //all bands 
                        {
                            bandS = get_reverse_band(RX.band);
                            bandS = " (" + bandS + ")";
                        }
                        if (rxloc != "")
                        {
                            AccumulateMarker(pos.lat, pos.lon, RX.rx_sign + bandS, "tx");
                        }
                        

                        i++;
                    }
                    else
                    {
                        break;
                    }

                }
                Reader.Close();
                connection.Close();


            }
            catch
            {

                found = false;

            }
        }

        private void configbutton_Click(object sender, EventArgs e)
        {


            locatortextBox.Text = locator;
            calltextBox.Text = call;
            groupBox1.BringToFront();
            groupBox1.Visible = true;

        }

        private async void pathcheckBox_CheckedChanged(object sender, EventArgs e)
        {
            await filter_results();   // just redraw; JS handles show/hide via showPaths flag
        }

        private void gmap_MouseClick(object sender, MouseEventArgs e)
        {
            pinlabel.Text = "";
            //Zoomlabel.Text = gmap.Zoom.ToString("F1");
        }

        private void savebutton_Click(object sender, EventArgs e)
        {
            groupBox1.Visible = false;
            saveUserandPassword("receiver", passtextBox.Text.Trim());
        }

        private void cancelbutton_Click(object sender, EventArgs e)
        {
            groupBox1.Visible = false;
        }

        private void kmcheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (kmcheckBox.Checked)
            {
                mlslabel.Text = "km";
                clutterlistBox.Items.Clear();
                clutterlistBox.Items.Add("0");
                clutterlistBox.Items.Add("80");
                clutterlistBox.Items.Add("160");
                clutterlistBox.Items.Add("320");
                clutterlistBox.Items.Add("640");
                clutterlistBox.Items.Add("960");
                clutterlistBox.Items.Add("1300");
                clutterlistBox.Items.Add("1600");
                clutterlistBox.Items.Add("1900");
                clutterlistBox.Items.Add("2400");
                clutterlistBox.Items.Add("2900");

                clutterlistBox.SelectedIndex = 0; //default to 0km

            }
            else
            {
                clutterlistBox.Items.Clear();
                clutterlistBox.Items.Add("0");
                clutterlistBox.Items.Add("50");
                clutterlistBox.Items.Add("100");
                clutterlistBox.Items.Add("200");
                clutterlistBox.Items.Add("400");
                clutterlistBox.Items.Add("600");
                clutterlistBox.Items.Add("800");
                clutterlistBox.Items.Add("1000");
                clutterlistBox.Items.Add("1200");
                clutterlistBox.Items.Add("1500");
                clutterlistBox.Items.Add("1800");
                mlslabel.Text = "mls";
                clutterlistBox.SelectedIndex = 0; //default to 0mls
            }
        }



        private void recentrebutton_Click(object sender, EventArgs e)
        {
            recentre();
        }
        private void recentre()
        {
            _ = RunJS($"window.wsprMap.centreOn(20, {mylon}, 2)");
        }

       
        private void showcheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (showcheckBox.Checked)
            {
                passtextBox.PasswordChar = '\0'; //show password
            }
            else
            {
                passtextBox.PasswordChar = '*'; //hide password
            }
        }

        private bool saveUserandPassword(string user, string password)
        {
            string key = "wsproundtheworld";
            Encryption enc = new Encryption();
            string encryptedpassword = enc.Encrypt(password, key);

            string homeDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string filepath = homeDirectory;
            string content = "db_user: " + user + " db_pass: " + encryptedpassword;
            if (Path.Exists(filepath))
            {
                string slash = "\\";
                if (filepath.EndsWith("\\"))
                {
                    slash = "";
                }
                filepath = filepath + slash + "DBmapcredential";
                try
                {
                    using (StreamWriter writer = new StreamWriter(filepath, false))
                    {
                        writer.WriteLine(content);
                        writer.Close();
                    }
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
            return true;
        }

        private bool getUserandPassword()
        {
            string key = "wsproundtheworld";
            Encryption enc = new Encryption();
            string encryptedpassword;
            string content = "";
            bool ok = false;

            string homeDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string filepath = homeDirectory;
            //string content = "db_user: " + user + " db_pass: " + passwordhash;

            if (Path.Exists(filepath))
            {
                string slash = "\\";
                if (filepath.EndsWith("\\"))
                {
                    slash = "";
                }
                filepath = filepath + slash + "DBmapcredential";
                if (File.Exists(filepath))
                {
                    try
                    {
                        using (StreamReader reader = new StreamReader(filepath))
                        {
                            content = reader.ReadLine();
                            reader.Close();
                        }
                        if (content != null || content != "")
                        {
                            if (content.Contains("db_pass:"))
                            {
                                encryptedpassword = content.Substring(content.IndexOf("db_pass: ") + "db_pass: ".Length);
                                string password = enc.Decrypt(encryptedpassword, key);
                                if (password.Length > 0 && password != null)
                                {
                                    pass = password;
                                    passtextBox.Text = password;

                                    ok = true;
                                }
                            }
                        }

                        if (!ok)
                        {
                            MessageBox.Show("Unable to read database credentials", "");
                            return false;
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Unable to read database credentials", "");
                        return false;
                    }
                }
            }


            return ok;
        }

        private void autocheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (autocheckBox.Checked)
            {
                periodlistBox.SelectedIndex = 5;  //180 mins default
                //bandlistBox.SelectedIndex = 0;
                //clutterlistBox.SelectedIndex = 1;
                pathcheckBox.Checked = true;
                timer1.Enabled = true;
                timer1.Start();
            }
            else
            {
                timer1.Enabled = false;
                timer1.Stop();
            }
        }

        private async void timer1_Tick(object sender, EventArgs e)
        {
            if (!autocheckBox.Checked || gettingData)
            {
                return;
            }
            panel1.Visible = true;
            panel1.BringToFront();
            panel1.Refresh();
            await filter_results();
            recentre();
            if (call != null || call != "")
            {
                this.Text = header;
            }
            // Database loaded OK - continue normally         
            this.Text = "Reports for station: " + call;
            if (!string.IsNullOrEmpty(locator))
                this.Text += "  at: " + locator;
            this.Text += "                  " + header;           
        }

        private void europeButton_Click(object sender, EventArgs e)
        {
            _ = RunJS("window.wsprMap.centreOn(50, 10, 3)");
        }

        private void americasButton_Click(object sender, EventArgs e)
        {
            _ = RunJS("window.wsprMap.centreOn(40, -90, 3)");
        }

        private void pacificButton_Click(object sender, EventArgs e)
        {
            _ = RunJS("window.wsprMap.centreOn(20, 150, 3)");
        }
    }
}