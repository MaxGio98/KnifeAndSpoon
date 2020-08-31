using Plugin.CloudFirestore;
using Plugin.FirebaseAuth;
using Plugin.GoogleClient;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace KnifeAndSpoon
{
    public partial class App : Application
    {
        public App()
        {
            CrossDeviceOrientation.Current.LockOrientation(DeviceOrientations.Landscape);
            InitializeComponent();
            if (CrossFirebaseAuth.Current.Instance.CurrentUser != null)
            {
                checkUser();
            }
            else
            {
                MainPage = new NavigationPage(new MainPage());
            }
        }

        public async void checkUser()
        {
            var glob = await CrossCloudFirestore.Current.Instance.GetCollection("Utenti").WhereEqualsTo("Mail", CrossFirebaseAuth.Current.Instance.CurrentUser.Email).GetDocumentsAsync();
            if (glob.Count == 0)
            {
                //Apertura pagina registrazione
                MainPage = new NavigationPage(new RegisterPage());
            }
            else
            {
                //Apertura pagina principale
                MainPage = new NavigationPage(new HomePage());
            }
            
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
