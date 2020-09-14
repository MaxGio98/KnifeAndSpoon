using KnifeAndSpoon.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace KnifeAndSpoon
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsPage : ContentPage
    {
        List<Utente> list;
        public SettingsPage(string mode, List<Utente> list)
        {
            InitializeComponent();
            this.list = list;
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
            PushPage(new ApprovePage(list));
        }

        public async void PushPage(ContentPage page)
        {
            await Navigation.PushAsync(page);
        }
    }
}