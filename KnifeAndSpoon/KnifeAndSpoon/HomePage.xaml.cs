using KnifeAndSpoon.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        readonly IList<Ricetta> source;
        public ObservableCollection<Ricetta> Ricette { get; private set; }
        public HomePage()
        {
            isFabsOpen = false;
            InitializeComponent();
            source = new List<Ricetta>();
            source.Add(new Ricetta
            {
                Titolo = "PASTAH",
                ImageUrl = "https://firebasestorage.googleapis.com/v0/b/knifeandspoon-3ac35.appspot.com/o/38c4468c-038e-4f4a-9a21-4cf3f915da2c.jpg?alt=media&token=248808c6-a2b0-47f0-bdbd-8ef0a21f9c4b"
            });
            Ricette = new ObservableCollection<Ricetta>(source);
            TheCarousel.ItemsSource = Ricette;
        }

        public void openFabs(object sender, EventArgs args)
        {
            if (isFabsOpen == false)
            {
                //Visualizza i pulsanti
                isFabsOpen = !isFabsOpen;
                Device.StartTimer(new TimeSpan(0, 0, 10), () =>
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
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
                    });
                    return false;
                });
                
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

        public void openRicetta(object sender, EventArgs args)
        {
            Console.WriteLine(TheCarousel.Position);
        }

        public void addRedirect(object sender, EventArgs args)
        {
            Navigation.PushAsync(new InsertPage());
        }
    }
}