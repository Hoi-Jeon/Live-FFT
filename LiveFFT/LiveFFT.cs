using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;

using System.Windows.Forms.DataVisualization.Charting;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using ColorMapNameSpace;

namespace LiveFFTNameSpace
{
    public partial class LiveFFT : Form
    {
        public const Int32 NdataOneSide = 2047;             ///< Number of data in 12 bit at one side of [-1, 1]
        public double dADC = 1.0 / NdataOneSide;            ///< Resolution of ADC data in [-1, 1]

        public double Fs = 4096;                            ///< Sampling Frequency   
        private const Int32 NByteTime = 2048;               ///< Size of buffer (SHOULD be EVEN Number)
        private byte[] BufferADC0 = new byte[NByteTime];    ///< Internal Buffer to buffer ADC signal
        private double[] ADC0 = new double[NByteTime/2];    ///< Converted ADC value
        private double[] TimeX = new double[NByteTime/2];   ///< Time x-axis data for chart

        private const Int32 NFFT = 128;                     ///< Number of FFT
        private byte[] BufferFFT = new byte[NFFT*2];        ///< Internal Buffer to buffer FFT signal (1 float = 4 bytes)
        private float[] FFT0 = new float[NFFT/2];           ///< FFT value
        private float[] FFT0x = new float[NFFT/2];          ///< FFT value from previous Step
        private float[] FFT0xSlow = new float[NFFT / 2];    ///< FFT value with a slower exponential averaging
        private float[] dBA_Weight = new float[NFFT / 2];   ///< dBA Weighting array
        private float[] FreqX = new float[NFFT/2];          ///< Frequency x-axis data for chart

        static byte[] CMD_NO_CMD = { 0x00 };                ///< no command was actually send by serial connection
        static byte[] CMD_SEND_ADC = { 0x01 };              ///< Start sending ADC (Time domain) signal
        static byte[] CMD_STOP = { 0x02 };                  ///< Stop sending ADC|FFT signal
        static byte[] CMD_SEND_FFT = { 0x03 };              ///< Start sending FFT signal
        static byte[] CMD_DISCONNECT = { 0x0D };            ///< Disconnect serail connection to Tiva Board
        private byte[] CMD_CURRENT = CMD_NO_CMD;            ///< Current CMD

        private Int32 MinByte = 2;                          ///< Minimum numer of bytes to read at one time in serial port
        private Int32 idxBuffer = 0;                        ///< index of the last read element
        private Int32 NbytesToRead;                         ///< Numer of bytes to read at one time in serial port

