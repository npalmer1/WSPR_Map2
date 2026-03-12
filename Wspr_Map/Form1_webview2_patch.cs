// ============================================================
// WSPR Map - WebView2 / Leaflet Migration Patch
// ============================================================
//
// PREREQUISITES
// -------------
// 1. NuGet: install  Microsoft.Web.WebView2
// 2. In the designer, DELETE the GMapControl (gmap)
// 3. Add a WebView2 control named  webView  covering the same area
//    (or add it in code as shown in InitWebView below)
// 4. Copy wspr_map.html into your project folder and set:
//      Properties → Copy to Output Directory = "Copy if newer"
//
// WHAT STAYS THE SAME
// -------------------
// • All MySQL query methods (find_selectedRX, find_selectedTX, etc.)
// • All structs (decoded_data, RX_data)
// • All filter / band / period logic
// • Database credential save/load
//
// WHAT CHANGES
// ------------
// • gmap.* calls → webView + JS calls via ExecuteScriptAsync
// • addMarker / addPath / addOwn → build a JSON payload, send once
// • GMapOverlay / GMapRoute fields removed
// ============================================================

using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using System.Text.Json;          // .NET 6+ built-in; or use Newtonsoft.Json

// ── Fields to ADD (remove the old GMapOverlay / GMapRoute fields) ────────────

WebView2 webView;                // if not added in designer, create in code
bool mapReady = false;           // true once WebView2 has finished loading

// Accumulate markers during a filter pass, then send as one JSON batch
private List<object> rxPoints = new List<object>();
private List<object> txPoints = new List<object>();


// ── Form1_Load  (replace your existing one) ──────────────────────────────────
private async void Form1_Load(object sender, EventArgs e)
{
    this.FormBorderStyle = FormBorderStyle.Sizable;
    this.MaximizeBox = true;
    this.MinimizeBox = true;

    string ver = "0.2.1";
    string header = "WSPR Scheduler Map 2   V." + ver + "   GNU GPLv3";
    Msg.TMessageBox("Initialising WSPR Scheduler Map", "WS Map", 25000);

    passtextBox.Text = pass;
    radioButton1.Checked = true;
    bandlistBox.SelectedIndex = 0;
    periodlistBox.SelectedIndex = 4;
    clutterlistBox.SelectedIndex = 0;
    pathcheckBox.Checked = true;

    // ── WebView2 setup ───────────────────────────────────────────────────────
    await InitWebView();

    int i = table_countRX();
    if (i > 0)
        await find_reportedRX(i);
    else
        MessageBox.Show("Database error or no data in database");

    this.Text = "Reports for station: " + call;
    if (!string.IsNullOrEmpty(locator))
        this.Text += "  at: " + locator;
    this.Text += "                  " + header;

    if (!getUserandPassword())
        passtextBox.Text = "";

    filter_results();
}


// ── InitWebView ───────────────────────────────────────────────────────────────
private async Task InitWebView()
{
    // If you added WebView2 in the designer you can skip the next 4 lines
    webView = new WebView2();
    webView.Dock = DockStyle.Fill;
    this.Controls.Add(webView);          // add to form (adjust if inside a panel)
    webView.BringToFront();

    await webView.EnsureCoreWebView2Async(null);

    // Allow local file access
    webView.CoreWebView2.SetVirtualHostNameToFolderMapping(
        "wspr.local", Application.StartupPath, CoreWebView2HostResourceAccessKind.Allow);

    webView.CoreWebView2.NavigationCompleted += (s, e) => { mapReady = true; };
    webView.Source = new Uri("https://wspr.local/wspr_map.html");

    // Wait for page to load (up to 10 sec)
    var timeout = DateTime.Now.AddSeconds(10);
    while (!mapReady && DateTime.Now < timeout)
        await Task.Delay(100);
}


// ── JS helper ─────────────────────────────────────────────────────────────────
private async Task RunJS(string script)
{
    if (!mapReady) return;
    try { await webView.ExecuteScriptAsync(script); }
    catch { /* swallow if page not ready */ }
}


// ── SendMapData  (replaces gmap.Overlays.Add calls) ──────────────────────────
private async Task SendMapData()
{
    var payload = new
    {
        home = new { lat = mylat, lon = mylon, call = call },
        rx = rxPoints,
        tx = txPoints,
        showPaths = pathcheckBox.Checked
    };

    string json = JsonSerializer.Serialize(payload);
    // Escape backticks so the template literal is safe
    json = json.Replace("\\", "\\\\").Replace("`", "\\`");
    await RunJS($"window.wsprMap.loadData(`{json}`)");
}


