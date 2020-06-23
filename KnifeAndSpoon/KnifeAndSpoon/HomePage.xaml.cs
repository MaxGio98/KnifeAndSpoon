using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace KnifeAndSpoon
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HomePage : ContentPage
    {
        Boolean isFabsOpen;
        Task autoCloseFabs;
        public HomePage()
        {
            isFabsOpen = false;
            InitializeComponent();
        }

        public void openFabs(object sender, EventArgs args)
        {
            if (isFabsOpen == false)
            {
                //Visualizza i pulsanti
                isFabsOpen = !isFabsOpen;
                //autoCloseFabs = closeFabsWithWait;
                mainFab.RotateTo(45, 150);
                settingsFab.TranslateTo(-210, 0, 150);
                settingsFab.FadeTo(255, 150);
                favouriteFab.TranslateTo(-210, 0, 150);
                favouriteFab.FadeTo(255, 150);
                searchFab.TranslateTo(-210, 0, 150);
                searchFab.FadeTo(255, 150);
                addFab.TranslateTo(-210, 0, 150);
                addFab.FadeTo(255, 150);
            }
            else
            {
                //Nascondi i pulsanti
                isFabsOpen = !isFabsOpen;
                mainFab.RotateTo(-45, 150);
                settingsFab.TranslateTo(0, 0, 150);
                settingsFab.FadeTo(0, 150); ;
                favouriteFab.TranslateTo(0, 0, 150);
                favouriteFab.FadeTo(0, 150);
                searchFab.TranslateTo(0, 0, 150);
                searchFab.FadeTo(0, 150);
                addFab.TranslateTo(0, 0, 150);
                addFab.FadeTo(0, 150);
            }
        }

        async Task closeFabsWithWait()
        {
            await Task.Delay(2000);
            isFabsOpen = !isFabsOpen;
            mainFab.RotateTo(-45, 150);
            settingsFab.TranslateTo(0, 0, 150);
            settingsFab.FadeTo(0, 150); ;
            favouriteFab.TranslateTo(0, 0, 150);
            favouriteFab.FadeTo(0, 150);
            searchFab.TranslateTo(0, 0, 150);
            searchFab.FadeTo(0, 150);
            addFab.TranslateTo(0, 0, 150);
            addFab.FadeTo(0, 150);
        }
    }
}