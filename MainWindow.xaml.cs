//------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Samples.Kinect.InfraredBasics
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using Microsoft.Kinect;
    using System.Diagnostics;
    using System.Threading;
    using System.IO.Ports;
    using System.Linq;




    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SerialPort serialPort;
        string[] serialPorts;
        /// <summary>
        /// Active Kinect sensor
        /// </summary>
        private KinectSensor sensor;

        /// <summary>
        /// Bitmap that will hold color information
        /// </summary>
        private WriteableBitmap colorBitmap;

        /// <summary>
        /// Intermediate storage for the color data received from the camera
        /// </summary>
        private byte[] colorPixels;


        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            InitializeSerialPorts();
        }

        class Global
        {
            public static string results;

        }

        /// <summary>
        /// Execute startup tasks
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void WindowLoaded(object sender, RoutedEventArgs e)
        {

            // Look through all sensors and start the first connected one.
            // This requires that a Kinect is connected at the time of app startup.
            // To make your app robust against plug/unplug, 
            // it is recommended to use KinectSensorChooser provided in Microsoft.Kinect.Toolkit (See components in Toolkit Browser).
            foreach (var potentialSensor in KinectSensor.KinectSensors)
            {
                if (potentialSensor.Status == KinectStatus.Connected)
                {
                    this.sensor = potentialSensor;
                    break;
                }
            }



            if (null != this.sensor)
            {
                // Turn on the color stream to receive color frames
                this.sensor.ColorStream.Enable(ColorImageFormat.InfraredResolution640x480Fps30);

                // Allocate space to put the pixels we'll receive
                this.colorPixels = new byte[this.sensor.ColorStream.FramePixelDataLength];

                // This is the bitmap we'll display on-screen
                this.colorBitmap = new WriteableBitmap(this.sensor.ColorStream.FrameWidth, this.sensor.ColorStream.FrameHeight, 96.0, 96.0, PixelFormats.Gray16, null);

                // Set the image we display to point to the bitmap where we'll put the image data
                this.Image.Source = this.colorBitmap;

                // Add an event handler to be called whenever there is new color frame data
                this.sensor.ColorFrameReady += this.SensorColorFrameReady;

                // Start the sensor!
                try
                {
                    this.sensor.Start();
                }
                catch (IOException)
                {
                    this.sensor = null;
                }
            }

            if (null == this.sensor)
            {
                this.statusBarText.Text = Properties.Resources.NoKinectReady;
            }


        }

        /// <summary>
        /// Execute shutdown tasks
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (null != this.sensor)
            {
                this.sensor.Stop();
            }
        }



        /// <summary>
        /// Event handler for Kinect sensor's ColorFrameReady event
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void SensorColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            using (ColorImageFrame colorFrame = e.OpenColorImageFrame())
            {
                if (colorFrame != null)
                {
                    // Copy the pixel data from the image to a temporary array
                    colorFrame.CopyPixelDataTo(this.colorPixels);

                    // Write the pixel data into our bitmap
                    this.colorBitmap.WritePixels(
                        new Int32Rect(0, 0, this.colorBitmap.PixelWidth, this.colorBitmap.PixelHeight),
                        this.colorPixels,
                        this.colorBitmap.PixelWidth * colorFrame.BytesPerPixel,
                        0);
                }
            }
        }



        // feeds data to a python script for processing
        private void ImageProcessing()
        {
            // create process info
            var ImageProcessor = new ProcessStartInfo();
            ImageProcessor.FileName = @"C:\Users\1089510\AppData\Local\Microsoft\WindowsApps\python3.exe";

            // Provide Script and arguments
            // give script location
            var script = @"Python\Brightest_point_detect.py";
            // give inputs to script
            var pysource = @"Images\For_Analysis\IR_Unaltered.png";
            var pyrad = "11";

            //combine
            ImageProcessor.Arguments = $"\"{script}\" -i \"{pysource}\" -r {pyrad} ";

            //Console.WriteLine(ImageProcessor.Arguments);
            // process config
            ImageProcessor.UseShellExecute = false;

            //dont make a new cmd window
            ImageProcessor.CreateNoWindow = true;


            // redirect stdout and stderror from python back to c#
            ImageProcessor.RedirectStandardOutput = true;
            ImageProcessor.RedirectStandardError = true;

            //execute process to receive output
            var errors = "";
            Global.results = "";


            using (var process = Process.Start(ImageProcessor))
            { 
                errors = process.StandardError.ReadToEnd();
                Global.results = process.StandardOutput.ReadToEnd();
            }
            // output



            OutputBox.Text = Global.results;
            MouseMover();
            
        }

        private void MouseMover()
        {
            // create process info
            var MouseProcessing = new ProcessStartInfo();
            MouseProcessing.FileName = @"C:\Users\1089510\AppData\Local\Microsoft\WindowsApps\python3.exe";

            // Provide Script and arguments
            // give script location
            var script = @"Python\Mousemover.py";
            // give inputs to script



            Global.results = Global.results.Remove(Global.results.Length - 3, 3);
            Global.results = Global.results.Remove(0, 1);
            //Int32 splat = 2; hello geive i was here 2023 code tag break
            Int32 splat = 2;
            String[] separator = { ", ", "(", ")\r\n" };
            string[] res = Global.results.Split(separator, splat,
               StringSplitOptions.RemoveEmptyEntries);

            //combine
            MouseProcessing.Arguments = $"\"{script}\" -xcoor \"{res[0]}\" -ycoor {res[1]} ";

            //Console.WriteLine(MouseProcessing.Arguments);
            // process config
            MouseProcessing.UseShellExecute = false;

            //dont make a new cmd window
            MouseProcessing.CreateNoWindow = true;


            // redirect stdout and stderror from python back to c#
            MouseProcessing.RedirectStandardOutput = true;
            MouseProcessing.RedirectStandardError = true;

            //execute process to receive output
            var errors = "";
            var results = "";


            using (var process = Process.Start(MouseProcessing))
            {
                errors = process.StandardError.ReadToEnd();
                results = process.StandardOutput.ReadToEnd();
            }
            // output



            OutputBox.Text = results;
        }

        private void InitializeSerialPorts()
        {
            serialPorts = SerialPort.GetPortNames();
            if (serialPorts.Count() != 0)
            {
                foreach (string serial in serialPorts)
                {
                    var serialItems = SerialPortNamesCmbBox.Items;
                    if (!serialItems.Contains(serial)) // if the serial is not yet inside the combobox 
                    {
                        SerialPortNamesCmbBox.Items.Add(serial);  // add a serial port name to combo box
                    }
                }
                SerialPortNamesCmbBox.SelectedItem = serialPorts[0];// combobox as default selected item 
            }
        }

        #region WPF to Arduino connection
        bool isConnectedToArduino = false;
        private void ConnectToArduino()
        {
            try
            {
                string selectedSerialPort = SerialPortNamesCmbBox.SelectedItem.ToString(); // gets the selected port
                serialPort = new SerialPort(selectedSerialPort, 9600);
                serialPort.Open();
                SerialPortConnectBtn.Content = "Disconnect";
                isConnectedToArduino = true;
                MessageBox.Show($"connetion to {selectedSerialPort} was succesful", "Succesful Connection", MessageBoxButton.OK, MessageBoxImage.Information);
                serialPort.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(port_DataReceived_1);
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("The selected serial port is busy!", "Busy", MessageBoxButton.OK, MessageBoxImage.Stop);
            }
            catch (NullReferenceException)
            {
                MessageBox.Show("There is no serial port!", "Empty Serial Port", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        private void DisconnectFromArduino()
        {
            SerialPortConnectBtn.Content = "Connect";

            isConnectedToArduino = false;
            ResetControl();
            serialPort.Close();
        }

        private void ResetControl()
        {

        }

        private void SerialPortConnectBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!isConnectedToArduino)
            {
                ConnectToArduino();
            }
            else
            {
                DisconnectFromArduino();
            }
        }
        #endregion


        private void RefreshBtn_Click(object sender, RoutedEventArgs e)
        {
            InitializeSerialPorts();
        }

        private void ButtonScreenshotClick(object sender, RoutedEventArgs e)
        {
            
            if (null == this.sensor)
            {
                this.statusBarText.Text = Properties.Resources.ConnectDeviceFirst;
                //return;
            }

            // create a png bitmap encoder which knows how to save a .png file
            BitmapEncoder encoder = new PngBitmapEncoder();

            // create frame from the writable bitmap and add to encoder
            encoder.Frames.Add(BitmapFrame.Create(this.colorBitmap));

            string time = System.DateTime.Now.ToString("hh'-'mm'-'ss", CultureInfo.CurrentUICulture.DateTimeFormat);

            string myPhotos = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);

            string StorageLoc = @"Images\For_Analysis\";

            string path = Path.Combine(StorageLoc, "IR_Unaltered" + ".png");




            // write the new file to disk
            try
            {
                using (FileStream fs = new FileStream(path, FileMode.Create))
                {
                    encoder.Save(fs);
                }


            }
            catch (IOException)
            {

            }
            ImageProcessing();
            MessageBox.Show($"brightest point at {Global.results}", "Brightest Point", MessageBoxButton.OK, MessageBoxImage.Information);

            // broken code for displaying the screenshot (is unnecessary)


        }//
        string InputData;

        private void port_DataReceived_1(object sender, SerialDataReceivedEventArgs e)
        {
            InputData = serialPort.ReadLine();
            
            if (InputData == "run")
            {
                ImageProcessing();
            }
            

        }
    }
}