        private static int nCmap = 256;                     ///< Number of colormap scale
        private int[,] JetColorMap = new int[nCmap, 4];     ///< An array for 8bit Jet color map 
        private int bMapWidth = 0, bMapHeight = 0;          ///< Colormap width and height
        private Bitmap bMapCurrent, bMapNew;                ///< bitmap color maps
        private double MaxValue, MinValue;                  ///< Global max. and min. variable for plotting
        private int dxScroll = 1;                           ///< Scroll step in the FFT colormap 
        public LiveFFT()
        {
            InitializeComponent();
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {            
            // Open Serial port
            srPort.PortName = Convert.ToString(cboPorts.Text);                               
            srPort.ReceivedBytesThreshold = MinByte;
            srPort.Open();
            srPort.DiscardInBuffer();
            srPort.DiscardOutBuffer();

            // Clean all data in buffer
            srPort.Write(CMD_STOP, 0, 1);

            // Activate and deactivate buttons
            btnConnect.Enabled = false;
            btnDisconnect.Enabled = true;
            btnADCoff.Enabled = false;
            btnADCon.Enabled = true;
            btnFFTon.Enabled = true;
            cbColormap.Enabled = false;
            cbAweight.Enabled = false;

            // Change texts in the status bar
            tssLBLstatus.Text = srPort.PortName;
        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            // Reset "TimeBlockStart" to true in TI Launchpad
            srPort.Write(CMD_DISCONNECT, 0, 1);

            // Suspend 0.1 second
            //  This line is not necessary, but just to send the last serial command to TI Launchpad 100% successfully
            System.Threading.Thread.Sleep(100); 

            // Close Serial port
            srPort.DiscardInBuffer();
            srPort.DiscardOutBuffer();
            srPort.Close();

            // Activate and deactivate buttons
            btnConnect.Enabled = true;
            btnDisconnect.Enabled = false;
            btnADCoff.Enabled = false;
            btnADCon.Enabled = false;
            btnFFTon.Enabled = false;
            cbColormap.Enabled = false;
            cbAweight.Enabled = false;

            // Change texts in the status bar
            tssLBLstatus.Text = "Disconnected";
            lbNBytes.Text = "0";
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Create a x-axis time data for Chart
            for (int n = 0; n < NByteTime/2; n++)
                TimeX[n] = n/Fs;

            // Create a x-axis time data for Chart
            for (int n = 0; n < NFFT/2; n++)
                FreqX[n] = (float) Fs/NFFT*n;

            // Calculate dBA Weighting
            // ref: https://en.wikipedia.org/wiki/A-weighting
            dBA_Weight[0] = 0; // DC components
            for (int n = 1; n < NFFT / 2; n++)
            {
                double FreqX2 = Math.Pow(FreqX[n], 2);

                double RA = Math.Pow(12194, 2) * Math.Pow(FreqX2, 2);
                double RA_den = (FreqX2 + 20.6*20.6)
                                * Math.Sqrt((FreqX2 + 107.7*107.7)
                                * (FreqX2 + 737.9*737.9))
                                * (FreqX2 + 12194*12194);
                RA /= RA_den;
                dBA_Weight[n] = (float)(20*Math.Log10(RA) + 2.0f);
            }           

            // Initialize a chart setting
            chartADC.Text = "ADC";
            chartADC.ChartAreas[0].AxisX.Title = "Time [sec]";
            chartADC.ChartAreas[0].AxisX.Minimum = 0;
            chartADC.ChartAreas[0].AxisX.Interval = 0.05;
            chartADC.ChartAreas[0].AxisX.MajorGrid.LineDashStyle = ChartDashStyle.Dash;
            chartADC.ChartAreas[0].AxisX.MajorGrid.Interval = 0.05;
            chartADC.ChartAreas[0].AxisY.Title = "ADC value";
            chartADC.ChartAreas[0].AxisY.Minimum = -0.05;
            chartADC.ChartAreas[0].AxisY.Maximum = 0.05;
            chartADC.ChartAreas[0].AxisY.Interval = 0.01;
            chartADC.ChartAreas[0].AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Dash;
            chartADC.ChartAreas[0].AxisY.MajorGrid.Interval = 0.01;

            // Initialize a colormap
            JetColorMap = ColorMapClass.JetTable();
            //JetColorMap = ColorMapClass.Jet(); // Uncomment, if generation of a RGB Jet Table is wished

            // Get all serial port names
            string[] ArrayComPortsNames = SerialPort.GetPortNames();
            try
            {
                int index = -1;
                string ComPortName = null;

                do
                {
                    index += 1;
                    cboPorts.Items.Add(ArrayComPortsNames[index]);
                }
                while (!((ArrayComPortsNames[index] == ComPortName)
                            || (index == ArrayComPortsNames.GetUpperBound(0))));
                
                Array.Sort(ArrayComPortsNames);

                // Get the first COM port for the text in the ComboBox
                if (index == ArrayComPortsNames.GetUpperBound(0))
                {
                    ComPortName = ArrayComPortsNames[0];
                }
                cboPorts.Text = ArrayComPortsNames[0];
            }

            catch (IndexOutOfRangeException err)
            {
                Console.WriteLine(err.Message);
                btnConnect.Enabled = false;
            }
        }

        private void srPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            // Local variables for Origianl 12bit integer ADC value
            int ui32ADC0; 

            // Get a number of read data from serial port
            NbytesToRead = srPort.BytesToRead;

            // Display a number of read data from serial port                
            this.BeginInvoke(new EventHandler(DisplayNbytesToRead));

            // Create a byte array for the data from serial port
            byte[] Buffer = new byte[NbytesToRead];
            srPort.Read(Buffer, 0, NbytesToRead);

            // ADC0: Time domain calculation
            if (CMD_CURRENT == CMD_SEND_ADC)
            {
                // Copy data in a buffer to the internal buffer
                for (int n = 0; n < NbytesToRead; n++)
                {
                    if (idxBuffer == NByteTime)
                    {
                        for (int i = 0; i < (NByteTime / 2); i++)
                        {
                            // Convert two bytes into Integer 32bit (original ADC value)
                            ui32ADC0 = ((BufferADC0[2 * i] << 8) | BufferADC0[2 * i + 1]) & 0x0FFF;

                            // Convert the 12bit Integer to the range [-1, 1]
                            ADC0[i] = ((int)ui32ADC0 - NdataOneSide) * dADC;
                        }

                        // Display the read ADC signal to Chart
                        this.BeginInvoke(new EventHandler(DisplayChart));

                        // Reset the location of the last read element
                        idxBuffer = 0;

                        // Continue, if the 2nd byte of 16 bit comes first to the internal buffer
                        if ((Buffer[n] & 0x10) != 0x10)
                            continue;
                    }

                    // Copy data into a temporary buffer
                    BufferADC0[idxBuffer] = Buffer[n];

                    // Increase the index of the last read element 
                    idxBuffer++;
                }
            }

            // FFT: Frequency domain calculation
            else if (CMD_CURRENT == CMD_SEND_FFT)
            {
                // Copy data in a buffer to the internal buffer
                for (int n = 0; n < NbytesToRead; n++)
                {
                    if (idxBuffer == NFFT*2)
                    {
                        for (int i = 0; i < (NFFT/2); i++)
                        {
                            // Convert 4 bytes into 1 floats
                            FFT0[i] = BitConverter.ToSingle(BufferFFT, 4 * i);
                        }

                        // Display the read ADC signal to Chart
                        if (cbColormap.Checked == false)
                            this.BeginInvoke(new EventHandler(DisplayChartFFT));
                        else
                            this.BeginInvoke(new EventHandler(DisplayColorMap));

                        // Reset the location of the last read element
                        idxBuffer = 0;
                    }

                    // Copy data into a temporary buffer
                    BufferFFT[idxBuffer] = Buffer[n];
                    // Increase the index of the last read element 
                    idxBuffer++;
                }
            }
        }
        
