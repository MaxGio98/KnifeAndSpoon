using Plugin.CloudFirestore;
using Plugin.Connectivity;
using Plugin.FirebaseAuth;
using Plugin.GoogleClient;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace KnifeAndSpoon
{
    public partial class App : Application
    {
        public App()
        {
            //CrossDeviceOrientation.Current.LockOrientation(DeviceOrientations.Landscape);
            InitializeComponent();
            MainPage = new NavigationPage(new MainPage());
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
