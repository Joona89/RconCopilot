using Rcon.Ui.Views;

namespace Rcon
{
    // App.xaml.cs
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            MainPage = new NavigationPage(new MainTabbedPage());
        }
    }

}
