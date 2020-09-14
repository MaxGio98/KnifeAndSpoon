using KnifeAndSpoon.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plugin.FirebaseAuth;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Plugin.GoogleClient;

namespace KnifeAndSpoon
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsPage : ContentPage
    {
        Utente utente;
        public SettingsPage(string mode, Utente usr)
        {
            InitializeComponent();
            utente = usr;
            if (mode == "Admin")
            {
                approve.IsEnabled = true;
                approve.IsVisible = true;
            }
        }

        public void Back(object sender, EventArgs args)
        {
            Navigation.PopAsync();
        }

        public void Approve(object sender, EventArgs args)
        {
            PushPage(new ApprovePage(utente));
        }

        public void LogOut(object sender, EventArgs args)
        {
            CrossFirebaseAuth.Current.Instance.SignOut();
            CrossGoogleClient.Current.Logout();
            App.Current.MainPage = new NavigationPage(new MainPage());
        }

        public async void PushPage(ContentPage page)
        {
            await Navigation.PushAsync(page);
        }
    }
}