// ── addMarker  (REPLACE existing) ────────────────────────────────────────────
// Now just accumulates into rxPoints / txPoints; SendMapData flushes them.
// type: "rx" or "tx"  (called from find_selectedRX / find_selectedTX)
private void AccumulateMarker(double lat, double lon, string label, string type)
{
    var pt = new { lat, lon, label };
    if (type == "rx") rxPoints.Add(pt);
    else              txPoints.Add(pt);
}


// ── addOwn  (REPLACE existing) ────────────────────────────────────────────────
private async Task addOwn()
{
    LatLng latlong = MaidenheadLocator.LocatorToLatLng(locator);
    mylat = latlong.Lat;
    mylon = latlong.Long;
    // Home marker is sent as part of the JSON payload in SendMapData
}


// ── filter_results  (REPLACE existing) ───────────────────────────────────────
private async void filter_results()
{
    rxPoints.Clear();
    txPoints.Clear();

    DateTime dtNow = DateTime.Now.ToUniversalTime();
    DateTime dtPrev = dtNow;
    double p = findPeriod();

    // --- same period logic as before ---
    if      (p > 0 && p < 1) dtPrev = dtPrev.AddMinutes(-p * 60);
    else if (p >= 1 && p < 24) dtPrev = dtPrev.AddHours(-p);
    else if (p == 24)  dtPrev = dtPrev.AddDays(-1);
    else if (p == 48)  dtPrev = dtPrev.AddDays(-2);
    else if (p == 96)  dtPrev = dtPrev.AddDays(-4);
    else if (p == 168) dtPrev = dtPrev.AddDays(-7);
    else if (p == 240) dtPrev = dtPrev.AddDays(-10);
    else if (p == 336) dtPrev = dtPrev.AddDays(-14);
    else if (p == 504) dtPrev = dtPrev.AddDays(-21);
    else if (p == 672) dtPrev = dtPrev.AddDays(-28);
    else dtPrev = dtNow;

    string now  = dtNow.ToString("yyyy-MM-dd HH:mm:00");
    string prev = dtPrev.ToString("yyyy-MM-dd HH:mm:00");
    string mhz  = find_band();
    int band    = get_band(bandlistBox.SelectedIndex);

    await addOwn();   // sets mylat / mylon

    int rows = table_countRX();
    if (rows > 0 && (radioButton1.Checked || radioButton3.Checked))
        await find_selectedRX(prev, now, mhz, rows);

    rows = table_countTX();
    if (rows > 0 && (radioButton1.Checked || radioButton2.Checked))
        await find_selectedTX(prev, now, band, rows);

    await SendMapData();   // ← single JS call with everything
}


// ── find_selectedRX  (change only the marker/path lines) ─────────────────────
// Inside the while(Reader.Read()) loop, REPLACE:
//
//   await addMarker(txlat, txlon, pin, DX.tx_sign+bandS);
//   if (pathcheckBox.Checked) { await addPath(...); }
//
// WITH:
//   AccumulateMarker(txlat, txlon, DX.tx_sign + bandS, "rx");
//
// Everything else (DB query, filtering, LatLng conversion) stays identical.


// ── find_selectedTX  (change only the marker/path lines) ─────────────────────
// Inside the while(Reader.Read()) loop, REPLACE:
//
//   await addMarker(rxlat, rxlon, pin, RX.rx_sign+bandS);
//   if (pathcheckBox.Checked) { await addPath(...); }
//
// WITH:
//   AccumulateMarker(rxlat, rxlon, RX.rx_sign + bandS, "tx");
//
// Everything else stays identical.


// ── recentre  (REPLACE existing) ─────────────────────────────────────────────
private void recentre()
{
    _ = RunJS($"window.wsprMap.centreOn({mylat}, {mylon}, 2)");
}


// ── timer1_Tick  (REPLACE existing) ──────────────────────────────────────────
private async void timer1_Tick(object sender, EventArgs e)
{
    filter_results();
    recentre();
}


// ── pathcheckBox_CheckedChanged  (REPLACE existing) ──────────────────────────
private void pathcheckBox_CheckedChanged(object sender, EventArgs e)
{
    filter_results();   // just redraw; JS handles show/hide via showPaths flag
}


// ── REMOVE these - no longer needed ──────────────────────────────────────────
// GMapOverlay markers = new GMapOverlay("markers");
// GMapOverlay routes  = new GMapOverlay("routes");
// private async Task addMarker(...)   ← replaced by AccumulateMarker
// private async Task addPath(...)     ← handled in JS great-circle code
// gmap_OnMarkerClick                  ← Leaflet tooltips handle this natively
// gmap_MouseMove                      ← remove or repurpose Zoomlabel update
// gmap_Load                           ← remove