        private void DisplayNbytesToRead(object sender, EventArgs e)
        {
            // Show the number of bytes to read
            lbNBytes.Text = NbytesToRead.ToString("000");
        }

        private void DisplayChart(object sender, EventArgs e)
        {
            // Clear all exisitng chart points
            chartADC.Series["ADC0"].Points.Clear();

            try 
            {
                for (int n = 0; n < (NByteTime / 2); n++)
                {
                    // Plot the time domain ADC0 data
                    chartADC.Series["ADC0"].Points.AddXY(TimeX[n], ADC0[n]);
                }
            }
            catch
            {

            }
        }

        private void DisplayChartFFT(object sender, EventArgs e)
        {
            try
            {   // Initiailze a chart background for FFT
                // This command is a duplicate, but it is inserted here to clear a colormap background completely 
                chartADC.ChartAreas[0].BackImage = "";
                chartADC.ChartAreas[0].BackColor = Color.White;

                // Clear all existing FFT data
                chartADC.Series["ADC0"].Points.Clear();
                chartADC.Series["ADC0Slow"].Points.Clear();

                // Set the smoothing factor exponential averaging
                float expSmoothF = 0.15f;
                float expSmoothFSlow = 0.05f;

                for (int n = 0; n < (NFFT/2); n++)
                {
                    // dBA calculation
                    float Af;
                    if (cbAweight.Checked == true)
                        Af = dBA_Weight[n];
                    else
                        Af = 0.0f;

                    // Exponential average for FFT values
                    FFT0x[n] = expSmoothF * FFT0[n] + (1-expSmoothF) * FFT0x[n];

                    // Exponential average for FFT slow values
                    if (FFT0x[n] > FFT0xSlow[n])
                        FFT0xSlow[n] = FFT0x[n];
                    else
                        FFT0xSlow[n] = expSmoothFSlow * FFT0x[n] + (1 - expSmoothFSlow) * FFT0xSlow[n];

                    // Plot the frequency domain FFT data
                    chartADC.Series["ADC0"].Points.AddXY(FreqX[n], 20 * Math.Log10(FFT0x[n]) + Af);
                    chartADC.Series["ADC0Slow"].Points.AddXY(FreqX[n], 20 * Math.Log10(FFT0xSlow[n]) + Af);
                }
            }
            catch
            {

            }
        }

