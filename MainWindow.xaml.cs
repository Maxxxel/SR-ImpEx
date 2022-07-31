using Microsoft.Win32;
using SR_ImpEx.Helpers;
using SR_ImpEx.Logger;
using SR_ImpEx.Structures;
using SR_ImpEx.Structures.GLTFFile;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Numerics;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace SR_ImpEx
{
    public partial class MainWindow : Window
    {
        public DRS DRSFile { get; private set; }
        public GLTF GLTFFile { get; private set; }
        //AnimationFinder AnimationFinder = new AnimationFinder();
        public OpenFileDialog OpenFileDialog { get; private set; }
        public static int index { get; private set; }
        public static ObservableCollection<LogEntry> LogEntries { get; set; }
        public static string defaultModelSize { get; private set; }
        public static Matrix4x4 Scale { get; private set; }
        public Boolean SizeChanged { get; private set; }
        Exporter Exporter = new Exporter();
        //private HashSet<string> animations;
        private bool AutoScroll = true;
        public MainWindow()
        {
            if (null == Application.Current)
            {
                new Application();
            }

            InitializeComponent();
            ClearData();
            DataContext = LogEntries = new ObservableCollection<LogEntry>();
            LogMessage("Welcome to SR ImpEx. Please start by importing a file.");
        }
        private void ImportFile(object sender, RoutedEventArgs e)
        {
            ClearData();
            OpenFileDialog = new OpenFileDialog();
            // *.bmg files are currently not supported!
            // *.drs files are planed to be supported in the future.
            // OpenFileDialog.Filter = "All files|*.glb;*.gltf;*.drs|gLTF|*.glb;*.gltf|Battleforge DRS|*.drs";
            OpenFileDialog.Filter = "All files|*.glb;*.gltf|gLTF|*.glb;*.gltf";

            if (OpenFileDialog.ShowDialog() == true)
            {
                string Extension = Path.GetExtension(OpenFileDialog.FileName).ToUpperInvariant();

                if (Extension == ".GLB" || Extension == ".GLTF")
                {
                    LogMessage("[INFO] Reading a " + Extension + " file...");
                    ImportGLTFFile(OpenFileDialog.FileName);
                }
            }
        }
        private void ClearData()
        {
            Model_Name.Text = "Please click 'Import File' to select a file.";
            Scale = Matrix4x4.CreateScale(1);
        }
        //private void ImportDRSFile(string fileName)
        //{
        //    FileWrapper File = new FileWrapper(fileName);
        //    DRSFile = new DRS(File);
        //    DRSFile.Location = fileName.Replace(OpenFileDialog.SafeFileName, "");
        //    if (DRSFile.AnimationSet != null) animations = AnimationFinder.FindModeAnimationKeys(DRSFile);
        //    FillData();
        //    LogMessage("[INFO] File read successfully.");
        //}
        private void FillData()
        {
            Model_Name.Text = OpenFileDialog.SafeFileName;

            if (GLTFFile != null) Model_Size.Text = GLTFFile.ModelSize.ToString();
            if (DRSFile != null) Model_Size.Text = DRSFile.ModelSize.ToString();

            // Set the default size
            defaultModelSize = Model_Size.Text;
        }
        private void ImportGLTFFile(string filePath)
        {
            // Read the file into a virtual GLTF object
            GLTFFile = new GLTF(filePath);
            // Fill the Interface with the data
            FillData();
            LogMessage("[INFO] File read successfully.");
            ExportButton.Visibility = Visibility.Visible;
        }
        private void ExportFile(object sender, RoutedEventArgs e)
        {
            if (OpenFileDialog != null)
            {
                string Extension = Path.GetExtension(OpenFileDialog.FileName).ToUpperInvariant();

                //if (Extension == ".DRS")
                //{
                //    LogMessage($"[INFO] Exporting {OpenFileDialog.SafeFileName}...");
                //    ExportDRSFile(OpenFileDialog.SafeFileName.Replace(Extension, ""));
                //}
                
                if (Extension == ".GLB" || Extension == ".GLTF")
                {
                    LogMessage($"[INFO] Exporting {OpenFileDialog.SafeFileName}...");
                    ExportGLTFFile(OpenFileDialog.SafeFileName.Replace(Extension, ""));
                }
            }
        }
        private void ExportGLTFFile(string fileName)
        {
            if (GLTFFile != null)
            {
                // Set the Scale of the model
                if (SizeChanged)
                {
                    GLTFFile.Root.ApplyBasisTransform(Scale);
                    SizeChanged = false;
                }
                // Export the scaled Model to a GLTF file for debugging
                //GLTFFile.Root.SaveGLB(fileName);
                // Create the Static Mesh
                GLTFFile.staticMesh = new StaticMesh(GLTFFile.Root);
                // Create the DRS file from the GLTF file
                Thread Worker = new Thread(() => Exporter.ExportToDRS(fileName, GLTFFile));
                Worker.Start();
            }
        }
        //private void ExportDRSFile(string fileName)
        //{
        //    if (DRSFile != null)
        //    {
        //        Thread Worker = new Thread(() => Exporter.ExportToGLTF(fileName, DRSFile, animations));
        //        Worker.Start();
        //    }
        //}
        private void OpenExportFolder(object sender, RoutedEventArgs e)
        {
            string CurrentDir = AppContext.BaseDirectory;

            if (!Directory.Exists(Path.Combine(CurrentDir, "Exports"))) Directory.CreateDirectory(Path.Combine(CurrentDir, "Exports"));

            Process.Start(new ProcessStartInfo()
            {
                FileName = CurrentDir + "\\Exports",
                UseShellExecute = true,
                Verb = "open"
            });
        }
        private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            // User scroll event : set or unset autoscroll mode
            if (e.ExtentHeightChange == 0)
            {   // Content unchanged : user scroll event
                if ((e.Source as ScrollViewer).VerticalOffset == (e.Source as ScrollViewer).ScrollableHeight)
                {   // Scroll bar is in bottom
                    // Set autoscroll mode
                    AutoScroll = true;
                }
                else
                {   // Scroll bar isn't in bottom
                    // Unset autoscroll mode
                    AutoScroll = false;
                }
            }

            // Content scroll event : autoscroll eventually
            if (AutoScroll && e.ExtentHeightChange != 0)
            {   // Content changed and autoscroll mode set
                // Autoscroll
                (e.Source as ScrollViewer).ScrollToVerticalOffset((e.Source as ScrollViewer).ExtentHeight);
            }
        }
        public static void LogMessage(string msg)
        {
            LogEntry E = new LogEntry()
            {
                Index = index++,
                DateTime = DateTime.Now.ToString("HH:mm:ss"),
                Message = msg
            };

            Application.Current.Dispatcher.BeginInvoke((Action)(() => LogEntries.Add(E)));
        }
        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            // Reset the text to the default value
            Model_Size.Text = defaultModelSize;
            var CurrentSize = GLTFFile.GetModelSize();
            var Dif = float.Parse(Model_Size.Text) / CurrentSize;
            Scale = Matrix4x4.CreateScale(Dif);
            SizeChanged = true;            

            // Print a debug message to the log
            LogMessage("[INFO] Model size reset to default value. Hit the 'Export' button to apply the change.");
        }
        private void Change_Click(object sender, RoutedEventArgs e)
        {
            // Replace '.' with ',' in the textbox
            Model_Size.Text = Model_Size.Text.Replace('.', ',');
            var CurrentSize = GLTFFile.GetModelSize();
            var NewSize = float.Parse(Model_Size.Text);
            var Dif = NewSize / CurrentSize;
            LogMessage("[INFO] Scaling the model Default Height: (" + CurrentSize + ") New Height: (" + NewSize + ")");

            Scale = Matrix4x4.CreateScale(Dif);
            SizeChanged = true;
            LogMessage($"[INFO] Model size changed to {Model_Size.Text}. Hit the 'Export' button to apply the change.");
        }
    }
}
