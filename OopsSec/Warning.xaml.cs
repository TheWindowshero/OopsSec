using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;

namespace OopsSec
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class Warning : Window
    {
        public Warning()
        {
            InitializeComponent();
            string html = LoadEmbeddedResource("index.html");
            pageViewer.Navigating += PageViewer_Navigating;
            pageViewer.NavigateToString(html);
        }

        // Open link on default browser
        private void PageViewer_Navigating(object sender, System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            if (e.Uri != null)
            {
                e.Cancel = true;
                Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            }
        }

        // Load embedded resource (only website in this case)
        private string LoadEmbeddedResource(string fileName)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();

            string[] resources = assembly.GetManifestResourceNames();

            string resourceName = resources.FirstOrDefault(r => r.EndsWith(fileName));
            if (resourceName == null)
            {
                throw new FileNotFoundException("The file was not found in the resources.");
            }

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
