using Microsoft.Win32;
using System.Windows;
using System;
using System.IO;
using SR_ImpEx.Structures;
using SR_ImpEx.Helpers;
using System.Diagnostics;
using System.Collections.Generic;
using SR_ImpEx.LogViewer;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Threading;

namespace SR_ImpEx
{
    public partial class MainWindow : Window
    {
        public DRS DRSFile { get; private set; }
        AnimationFinder AnimationFinder = new AnimationFinder();
        public OpenFileDialog OpenFileDialog { get; private set; }
        public static ObservableCollection<LogEntry> LogEntries { get; set; }
        public bool AutoScroll { get; private set; }
        Exporter Exporter = new Exporter();
        private HashSet<string> animations;
        private static int index;
        public MainWindow()
        {
            InitializeComponent();
            ClearData();
            DataContext = LogEntries = new ObservableCollection<LogEntry>();
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
                    ImportDRSFile(OpenFileDialog.FileName);
                }
                else if (Extension == ".GLB" || Extension == ".GLTF")
                {
                    ImportGLTFFile(OpenFileDialog.FileName);
                }
            }
        }
        private void ClearData()
        {
            Model_Name.Text = "Please import a file";
            Triangle_Count.Text = "0";
            Has_Skeleton.Text = "-";
            Animation_Count.Text = "0";
        }
        private void ImportDRSFile(string fileName)
        {
            FileWrapper File = new FileWrapper(fileName);
            LogMessage("[INFO] Loading .drs file...");
            DRSFile = new DRS(File);
            DRSFile.Location = fileName.Replace(OpenFileDialog.SafeFileName, "");
            animations = AnimationFinder.FindModeAnimationKeys(DRSFile);
            FillData();
            LogMessage($"[INFO] Successfully loaded file ({OpenFileDialog.SafeFileName})");
        }
        private void FillData()
        {
            Model_Name.Text = OpenFileDialog.SafeFileName;
            Triangle_Count.Text = (DRSFile.CGeoMesh.IndexCount / 3).ToString();
            Has_Skeleton.Text = DRSFile.CSkSkeleton != null ? "True" : "False";
            Animation_Count.Text = animations.Count.ToString();
        }
        private void ImportGLTFFile(string fileName)
        {
            throw new NotImplementedException();
        }
        private void ExportFile(object sender, RoutedEventArgs e)
        {
            if (OpenFileDialog != null)
            {
                string Extension = Path.GetExtension(OpenFileDialog.FileName).ToUpperInvariant();

                if (Extension == ".DRS")
                {
                    ExportDRSFile(OpenFileDialog.SafeFileName);
                }
                else if (Extension == ".GLB" || Extension == ".GLTF")
                {
                    ExportGLTFFile(OpenFileDialog.SafeFileName);
                }
            }
        }
        private void ExportGLTFFile(string fileName)
        {
            throw new NotImplementedException();
        }
        private void ExportDRSFile(string fileName)
        {
            if (DRSFile != null)
            {
                LogMessage("[INFO] Exporting file to GTLF-format.");
                Thread Worker = new Thread(() => Exporter.ExportToGLTF(fileName, DRSFile, animations));
                Worker.Start();
            }
        }
        private void OpenExportFolder(object sender, RoutedEventArgs e)
        {
            string CurrentDir = Directory.GetCurrentDirectory();

            if (!Directory.Exists(Path.Combine(CurrentDir, "GLTF_Exports"))) Directory.CreateDirectory(Path.Combine(CurrentDir, "GLTF_Exports"));

            Process.Start(new ProcessStartInfo()
            {
                FileName = CurrentDir + "\\GLTF_Exports",
                UseShellExecute = true,
                Verb = "open"
            });
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
        private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (e.ExtentHeightChange == 0)
            {
                if ((e.Source as ScrollViewer).VerticalOffset == (e.Source as ScrollViewer).ScrollableHeight)
                {
                    AutoScroll = true;
                }
                else
                {
                    AutoScroll = false;
                }
            }

            if (AutoScroll && e.ExtentHeightChange != 0)
            {
                (e.Source as ScrollViewer).ScrollToVerticalOffset((e.Source as ScrollViewer).ExtentHeight);
            }
        }
    }
}
