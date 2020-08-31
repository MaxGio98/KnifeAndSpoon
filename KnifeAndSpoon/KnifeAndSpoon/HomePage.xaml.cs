using KnifeAndSpoon.Model;
using Plugin.CloudFirestore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
        readonly Task autoCloseFabs;
        public ObservableCollection<Ricetta> Ricette { get; private set; }
        public HomePage()
        {
            isFabsOpen = false;
            InitializeComponent();
            LoadRicette();
        }


        public void OpenFabs(object sender, EventArgs args)
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

        async Task CloseFabsWithWait()
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

        public void OpenRicetta(object sender, EventArgs args)
        {
            PushPage(new ShowPage(Ricette[TheCarousel.Position],"Show"));
        }

        public void AddRedirect(object sender, EventArgs args)
        {
            PushPage(new InsertPage());
        }

        public async void PushPage(ContentPage page)
        {
            await Navigation.PushAsync(page);
        }


        public async Task<IEnumerable<Ricetta>> GetRicetta()
        {
            var group = await CrossCloudFirestore.Current.
               Instance.
               GetCollection("Ricette").
               GetDocumentsAsync();
            List<Ricetta> ricetta = group.ToObjects<Ricetta>().ToList();

            Debug.WriteLine(ricetta[0].NumeroPersone);
            Debug.WriteLine(ricetta[0].Passaggi.Count);
            return ricetta;
        }

        public async Task LoadRicette()
        {
            var group = await CrossCloudFirestore.Current.
               Instance.
               GetCollection("Ricette").
               GetDocumentsAsync();
            List<Ricetta> ricette = group.ToObjects<Ricetta>().ToList();
            Ricette = new ObservableCollection<Ricetta>(ricette);
            TheCarousel.ItemsSource = Ricette;
        }
    }
}