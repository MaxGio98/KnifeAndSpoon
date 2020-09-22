using Plugin.Connectivity;
using Plugin.DeviceOrientation;
using Plugin.DeviceOrientation.Abstractions;
using System;
using Xamarin.Forms;

namespace KnifeAndSpoon
{
    public partial class App : Application
    {
        private Boolean connection;
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

        //controlla connessione attuale
        private void checkInitialConnection()
        {
            if (!CrossConnectivity.Current.IsConnected)
            {
                var popup = new CustomBox("Attenzione", "Nessuna connessione");
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

        //controlla cambio di connessione
        private void checkConnection()
        {
            CrossConnectivity.Current.ConnectivityChanged += async (sender, agrs) =>
            {
                if (!CrossConnectivity.Current.IsConnected)
                {
                    if (!connection)
                    {
                        var popup = new CustomBox("Attenzione", "Nessuna connessione");
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
