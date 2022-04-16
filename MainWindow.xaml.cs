using Microsoft.Win32;
using SR_ImpEx.Helpers;
using SR_ImpEx.Logger;
using SR_ImpEx.Structures;
using SR_ImpEx.Structures.GLTFFile;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
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
        AnimationFinder AnimationFinder = new AnimationFinder();
        public OpenFileDialog OpenFileDialog { get; private set; }
        public static int index { get; private set; }
        public static ObservableCollection<LogEntry> LogEntries { get; set; }
        Exporter Exporter = new Exporter();
        private HashSet<string> animations;
        private bool AutoScroll = true;

        private MainWindow()
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
        [STAThreadAttribute]
        public static void Main()
        {
            AppDomain.CurrentDomain.AssemblyResolve += OnResolveAssembly;
            MainWindow mainWindow = new MainWindow();
            mainWindow.ShowDialog();
        }
        private void ImportFile(object sender, RoutedEventArgs e)
        {
            ClearData();
            OpenFileDialog = new OpenFileDialog();
            // *.bmg files are currently not supported!
            OpenFileDialog.Filter = "All files|*.glb;*.gltf;*.drs|gLTF|*.glb;*.gltf|Battleforge DRS|*.drs";

            if (OpenFileDialog.ShowDialog() == true)
            {
                string Extension = Path.GetExtension(OpenFileDialog.FileName).ToUpperInvariant();

                if (Extension == ".DRS")
                {
                    LogMessage("[INFO] Reading file...");
                    ImportDRSFile(OpenFileDialog.FileName);
                }
                else if (Extension == ".GLB" || Extension == ".GLTF")
                {
                    LogMessage("[INFO] Reading file...");
                    ImportGLTFFile(OpenFileDialog.FileName);
                }
            }
        }
        private void ClearData()
        {
            Model_Name.Text = "Please import a file";
        }
        private void ImportDRSFile(string fileName)
        {
            FileWrapper File = new FileWrapper(fileName);
            DRSFile = new DRS(File);
            DRSFile.Location = fileName.Replace(OpenFileDialog.SafeFileName, "");
            if (DRSFile.AnimationSet != null) animations = AnimationFinder.FindModeAnimationKeys(DRSFile);
            FillData();
            LogMessage("[INFO] File read successfully.");
        }
        private void FillData()
        {
            Model_Name.Text = OpenFileDialog.SafeFileName;
        }
        private void ImportGLTFFile(string filePath)
        {
            GLTFFile = new GLTF(filePath);
            FillData();
            LogMessage("[INFO] File read successfully.");
        }
        private void ExportFile(object sender, RoutedEventArgs e)
        {
            if (OpenFileDialog != null)
            {
                string Extension = Path.GetExtension(OpenFileDialog.FileName).ToUpperInvariant();

                if (Extension == ".DRS")
                {
                    LogMessage($"[INFO] Exporting {OpenFileDialog.SafeFileName}...");
                    ExportDRSFile(OpenFileDialog.SafeFileName);
                }
                else if (Extension == ".GLB" || Extension == ".GLTF")
                {
                    LogMessage($"[INFO] Exporting {OpenFileDialog.SafeFileName}...");
                    ExportGLTFFile(OpenFileDialog.SafeFileName);
                }
            }
        }
        private void ExportGLTFFile(string fileName)
        {
            if (GLTFFile != null)
            {
                Thread Worker = new Thread(() => Exporter.ExportToDRS(fileName, GLTFFile));
                Worker.Start();
            }
        }
        private void ExportDRSFile(string fileName)
        {
            if (DRSFile != null)
            {
                Thread Worker = new Thread(() => Exporter.ExportToGLTF(fileName, DRSFile, animations));
                Worker.Start();
            }
        }
        private void OpenExportFolder(object sender, RoutedEventArgs e)
        {
            string CurrentDir = AppContext.BaseDirectory;

            if (!Directory.Exists(Path.Combine(CurrentDir, "GLTF_Exports"))) Directory.CreateDirectory(Path.Combine(CurrentDir, "GLTF_Exports"));

            Process.Start(new ProcessStartInfo()
            {
                FileName = CurrentDir + "\\GLTF_Exports",
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
        private static Assembly OnResolveAssembly(object sender, ResolveEventArgs args)
        {
            Assembly executingAssembly = Assembly.GetExecutingAssembly();
            AssemblyName assemblyName = new AssemblyName(args.Name);

            var path = assemblyName.Name + ".dll";
            if (assemblyName.CultureInfo.Equals(CultureInfo.InvariantCulture) == false) path = String.Format(@"{0}\{1}", assemblyName.CultureInfo, path);

            using (Stream stream = executingAssembly.GetManifestResourceStream(path))
            {
                if (stream == null) return null;

                var assemblyRawBytes = new byte[stream.Length];
                stream.Read(assemblyRawBytes, 0, assemblyRawBytes.Length);
                return Assembly.Load(assemblyRawBytes);
            }
        }
    }
}
