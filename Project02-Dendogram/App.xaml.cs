using System.Windows;
using Project02_Dendogram.Presentation;

namespace Project02_Dendogram
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var mainWindow = new ClusteringJerarquico();
            mainWindow.Show();
        }
    }
}