        private void DisplayColorMap(object sender, EventArgs e)
        {
            // Enter a null time data for showing the ticks and label of axis
            chartADC.Series["ADC0"].Points.Clear();
            chartADC.Series["ADC0"].Points.AddXY(0, 0);

            try
            {
                // Set a frequency and time width of the backimage of Chart background
                float dFrq = (float) (bMapHeight / ((NFFT/2.0f) - 1.0f));

                // Get a new memory bitmap with the same size, as the picture box
                bMapCurrent = new Bitmap(chartADC.Images[0].Image);
                bMapNew = new Bitmap(bMapWidth, bMapHeight);
                
                // Graphics a new graphics
                Graphics grphNew = Graphics.FromImage(bMapNew);

                // Shift Graphics by dxScroll
                grphNew.DrawImage(bMapCurrent, new PointF(-dxScroll, 0));

                // Convert FFT values into Byte range within Max. and Min.
                byte[] SPLband = new byte[(NFFT/2)];
                for (int Idx = 0; Idx < (NFFT/2); Idx++)
                {
                    // dBA calculation
                    float Af;
                    if (cbAweight.Checked == true)
                        Af = dBA_Weight[Idx];
                    else
                        Af = 0.0f;

                    double tmpSPL = 20 * Math.Log10(FFT0[Idx]) + Af;

                    if (tmpSPL > MaxValue)
                        tmpSPL = MaxValue;
                    else if (tmpSPL < MinValue)
                        tmpSPL = MinValue;

                    SPLband[(NFFT/2)-(Idx+1)] = (byte) (Math.Floor(255/(MinValue-MaxValue)*(tmpSPL - MaxValue)));
                }
                
                // Copy the latest data into the most-right column
                for (int Idx = 0; Idx < (NFFT/2)-1; Idx++)
                {                    
                    // Create a Rectangle for each single frequency line
                    RectangleF RectFrq = new RectangleF(bMapWidth - dxScroll, Idx*dFrq, dxScroll, dFrq);
                    
                    // Create a linear gradient brush for a signel Rectangle
                    LinearGradientBrush LinearBrush = new LinearGradientBrush(RectFrq,
                                                            Color.FromArgb(SPLband[Idx], SPLband[Idx], SPLband[Idx]),
                                                            Color.FromArgb(SPLband[Idx+1], SPLband[Idx + 1], SPLband[Idx + 1]),
                                                            LinearGradientMode.Vertical);
                    
                    // Fill a color of Rectangle with the created linear gradient
                    grphNew.FillRectangle(LinearBrush, RectFrq);                    
                }

                // Print the bitmap with colorization
                bMapNew = ColorMapClass.Colorize(bMapNew, 255, JetColorMap);                

                // Copy the new map into the backimage of Chart background
                if (chartADC.Images.Count != 0)
                    chartADC.Images.RemoveAt(0);

                // TO COMMENT
                chartADC.Images.Add(new NamedImage("ColorMapImage", bMapNew));
                chartADC.ChartAreas[0].BackImage = chartADC.Images[0].Name;
                chartADC.ChartAreas[0].BackImageWrapMode = ChartImageWrapMode.Scaled;
            }

            catch
            {

            }
        }

        private void btnADCoff_Click(object sender, EventArgs e)
        {
            // Update the current command
            CMD_CURRENT = CMD_STOP;
            srPort.Write(CMD_CURRENT, 0, 1);

            // Run out of all remaining received bytess
            while (srPort.BytesToRead > 0) { }

            // Deactivate buttons
            btnADCoff.Enabled = false;
            btnADCon.Enabled = true;
            btnFFTon.Enabled = true;
            btnDisconnect.Enabled = true;
            cbColormap.Enabled = false;
            cbColormap.Checked = false;
            cbAweight.Enabled = false;
            cbAweight.Checked = false;

            // Close Serial port
            srPort.DiscardInBuffer();
            srPort.DiscardOutBuffer();

            // Reset counter index
            idxBuffer = 0;
        }

