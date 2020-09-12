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
        public SettingsPage(string mode)
        {
            InitializeComponent();
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
            PushPage(new ApprovePage());
        }

        public async void PushPage(ContentPage page)
        {
            await Navigation.PushAsync(page);
        }
    }
}