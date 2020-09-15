using Plugin.CloudFirestore;
using Plugin.Connectivity;
using Plugin.DeviceOrientation;
using Plugin.DeviceOrientation.Abstractions;
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
        Boolean connection;
        public App()
        {
            InitializeComponent();
            CrossDeviceOrientation.Current.LockOrientation(DeviceOrientations.Portrait);
            MainPage = new NavigationPage(new MainPage());
            connection = false;
            checkInitialConnection();
            checkConnection();
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

        private void checkInitialConnection()
        {
            if (!CrossConnectivity.Current.IsConnected)
            {
                var popup = new CustomYesNoBox("Attenzione", "Nessuna connessione");
                popup.PopupClosed += (o, closedArgs) =>
                {
                    checkInitialConnection();
                };
                popup.Show();
                connection = true;
            }
            else
            {
                connection = false;
            }
        }

        private void checkConnection()
        {
            CrossConnectivity.Current.ConnectivityChanged += async (sender, agrs) =>
            {
                if (!CrossConnectivity.Current.IsConnected)
                {
                    if(!connection)
                    {
                        var popup = new CustomYesNoBox("Attenzione", "Nessuna connessione");
                        popup.PopupClosed += (o, closedArgs) =>
                        {
                            checkInitialConnection();
                        };
                        connection = true;
                        popup.Show();
                    }                    
                }
            };
        }
    }
}
