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
            InitializeComponent();
            if (CrossGoogleClient.Current.CurrentUser != null)
            {
                MainPage = new NavigationPage(new HomePage());
            }
            else
            {
                MainPage = new NavigationPage(new MainPage());
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
