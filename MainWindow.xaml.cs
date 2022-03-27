using Microsoft.Win32;
using System.Windows;
using System.IO;

namespace SR_ImpEx
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ImportDRSFile(string fileName)
        {

        }

        private void ImportGLTFFile(string fileName)
        {

        }

        private void ImportFile(object sender, RoutedEventArgs e)
        {
            OpenFileDialog OpenFileDialog = new OpenFileDialog();
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
    }
}
