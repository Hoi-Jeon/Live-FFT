namespace LiveFFTNameSpace
{
    partial class LiveFFT
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataVisualization.Charting.LineAnnotation lineAnnotation1 = new System.Windows.Forms.DataVisualization.Charting.LineAnnotation();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LiveFFT));
            this.srPort = new System.IO.Ports.SerialPort(this.components);
            this.btnConnect = new System.Windows.Forms.Button();
            this.btnDisconnect = new System.Windows.Forms.Button();
            this.cboPorts = new System.Windows.Forms.ComboBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.tssLBLstatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsLBLSpacer1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.lbNBytes = new System.Windows.Forms.ToolStripStatusLabel();
            this.chartADC = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.btnADCon = new System.Windows.Forms.Button();
            this.btnADCoff = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.btnFFTon = new System.Windows.Forms.Button();
            this.cbColormap = new System.Windows.Forms.CheckBox();
            this.pbColorMapScale = new System.Windows.Forms.PictureBox();
            this.lbl_Ymax = new System.Windows.Forms.Label();
            this.tb_Ymax = new System.Windows.Forms.TextBox();
            this.lbl_Ymin = new System.Windows.Forms.Label();
            this.tb_Ymin = new System.Windows.Forms.TextBox();
            this.lbl_Yint = new System.Windows.Forms.Label();
            this.tb_Yint = new System.Windows.Forms.TextBox();
            this.btnReplot = new System.Windows.Forms.Button();
            this.cbAweight = new System.Windows.Forms.CheckBox();
            this.statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chartADC)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbColorMapScale)).BeginInit();
            this.SuspendLayout();
            // 
            // srPort
            // 
            this.srPort.BaudRate = 115200;
            this.srPort.PortName = "COM3";
            this.srPort.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(this.srPort_DataReceived);
            // 
            // btnConnect
            // 
            this.btnConnect.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnConnect.Location = new System.Drawing.Point(3, 30);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(74, 34);
            this.btnConnect.TabIndex = 0;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // btnDisconnect
            // 
            this.btnDisconnect.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnDisconnect.Enabled = false;
            this.btnDisconnect.Location = new System.Drawing.Point(3, 70);
            this.btnDisconnect.Name = "btnDisconnect";
            this.btnDisconnect.Size = new System.Drawing.Size(74, 34);
            this.btnDisconnect.TabIndex = 1;
            this.btnDisconnect.Text = "Disconnect";
            this.btnDisconnect.UseVisualStyleBackColor = true;
            this.btnDisconnect.Click += new System.EventHandler(this.btnDisconnect_Click);
            // 
            // cboPorts
            // 
            this.cboPorts.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cboPorts.FormattingEnabled = true;
            this.cboPorts.Location = new System.Drawing.Point(3, 3);
            this.cboPorts.Name = "cboPorts";
            this.cboPorts.Size = new System.Drawing.Size(74, 21);
            this.cboPorts.TabIndex = 2;
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tssLBLstatus,
            this.tsLBLSpacer1,
            this.toolStripStatusLabel1,
            this.lbNBytes});
            this.statusStrip1.Location = new System.Drawing.Point(0, 318);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(569, 22);
            this.statusStrip1.TabIndex = 3;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // tssLBLstatus
            // 
            this.tssLBLstatus.Name = "tssLBLstatus";
            this.tssLBLstatus.Size = new System.Drawing.Size(79, 17);
            this.tssLBLstatus.Text = "Disconnected";
            // 
            // tsLBLSpacer1
            // 
            this.tsLBLSpacer1.Name = "tsLBLSpacer1";
            this.tsLBLSpacer1.Size = new System.Drawing.Size(308, 17);
            this.tsLBLSpacer1.Spring = true;
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(142, 17);
            this.toolStripStatusLabel1.Text = "Number of data in buffer:";
            // 
            // lbNBytes
            // 
            this.lbNBytes.Name = "lbNBytes";
            this.lbNBytes.Size = new System.Drawing.Size(25, 17);
            this.lbNBytes.Text = "000";
            // 
            // chartADC
            // 
            lineAnnotation1.Name = "LineAnnotation1";
            this.chartADC.Annotations.Add(lineAnnotation1);
            this.chartADC.BackColor = System.Drawing.SystemColors.Control;
            chartArea1.AxisX.IsLabelAutoFit = false;
            chartArea1.AxisX.LabelStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            chartArea1.AxisY.IsLabelAutoFit = false;
            chartArea1.AxisY.LabelStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            chartArea1.BorderColor = System.Drawing.Color.Gray;
            chartArea1.BorderDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid;
            chartArea1.Name = "ChartArea1";
            this.chartADC.ChartAreas.Add(chartArea1);
            this.chartADC.Dock = System.Windows.Forms.DockStyle.Fill;
            legend1.Enabled = false;
            legend1.Name = "Legend1";
            this.chartADC.Legends.Add(legend1);
            this.chartADC.Location = new System.Drawing.Point(83, 3);
            this.chartADC.Name = "chartADC";
            this.tableLayoutPanel1.SetRowSpan(this.chartADC, 9);
            series1.BorderColor = System.Drawing.Color.Navy;
            series1.ChartArea = "ChartArea1";
            series1.Color = System.Drawing.Color.White;
            series1.Legend = "Legend1";
            series1.Name = "ADC0Slow";
            series2.BorderColor = System.Drawing.Color.Black;
            series2.ChartArea = "ChartArea1";
            series2.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series2.Font = new System.Drawing.Font("Microsoft Sans Serif", 6F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            series2.IsVisibleInLegend = false;
            series2.LabelForeColor = System.Drawing.Color.White;
            series2.Legend = "Legend1";
            series2.Name = "ADC0";
            this.chartADC.Series.Add(series1);
            this.chartADC.Series.Add(series2);
            this.chartADC.Size = new System.Drawing.Size(408, 312);
            this.chartADC.TabIndex = 5;
            this.chartADC.Text = "ADC";
            // 
            // btnADCon
            // 
            this.btnADCon.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnADCon.Enabled = false;
            this.btnADCon.Location = new System.Drawing.Point(3, 190);
            this.btnADCon.Name = "btnADCon";
            this.btnADCon.Size = new System.Drawing.Size(74, 34);
            this.btnADCon.TabIndex = 10;
            this.btnADCon.Text = "ADC on";
            this.btnADCon.UseVisualStyleBackColor = true;
            this.btnADCon.Click += new System.EventHandler(this.btnADCon_Click);
            // 
            // btnADCoff
            // 
            this.btnADCoff.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnADCoff.Enabled = false;
            this.btnADCoff.Location = new System.Drawing.Point(3, 150);
            this.btnADCoff.Name = "btnADCoff";
            this.btnADCoff.Size = new System.Drawing.Size(74, 34);
            this.btnADCoff.TabIndex = 11;
            this.btnADCoff.Text = "STOP";
            this.btnADCoff.UseVisualStyleBackColor = true;
            this.btnADCoff.Click += new System.EventHandler(this.btnADCoff_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 15F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tableLayoutPanel1.Controls.Add(this.cboPorts, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnConnect, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.btnDisconnect, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.chartADC, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnADCoff, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.btnADCon, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.btnFFTon, 0, 6);
            this.tableLayoutPanel1.Controls.Add(this.cbColormap, 0, 7);
            this.tableLayoutPanel1.Controls.Add(this.pbColorMapScale, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.lbl_Ymax, 3, 0);
            this.tableLayoutPanel1.Controls.Add(this.tb_Ymax, 3, 1);
            this.tableLayoutPanel1.Controls.Add(this.lbl_Ymin, 3, 2);
            this.tableLayoutPanel1.Controls.Add(this.tb_Ymin, 3, 3);
            this.tableLayoutPanel1.Controls.Add(this.lbl_Yint, 3, 4);
            this.tableLayoutPanel1.Controls.Add(this.tb_Yint, 3, 5);
            this.tableLayoutPanel1.Controls.Add(this.btnReplot, 3, 6);
            this.tableLayoutPanel1.Controls.Add(this.cbAweight, 0, 8);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 9;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(569, 318);
            this.tableLayoutPanel1.TabIndex = 12;
            // 
            // btnFFTon
            // 
            this.btnFFTon.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnFFTon.Enabled = false;
            this.btnFFTon.Location = new System.Drawing.Point(3, 230);
            this.btnFFTon.Name = "btnFFTon";
            this.btnFFTon.Size = new System.Drawing.Size(74, 34);
            this.btnFFTon.TabIndex = 13;
            this.btnFFTon.Text = "FFT on";
            this.btnFFTon.UseVisualStyleBackColor = true;
            this.btnFFTon.Click += new System.EventHandler(this.btnFFTon_Click);
            // 
            // cbColormap
            // 
            this.cbColormap.AutoSize = true;
            this.cbColormap.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cbColormap.Enabled = false;
            this.cbColormap.Location = new System.Drawing.Point(3, 270);
            this.cbColormap.Name = "cbColormap";
            this.cbColormap.Size = new System.Drawing.Size(74, 19);
            this.cbColormap.TabIndex = 21;
            this.cbColormap.Text = "Colormap";
            this.cbColormap.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.cbColormap.UseVisualStyleBackColor = true;
            this.cbColormap.CheckedChanged += new System.EventHandler(this.cbColormap_CheckedChanged);
            // 
            // pbColorMapScale
            // 
            this.pbColorMapScale.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pbColorMapScale.Location = new System.Drawing.Point(497, 3);
            this.pbColorMapScale.Name = "pbColorMapScale";
            this.tableLayoutPanel1.SetRowSpan(this.pbColorMapScale, 7);
            this.pbColorMapScale.Size = new System.Drawing.Size(9, 261);
            this.pbColorMapScale.TabIndex = 22;
            this.pbColorMapScale.TabStop = false;
            // 
            // lbl_Ymax
            // 
            this.lbl_Ymax.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lbl_Ymax.AutoSize = true;
            this.lbl_Ymax.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_Ymax.Location = new System.Drawing.Point(512, 1);
            this.lbl_Ymax.Name = "lbl_Ymax";
            this.lbl_Ymax.Size = new System.Drawing.Size(39, 26);
            this.lbl_Ymax.TabIndex = 14;
            this.lbl_Ymax.Text = "Max. Range";
            this.lbl_Ymax.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // tb_Ymax
            // 
            this.tb_Ymax.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tb_Ymax.Location = new System.Drawing.Point(512, 30);
            this.tb_Ymax.Name = "tb_Ymax";
            this.tb_Ymax.Size = new System.Drawing.Size(54, 20);
            this.tb_Ymax.TabIndex = 16;
            // 
            // lbl_Ymin
            // 
            this.lbl_Ymin.AutoSize = true;
            this.lbl_Ymin.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbl_Ymin.Location = new System.Drawing.Point(512, 67);
            this.lbl_Ymin.Name = "lbl_Ymin";
            this.lbl_Ymin.Size = new System.Drawing.Size(54, 40);
            this.lbl_Ymin.TabIndex = 15;
            this.lbl_Ymin.Text = "Min. Range";
            this.lbl_Ymin.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // tb_Ymin
            // 
            this.tb_Ymin.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tb_Ymin.Location = new System.Drawing.Point(512, 110);
            this.tb_Ymin.Name = "tb_Ymin";
            this.tb_Ymin.Size = new System.Drawing.Size(54, 20);
            this.tb_Ymin.TabIndex = 17;
            // 
            // lbl_Yint
            // 
            this.lbl_Yint.AutoSize = true;
            this.lbl_Yint.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbl_Yint.Location = new System.Drawing.Point(512, 147);
            this.lbl_Yint.Name = "lbl_Yint";
            this.lbl_Yint.Size = new System.Drawing.Size(54, 40);
            this.lbl_Yint.TabIndex = 18;
            this.lbl_Yint.Text = "Interval";
            this.lbl_Yint.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // tb_Yint
            // 
            this.tb_Yint.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tb_Yint.Location = new System.Drawing.Point(512, 190);
            this.tb_Yint.Name = "tb_Yint";
            this.tb_Yint.Size = new System.Drawing.Size(54, 20);
            this.tb_Yint.TabIndex = 19;
            // 
            // btnReplot
            // 
            this.btnReplot.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnReplot.Location = new System.Drawing.Point(512, 230);
            this.btnReplot.Name = "btnReplot";
            this.btnReplot.Size = new System.Drawing.Size(54, 34);
            this.btnReplot.TabIndex = 20;
            this.btnReplot.Text = "Replot";
            this.btnReplot.UseVisualStyleBackColor = true;
            this.btnReplot.Click += new System.EventHandler(this.btnReplot_Click);
            // 
            // cbAweight
            // 
            this.cbAweight.AutoSize = true;
            this.cbAweight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cbAweight.Enabled = false;
            this.cbAweight.Location = new System.Drawing.Point(3, 295);
            this.cbAweight.Name = "cbAweight";
            this.cbAweight.Size = new System.Drawing.Size(74, 20);
            this.cbAweight.TabIndex = 23;
            this.cbAweight.Text = "A-weight";
            this.cbAweight.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.cbAweight.UseVisualStyleBackColor = true;
            this.cbAweight.CheckedChanged += new System.EventHandler(this.cbAweight_CheckedChanged);
            // 
            // LiveFFT
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(569, 340);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.statusStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "LiveFFT";
            this.Text = "Live FFT -Tiva C series TM4C123GXL";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chartADC)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbColorMapScale)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.IO.Ports.SerialPort srPort;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Button btnDisconnect;
        private System.Windows.Forms.ComboBox cboPorts;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel tssLBLstatus;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartADC;
        private System.Windows.Forms.Button btnADCon;
        private System.Windows.Forms.Button btnADCoff;
        private System.Windows.Forms.ToolStripStatusLabel tsLBLSpacer1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripStatusLabel lbNBytes;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button btnFFTon;
        private System.Windows.Forms.Label lbl_Ymax;
        private System.Windows.Forms.Label lbl_Ymin;
        private System.Windows.Forms.TextBox tb_Ymax;
        private System.Windows.Forms.TextBox tb_Ymin;
        private System.Windows.Forms.Label lbl_Yint;
        private System.Windows.Forms.TextBox tb_Yint;
        private System.Windows.Forms.Button btnReplot;
        private System.Windows.Forms.CheckBox cbColormap;
        private System.Windows.Forms.PictureBox pbColorMapScale;
        private System.Windows.Forms.CheckBox cbAweight;
    }
}