        private void btnADCon_Click(object sender, EventArgs e)
        {
            // Update the current command
            CMD_CURRENT = CMD_SEND_ADC;
            srPort.Write(CMD_CURRENT, 0, 1);

            // Deactivate buttons
            btnADCoff.Enabled = true;
            btnADCon.Enabled = false;
            btnFFTon.Enabled = false;
            btnDisconnect.Enabled = false;
            lbl_Yint.Enabled = true;
            tb_Yint.Enabled = true;
            cbColormap.Enabled = false;
            cbAweight.Enabled = false;
            pbColorMapScale.Visible = false;

            // Set a scale box
            tb_Ymax.Text = "0.05";
            tb_Ymin.Text = "-0.05";
            tb_Yint.Text = "0.01";

            // Set a max and min value
            MaxValue = Convert.ToDouble(tb_Ymax.Text);
            MinValue= Convert.ToDouble(tb_Ymin.Text);

            // Delete the 2nd series
            chartADC.Series["ADC0Slow"].Points.Clear();

            // Use "Line" for plotting
            chartADC.Series["ADC0"].ChartType = SeriesChartType.Line;
            chartADC.ChartAreas[0].AxisY.Crossing = MinValue;

            // Change a chart setting for ADC
            chartADC.Text = "ADC";
            chartADC.ChartAreas[0].AxisX.Title = "Time [sec]";
            chartADC.ChartAreas[0].AxisX.Minimum = 0;
            chartADC.ChartAreas[0].AxisX.Maximum = 0.25;
            chartADC.ChartAreas[0].AxisX.Interval = 0.05;
            chartADC.ChartAreas[0].AxisX.MajorGrid.LineDashStyle = ChartDashStyle.Dash;
            chartADC.ChartAreas[0].AxisX.MajorGrid.Interval = 0.05;
            chartADC.ChartAreas[0].AxisY.Title = "ADC value";
            chartADC.ChartAreas[0].AxisY.Minimum = -0.05;
            chartADC.ChartAreas[0].AxisY.Maximum = 0.05;
            chartADC.ChartAreas[0].AxisY.Interval = 0.01;
            chartADC.ChartAreas[0].AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Dash;
            chartADC.ChartAreas[0].AxisY.MajorGrid.Interval = 0.01;
        }

        private void cbAweight_CheckedChanged(object sender, EventArgs e)
        {
            if((CMD_CURRENT == CMD_SEND_FFT) && (cbColormap.Checked == false))
            {
                // Change a y-axis title for FFT plot
                if (cbAweight.Checked == true)
                    chartADC.ChartAreas[0].AxisY.Title = "FFTvalue [dBA]";
                else
                    chartADC.ChartAreas[0].AxisY.Title = "FFTvalue [dB]";
            }
        }

        private void btnFFTon_Click(object sender, EventArgs e)
        {
            // Update the current command
            CMD_CURRENT = CMD_SEND_FFT;
            srPort.Write(CMD_CURRENT, 0, 1);

            // Deactivate buttons
            btnADCoff.Enabled = true;
            btnADCon.Enabled = false;
            btnFFTon.Enabled = false;
            btnDisconnect.Enabled = false;
            lbl_Yint.Enabled = true;
            tb_Yint.Enabled = true;
            cbColormap.Enabled = true;
            cbAweight.Enabled = true;
            pbColorMapScale.Visible = false;

            // Set a scale box
            tb_Ymax.Text = "0";
            tb_Ymin.Text = "-60";
            tb_Yint.Text = "10";

            // Set a max and min value
            MaxValue = Convert.ToDouble(tb_Ymax.Text);
            MinValue = Convert.ToDouble(tb_Ymin.Text);

            // Use Column for plotting
            chartADC.Series["ADC0"].ChartType = SeriesChartType.Column;
            chartADC.Series["ADC0"].BorderColor = chartADC.Series["ADC0"].Color;
            chartADC.ChartAreas[0].AxisY.Crossing = MinValue;

            chartADC.Series["ADC0Slow"].ChartType = SeriesChartType.Column;
            chartADC.Series["ADC0Slow"].BorderWidth = 10;

            // Put two columns overlapped
            chartADC.Series["ADC0"].CustomProperties = "DrawSideBySide=False";
            chartADC.Series["ADC0Slow"].CustomProperties = "DrawSideBySide=False";

            // Change a chart setting for FFT
            chartADC.Text = "FFT";
            chartADC.ChartAreas[0].AxisX.Title = "Frequency [Hz]";
            chartADC.ChartAreas[0].AxisX.Minimum = 0;
            chartADC.ChartAreas[0].AxisX.Maximum = 2000;
            chartADC.ChartAreas[0].AxisX.Interval = 400;
            chartADC.ChartAreas[0].AxisX.MajorGrid.LineDashStyle = ChartDashStyle.Dash;
            chartADC.ChartAreas[0].AxisX.MajorGrid.Interval = 200;
            chartADC.ChartAreas[0].AxisY.Title = "FFTvalue [dB]";
            chartADC.ChartAreas[0].AxisY.Minimum = -60;
            chartADC.ChartAreas[0].AxisY.Maximum = 0;
            chartADC.ChartAreas[0].AxisY.Interval = 10;
            chartADC.ChartAreas[0].AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Dash;
            chartADC.ChartAreas[0].AxisY.MajorGrid.Interval = 10;

            // Set a max and min value
            MaxValue = Convert.ToDouble(tb_Ymax.Text);
            MinValue = Convert.ToDouble(tb_Ymin.Text);
        }

