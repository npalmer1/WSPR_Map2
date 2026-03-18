namespace Wspr_Map
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            pinlabel = new Label();
            periodlistBox = new ListBox();
            bandlistBox = new ListBox();
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            filterbutton = new Button();
            configbutton = new Button();
            pathcheckBox = new CheckBox();
            radioButton1 = new RadioButton();
            radioButton2 = new RadioButton();
            radioButton3 = new RadioButton();
            label8 = new Label();
            clutterlistBox = new ListBox();
            mlslabel = new Label();
            kmcheckBox = new CheckBox();
            label9 = new Label();
            Zoomlabel = new Label();
            recentrebutton = new Button();
            autocheckBox = new CheckBox();
            timer1 = new System.Windows.Forms.Timer(components);
            showcheckBox = new CheckBox();
            cancelbutton = new Button();
            savebutton = new Button();
            label1 = new Label();
            locatortextBox = new TextBox();
            label5 = new Label();
            calltextBox = new TextBox();
            passtextBox = new TextBox();
            label6 = new Label();
            label7 = new Label();
            groupBox1 = new GroupBox();
            QcheckBox = new CheckBox();
            loadinglabel = new Label();
            loadingPanel = new Panel();
            europeButton = new Button();
            americasButton = new Button();
            pacificButton = new Button();
            label10 = new Label();
            bottomlabel = new Label();
            rightPanel = new Panel();
            panel1 = new Panel();
            Waitlabel = new Label();
            groupBox1.SuspendLayout();
            loadingPanel.SuspendLayout();
            rightPanel.SuspendLayout();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // pinlabel
            // 
            pinlabel.AutoSize = true;
            pinlabel.Location = new Point(154, 723);
            pinlabel.Margin = new Padding(4, 0, 4, 0);
            pinlabel.Name = "pinlabel";
            pinlabel.Size = new Size(22, 15);
            pinlabel.TabIndex = 1;
            pinlabel.Text = "---";
            // 
            // periodlistBox
            // 
            periodlistBox.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            periodlistBox.FormattingEnabled = true;
            periodlistBox.Items.AddRange(new object[] { "10 mins", "20 mins", "30 mins", "1 hour", "2 hours", "3 hours", "6 hours", "12 hours", "24 hours", "2 days", "4 days", "7 days", "10 days", "14 days", "21 days", "28 days" });
            periodlistBox.Location = new Point(17, 59);
            periodlistBox.Margin = new Padding(4, 3, 4, 3);
            periodlistBox.Name = "periodlistBox";
            periodlistBox.Size = new Size(67, 199);
            periodlistBox.TabIndex = 2;
            // 
            // bandlistBox
            // 
            bandlistBox.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            bandlistBox.FormattingEnabled = true;
            bandlistBox.Items.AddRange(new object[] { "All", "LF", "MF", "160m", "80m", "60m", "40m", "30m", "22m", "20m", "17m", "15m", "12m", "10m", "8m", "6m", "4m", "2m", "70cm", "23cm" });
            bandlistBox.Location = new Point(93, 59);
            bandlistBox.Margin = new Padding(4, 3, 4, 3);
            bandlistBox.Name = "bandlistBox";
            bandlistBox.Size = new Size(53, 251);
            bandlistBox.TabIndex = 3;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label2.Location = new Point(17, 43);
            label2.Margin = new Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new Size(40, 13);
            label2.TabIndex = 4;
            label2.Text = "Period";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label3.Location = new Point(89, 43);
            label3.Margin = new Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.Size = new Size(33, 13);
            label3.TabIndex = 5;
            label3.Text = "Band";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(83, 462);
            label4.Margin = new Padding(4, 0, 4, 0);
            label4.Name = "label4";
            label4.Size = new Size(75, 45);
            label4.TabIndex = 6;
            label4.Text = "Red - my TX\r\nBlue - my RX\r\nYellow - QTH";
            // 
            // filterbutton
            // 
            filterbutton.Location = new Point(50, 10);
            filterbutton.Margin = new Padding(4, 3, 4, 3);
            filterbutton.Name = "filterbutton";
            filterbutton.Size = new Size(52, 23);
            filterbutton.TabIndex = 7;
            filterbutton.Text = "Apply";
            filterbutton.UseVisualStyleBackColor = true;
            filterbutton.Click += filterbutton_Click;
            // 
            // configbutton
            // 
            configbutton.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            configbutton.Location = new Point(84, 575);
            configbutton.Margin = new Padding(0);
            configbutton.Name = "configbutton";
            configbutton.Size = new Size(58, 23);
            configbutton.TabIndex = 8;
            configbutton.Text = "Config";
            configbutton.UseVisualStyleBackColor = true;
            configbutton.Click += configbutton_Click;
            // 
            // pathcheckBox
            // 
            pathcheckBox.AutoSize = true;
            pathcheckBox.Checked = true;
            pathcheckBox.CheckState = CheckState.Checked;
            pathcheckBox.Font = new Font("Segoe UI", 8.25F);
            pathcheckBox.Location = new Point(20, 279);
            pathcheckBox.Margin = new Padding(4, 3, 4, 3);
            pathcheckBox.Name = "pathcheckBox";
            pathcheckBox.Size = new Size(55, 17);
            pathcheckBox.TabIndex = 9;
            pathcheckBox.Text = "paths";
            pathcheckBox.UseVisualStyleBackColor = true;
            pathcheckBox.CheckedChanged += pathcheckBox_CheckedChanged;
            // 
            // radioButton1
            // 
            radioButton1.AutoSize = true;
            radioButton1.Font = new Font("Segoe UI", 8.25F);
            radioButton1.Location = new Point(20, 299);
            radioButton1.Margin = new Padding(4, 3, 4, 3);
            radioButton1.Name = "radioButton1";
            radioButton1.Size = new Size(64, 17);
            radioButton1.TabIndex = 10;
            radioButton1.TabStop = true;
            radioButton1.Text = "TX + RX";
            radioButton1.UseVisualStyleBackColor = true;
            // 
            // radioButton2
            // 
            radioButton2.AutoSize = true;
            radioButton2.Font = new Font("Segoe UI", 8.25F);
            radioButton2.Location = new Point(20, 319);
            radioButton2.Margin = new Padding(4, 3, 4, 3);
            radioButton2.Name = "radioButton2";
            radioButton2.Size = new Size(62, 17);
            radioButton2.TabIndex = 11;
            radioButton2.TabStop = true;
            radioButton2.Text = "TX only";
            radioButton2.UseVisualStyleBackColor = true;
            // 
            // radioButton3
            // 
            radioButton3.AutoSize = true;
            radioButton3.Font = new Font("Segoe UI", 8.25F);
            radioButton3.Location = new Point(20, 337);
            radioButton3.Margin = new Padding(4, 3, 4, 3);
            radioButton3.Name = "radioButton3";
            radioButton3.Size = new Size(63, 17);
            radioButton3.TabIndex = 12;
            radioButton3.TabStop = true;
            radioButton3.Text = "RX only";
            radioButton3.UseVisualStyleBackColor = true;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label8.Location = new Point(8, 357);
            label8.Margin = new Padding(4, 0, 4, 0);
            label8.Name = "label8";
            label8.Size = new Size(58, 13);
            label8.TabIndex = 14;
            label8.Text = "De-clutter";
            // 
            // clutterlistBox
            // 
            clutterlistBox.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            clutterlistBox.FormattingEnabled = true;
            clutterlistBox.Items.AddRange(new object[] { "0", "50", "100", "200", "400", "600", "800", "1000", "1200", "1500", "1800" });
            clutterlistBox.Location = new Point(16, 373);
            clutterlistBox.Margin = new Padding(4, 3, 4, 3);
            clutterlistBox.Name = "clutterlistBox";
            clutterlistBox.Size = new Size(56, 134);
            clutterlistBox.TabIndex = 15;
            // 
            // mlslabel
            // 
            mlslabel.AutoSize = true;
            mlslabel.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            mlslabel.Location = new Point(74, 373);
            mlslabel.Margin = new Padding(4, 0, 4, 0);
            mlslabel.Name = "mlslabel";
            mlslabel.Size = new Size(24, 13);
            mlslabel.TabIndex = 16;
            mlslabel.Text = "mls";
            // 
            // kmcheckBox
            // 
            kmcheckBox.AutoSize = true;
            kmcheckBox.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            kmcheckBox.Location = new Point(80, 417);
            kmcheckBox.Margin = new Padding(4, 3, 4, 3);
            kmcheckBox.Name = "kmcheckBox";
            kmcheckBox.Size = new Size(62, 17);
            kmcheckBox.TabIndex = 17;
            kmcheckBox.Text = "use km";
            kmcheckBox.UseVisualStyleBackColor = true;
            kmcheckBox.CheckedChanged += kmcheckBox_CheckedChanged;
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label9.Location = new Point(20, 560);
            label9.Margin = new Padding(4, 0, 4, 0);
            label9.Name = "label9";
            label9.Size = new Size(42, 13);
            label9.TabIndex = 18;
            label9.Text = "Zoom: ";
            // 
            // Zoomlabel
            // 
            Zoomlabel.AutoSize = true;
            Zoomlabel.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Zoomlabel.Location = new Point(64, 559);
            Zoomlabel.Margin = new Padding(4, 0, 4, 0);
            Zoomlabel.Name = "Zoomlabel";
            Zoomlabel.Size = new Size(13, 13);
            Zoomlabel.TabIndex = 19;
            Zoomlabel.Text = "2";
            // 
            // recentrebutton
            // 
            recentrebutton.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            recentrebutton.Location = new Point(7, 576);
            recentrebutton.Margin = new Padding(4, 3, 4, 3);
            recentrebutton.Name = "recentrebutton";
            recentrebutton.Size = new Size(62, 22);
            recentrebutton.TabIndex = 20;
            recentrebutton.Text = "Recentre";
            recentrebutton.UseVisualStyleBackColor = true;
            recentrebutton.Click += recentrebutton_Click;
            // 
            // autocheckBox
            // 
            autocheckBox.AutoSize = true;
            autocheckBox.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            autocheckBox.Location = new Point(29, 607);
            autocheckBox.Margin = new Padding(4, 3, 4, 3);
            autocheckBox.Name = "autocheckBox";
            autocheckBox.Size = new Size(79, 17);
            autocheckBox.TabIndex = 21;
            autocheckBox.Text = "Auto track";
            autocheckBox.UseVisualStyleBackColor = true;
            autocheckBox.CheckedChanged += autocheckBox_CheckedChanged;
            // 
            // timer1
            // 
            timer1.Interval = 580000;
            timer1.Tick += timer1_Tick;
            // 
            // showcheckBox
            // 
            showcheckBox.AutoSize = true;
            showcheckBox.Location = new Point(307, 159);
            showcheckBox.Margin = new Padding(4, 3, 4, 3);
            showcheckBox.Name = "showcheckBox";
            showcheckBox.Size = new Size(108, 19);
            showcheckBox.TabIndex = 18;
            showcheckBox.Text = "Show password";
            showcheckBox.UseVisualStyleBackColor = true;
            showcheckBox.CheckedChanged += showcheckBox_CheckedChanged;
            // 
            // cancelbutton
            // 
            cancelbutton.Location = new Point(289, 210);
            cancelbutton.Margin = new Padding(4, 3, 4, 3);
            cancelbutton.Name = "cancelbutton";
            cancelbutton.Size = new Size(75, 23);
            cancelbutton.TabIndex = 17;
            cancelbutton.Text = "Cancel";
            cancelbutton.UseVisualStyleBackColor = true;
            cancelbutton.Click += cancelbutton_Click;
            // 
            // savebutton
            // 
            savebutton.Location = new Point(167, 210);
            savebutton.Margin = new Padding(4, 3, 4, 3);
            savebutton.Name = "savebutton";
            savebutton.Size = new Size(75, 23);
            savebutton.TabIndex = 16;
            savebutton.Text = "Save";
            savebutton.UseVisualStyleBackColor = true;
            savebutton.Click += savebutton_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(54, 113);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(87, 15);
            label1.TabIndex = 15;
            label1.Text = "Station locator:";
            // 
            // locatortextBox
            // 
            locatortextBox.Location = new Point(150, 110);
            locatortextBox.Margin = new Padding(4, 3, 4, 3);
            locatortextBox.Name = "locatortextBox";
            locatortextBox.ReadOnly = true;
            locatortextBox.Size = new Size(82, 23);
            locatortextBox.TabIndex = 14;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(54, 72);
            label5.Margin = new Padding(4, 0, 4, 0);
            label5.Name = "label5";
            label5.Size = new Size(90, 15);
            label5.TabIndex = 13;
            label5.Text = "Station callsign:";
            // 
            // calltextBox
            // 
            calltextBox.Location = new Point(150, 69);
            calltextBox.Margin = new Padding(4, 3, 4, 3);
            calltextBox.Name = "calltextBox";
            calltextBox.ReadOnly = true;
            calltextBox.Size = new Size(100, 23);
            calltextBox.TabIndex = 12;
            // 
            // passtextBox
            // 
            passtextBox.Location = new Point(150, 157);
            passtextBox.Margin = new Padding(4, 3, 4, 3);
            passtextBox.Name = "passtextBox";
            passtextBox.PasswordChar = '*';
            passtextBox.Size = new Size(132, 23);
            passtextBox.TabIndex = 11;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(40, 202);
            label6.Margin = new Padding(4, 0, 4, 0);
            label6.Name = "label6";
            label6.Size = new Size(0, 15);
            label6.TabIndex = 10;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(33, 160);
            label7.Margin = new Padding(4, 0, 4, 0);
            label7.Name = "label7";
            label7.Size = new Size(111, 15);
            label7.TabIndex = 9;
            label7.Text = "Database password:";
            // 
            // groupBox1
            // 
            groupBox1.BackColor = Color.LightYellow;
            groupBox1.Controls.Add(label1);
            groupBox1.Controls.Add(showcheckBox);
            groupBox1.Controls.Add(locatortextBox);
            groupBox1.Controls.Add(savebutton);
            groupBox1.Controls.Add(label5);
            groupBox1.Controls.Add(cancelbutton);
            groupBox1.Controls.Add(calltextBox);
            groupBox1.Controls.Add(passtextBox);
            groupBox1.Controls.Add(label7);
            groupBox1.Controls.Add(label6);
            groupBox1.Location = new Point(384, 159);
            groupBox1.Margin = new Padding(4, 3, 4, 3);
            groupBox1.Name = "groupBox1";
            groupBox1.Padding = new Padding(4, 3, 4, 3);
            groupBox1.Size = new Size(436, 282);
            groupBox1.TabIndex = 22;
            groupBox1.TabStop = false;
            groupBox1.Text = "groupBox2";
            groupBox1.Visible = false;
            // 
            // QcheckBox
            // 
            QcheckBox.AutoSize = true;
            QcheckBox.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            QcheckBox.Location = new Point(11, 525);
            QcheckBox.Margin = new Padding(4, 3, 4, 3);
            QcheckBox.Name = "QcheckBox";
            QcheckBox.Size = new Size(127, 30);
            QcheckBox.TabIndex = 23;
            QcheckBox.Text = "Don't show special \r\ncalls (Q,  0 or 1)";
            QcheckBox.UseVisualStyleBackColor = true;
            // 
            // loadinglabel
            // 
            loadinglabel.AutoSize = true;
            loadinglabel.ForeColor = SystemColors.ControlText;
            loadinglabel.Location = new Point(33, 29);
            loadinglabel.Margin = new Padding(4, 0, 4, 0);
            loadinglabel.Name = "loadinglabel";
            loadinglabel.Size = new Size(247, 60);
            loadinglabel.TabIndex = 0;
            loadinglabel.Text = "Initialising WSPR Scheduler Map... please wait\r\n\r\nRun WSPR Scheduler for RX reports + \r\nWSPR Scheduler Live for TX reports";
            // 
            // loadingPanel
            // 
            loadingPanel.BackColor = Color.SeaShell;
            loadingPanel.Controls.Add(loadinglabel);
            loadingPanel.Location = new Point(374, 272);
            loadingPanel.Margin = new Padding(4, 3, 4, 3);
            loadingPanel.Name = "loadingPanel";
            loadingPanel.Size = new Size(309, 121);
            loadingPanel.TabIndex = 25;
            loadingPanel.Visible = false;
            // 
            // europeButton
            // 
            europeButton.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            europeButton.Location = new Point(33, 644);
            europeButton.Margin = new Padding(4, 3, 4, 3);
            europeButton.Name = "europeButton";
            europeButton.Size = new Size(75, 23);
            europeButton.TabIndex = 26;
            europeButton.Text = "Europe";
            europeButton.UseVisualStyleBackColor = true;
            europeButton.Click += europeButton_Click;
            // 
            // americasButton
            // 
            americasButton.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            americasButton.Location = new Point(33, 673);
            americasButton.Margin = new Padding(4, 3, 4, 3);
            americasButton.Name = "americasButton";
            americasButton.Size = new Size(75, 23);
            americasButton.TabIndex = 27;
            americasButton.Text = "Americas";
            americasButton.UseVisualStyleBackColor = true;
            americasButton.Click += americasButton_Click;
            // 
            // pacificButton
            // 
            pacificButton.Location = new Point(33, 702);
            pacificButton.Margin = new Padding(4, 3, 4, 3);
            pacificButton.Name = "pacificButton";
            pacificButton.Size = new Size(75, 23);
            pacificButton.TabIndex = 28;
            pacificButton.Text = "Pacific";
            pacificButton.UseVisualStyleBackColor = true;
            pacificButton.Click += pacificButton_Click;
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label10.Location = new Point(24, 627);
            label10.Margin = new Padding(4, 0, 4, 0);
            label10.Name = "label10";
            label10.Size = new Size(61, 13);
            label10.TabIndex = 29;
            label10.Text = "Centre on:";
            // 
            // bottomlabel
            // 
            bottomlabel.AutoSize = true;
            bottomlabel.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            bottomlabel.Location = new Point(35, 742);
            bottomlabel.Name = "bottomlabel";
            bottomlabel.Size = new Size(32, 13);
            bottomlabel.TabIndex = 30;
            bottomlabel.Text = "label";
            // 
            // rightPanel
            // 
            rightPanel.Controls.Add(filterbutton);
            rightPanel.Controls.Add(bandlistBox);
            rightPanel.Controls.Add(label10);
            rightPanel.Controls.Add(periodlistBox);
            rightPanel.Controls.Add(pinlabel);
            rightPanel.Controls.Add(pacificButton);
            rightPanel.Controls.Add(label2);
            rightPanel.Controls.Add(americasButton);
            rightPanel.Controls.Add(label3);
            rightPanel.Controls.Add(europeButton);
            rightPanel.Controls.Add(pathcheckBox);
            rightPanel.Controls.Add(radioButton1);
            rightPanel.Controls.Add(QcheckBox);
            rightPanel.Controls.Add(radioButton2);
            rightPanel.Controls.Add(radioButton3);
            rightPanel.Controls.Add(autocheckBox);
            rightPanel.Controls.Add(label8);
            rightPanel.Controls.Add(recentrebutton);
            rightPanel.Controls.Add(clutterlistBox);
            rightPanel.Controls.Add(Zoomlabel);
            rightPanel.Controls.Add(label4);
            rightPanel.Controls.Add(label9);
            rightPanel.Controls.Add(configbutton);
            rightPanel.Controls.Add(kmcheckBox);
            rightPanel.Controls.Add(mlslabel);
            rightPanel.Location = new Point(1033, 7);
            rightPanel.Name = "rightPanel";
            rightPanel.Size = new Size(162, 748);
            rightPanel.TabIndex = 31;
            // 
            // panel1
            // 
            panel1.BackColor = Color.SeaShell;
            panel1.Controls.Add(Waitlabel);
            panel1.Location = new Point(463, 85);
            panel1.Name = "panel1";
            panel1.Size = new Size(303, 120);
            panel1.TabIndex = 32;
            panel1.Visible = false;
            // 
            // Waitlabel
            // 
            Waitlabel.AutoSize = true;
            Waitlabel.Location = new Point(68, 56);
            Waitlabel.Name = "Waitlabel";
            Waitlabel.Size = new Size(152, 15);
            Waitlabel.TabIndex = 0;
            Waitlabel.Text = "Please wait .. updating map";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoScroll = true;
            ClientSize = new Size(1215, 757);
            Controls.Add(panel1);
            Controls.Add(rightPanel);
            Controls.Add(bottomlabel);
            Controls.Add(loadingPanel);
            Controls.Add(groupBox1);
            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Margin = new Padding(4, 3, 4, 3);
            Name = "Form1";
            Text = "Reports for this station";
            Load += Form1_Load;
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            loadingPanel.ResumeLayout(false);
            loadingPanel.PerformLayout();
            rightPanel.ResumeLayout(false);
            rightPanel.PerformLayout();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Label pinlabel;
        private ListBox periodlistBox;
        private ListBox bandlistBox;
        private Label label2;
        private Label label3;
        private Label label4;
        private Button filterbutton;
        private Button configbutton;
        private CheckBox pathcheckBox;
        private RadioButton radioButton1;
        private RadioButton radioButton2;
        private RadioButton radioButton3;
        private Label label8;
        private ListBox clutterlistBox;
        private Label mlslabel;
        private CheckBox kmcheckBox;
        private Label label9;
        private Label Zoomlabel;
        private Button recentrebutton;
        private CheckBox autocheckBox;
        private System.Windows.Forms.Timer timer1;
        private CheckBox showcheckBox;
        private Button cancelbutton;
        private Button savebutton;
        private Label label1;
        private TextBox locatortextBox;
        private Label label5;
        private TextBox calltextBox;
        private TextBox passtextBox;
        private Label label6;
        private Label label7;
       
        private GroupBox groupBox1;
        private CheckBox QcheckBox;
        private Label loadinglabel;
        private Panel loadingPanel;
        private Button europeButton;
        private Button americasButton;
        private Button pacificButton;
        private Label label10;
        private Label bottomlabel;
        private Panel rightPanel;
        private Panel panel1;
        private Label Waitlabel;
    }
}
