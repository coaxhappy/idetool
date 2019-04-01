using System.Windows;
using Telerik.Windows.Controls;

namespace Hami.WPF.IDETool
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            this.InitializeComponent();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            Office2013Palette.LoadPreset(Office2013Palette.ColorVariation.DarkGray);

            MainWindow mainWindow = new MainWindow();
            mainWindow.ShowDialog();
        }
    }
}