        private void btnReplot_Click(object sender, EventArgs e)
        {
            try 
            {
                // Set a max and mion value
                MaxValue = Convert.ToDouble(tb_Ymax.Text);
                MinValue = Convert.ToDouble(tb_Ymin.Text);
                chartADC.ChartAreas[0].AxisY.Crossing = MinValue;

                // Case: FFT column graph
                if (cbColormap.Checked != true)
                {
                    chartADC.ChartAreas[0].AxisY.Maximum = MaxValue;
                    chartADC.ChartAreas[0].AxisY.Minimum = MinValue;
                    chartADC.ChartAreas[0].AxisY.Interval = Convert.ToDouble(tb_Yint.Text);
                    chartADC.ChartAreas[0].AxisY.MajorGrid.Interval = Convert.ToDouble(tb_Yint.Text);
                }
            }
            catch { }
        }

        private void cbColormap_CheckedChanged(object sender, EventArgs e)
        {
            if (cbColormap.Checked != true)
            {
                // Activate and deactivate buttons and colormap scale
                lbl_Yint.Enabled = true;
                tb_Yint.Enabled = true;
                pbColorMapScale.Visible = false;

                // Initiailze a chart background
                chartADC.ChartAreas[0].BackImage = "";
                chartADC.ChartAreas[0].BackColor = Color.White;

                // Change a chart setting for FFT
                chartADC.Text = "FFT";
                chartADC.ChartAreas[0].AxisX.Title = "Frequency [Hz]";
                chartADC.ChartAreas[0].AxisX.Minimum = 0;
                chartADC.ChartAreas[0].AxisX.Maximum = 2000;
                chartADC.ChartAreas[0].AxisX.Interval = 400;
                chartADC.ChartAreas[0].AxisX.MajorGrid.LineDashStyle = ChartDashStyle.Dash;
                chartADC.ChartAreas[0].AxisX.MajorGrid.Interval = 200;
                if (cbAweight.Checked == true)
                    chartADC.ChartAreas[0].AxisY.Title = "FFTvalue [dBA]";
                else
                    chartADC.ChartAreas[0].AxisY.Title = "FFTvalue [dB]";
                chartADC.ChartAreas[0].AxisY.Minimum = -60;
                chartADC.ChartAreas[0].AxisY.Maximum = 0;
                chartADC.ChartAreas[0].AxisY.Interval = 10;
                chartADC.ChartAreas[0].AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Dash;
                chartADC.ChartAreas[0].AxisY.MajorGrid.Interval = 10;
                
                // Set a scale box
                tb_Ymax.Text = "0";
                tb_Ymin.Text = "-60";
                tb_Yint.Text = "10";

                // Set a max and mion value
                MaxValue = Convert.ToDouble(tb_Ymax.Text);
                MinValue = Convert.ToDouble(tb_Ymin.Text);
                chartADC.ChartAreas[0].AxisY.Crossing = MinValue;
            }
            else
            {
                // Activate and deactivate buttons
                lbl_Yint.Enabled = false;
                tb_Yint.Enabled = false;

                // Initiailze a color map scale
                pbColorMapScale.Visible = true;

                // Create a new memory bitmap the same size as the picture box
                Bitmap mapColorScale = new Bitmap(pbColorMapScale.Width, pbColorMapScale.Height);

                // Create a graphsics object from our memory bitmap, so we can draw on it and clear
                Graphics Canvas = Graphics.FromImage(mapColorScale);

                // Create a rectangle area
                Rectangle RectColorScale = new Rectangle(0, 0, pbColorMapScale.Width, pbColorMapScale.Height);

                // Create a new color belnd to tell the LinearGradientBrush what colors to use and where to put them
                LinearGradientBrush LinearBrush = new LinearGradientBrush(RectColorScale, Color.Black, Color.White, LinearGradientMode.Vertical);

                // Pass off color blend to LinearGradientBrush to instruct it how to generate the gradient
                Canvas.FillRectangle(LinearBrush, RectColorScale);

                // Copy the Bitmap into the image of picture box
                pbColorMapScale.Image = ColorMapClass.Colorize(mapColorScale, 255, JetColorMap);

                // Get the pixel position of real plotting chart area
                double PixelXmin = chartADC.ChartAreas[0].AxisX.ValueToPixelPosition(chartADC.ChartAreas[0].AxisX.Minimum);
                double PixelXmax = chartADC.ChartAreas[0].AxisX.ValueToPixelPosition(chartADC.ChartAreas[0].AxisX.Maximum);
                double PixelYmin = chartADC.ChartAreas[0].AxisY.ValueToPixelPosition(chartADC.ChartAreas[0].AxisY.Minimum);
                double PixelYmax = chartADC.ChartAreas[0].AxisY.ValueToPixelPosition(chartADC.ChartAreas[0].AxisY.Maximum);

                // Get the size of bitmap in terms of pixel number
                bMapWidth = (int) Math.Abs(PixelXmax - PixelXmin);
                bMapHeight = (int) Math.Abs(PixelYmax - PixelYmin);

                // Change a chart setting for FFT
                chartADC.Text = "Color map";
                chartADC.ChartAreas[0].AxisX.Title = "Time [sec]";
                chartADC.ChartAreas[0].AxisX.Minimum = 0;
                chartADC.ChartAreas[0].AxisX.Maximum = (NFFT/Fs)* bMapWidth/dxScroll;
                chartADC.ChartAreas[0].AxisX.Interval = 1;
                chartADC.ChartAreas[0].AxisX.MajorGrid.LineDashStyle = ChartDashStyle.Dash;
                chartADC.ChartAreas[0].AxisX.MajorGrid.Interval = 2;
                chartADC.ChartAreas[0].AxisY.Title = "Frequency [Hz]";
                chartADC.ChartAreas[0].AxisY.Minimum = 0;
                chartADC.ChartAreas[0].AxisY.Maximum = 2000;
                chartADC.ChartAreas[0].AxisY.Interval = 400;
                chartADC.ChartAreas[0].AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Dash;
                chartADC.ChartAreas[0].AxisY.MajorGrid.Interval = 200;

                // Set a scale box
                tb_Ymax.Text = "0";
                tb_Ymin.Text = "-60";
                tb_Yint.Text = "10";

                // Create a new memory bitmap the same size as the picture box
                Bitmap bMap = new Bitmap((int)Math.Abs(PixelXmax - PixelXmin), (int)Math.Abs(PixelYmax - PixelYmin));
                bMap = ColorMapClass.Colorize(bMap, 255, JetColorMap);

                // Draw a backimage of Chart background
                if (chartADC.Images.Count != 0)
                    chartADC.Images.RemoveAt(0);

                chartADC.Images.Add(new NamedImage("ColorMapImage", bMap));
                chartADC.ChartAreas[0].BackImage = chartADC.Images[0].Name;
                chartADC.ChartAreas[0].BackImageWrapMode = ChartImageWrapMode.Scaled;
 
            }
        }
    }
}
