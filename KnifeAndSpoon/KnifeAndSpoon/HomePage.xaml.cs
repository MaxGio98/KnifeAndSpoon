using KnifeAndSpoon.Model;
using Plugin.CloudFirestore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace KnifeAndSpoon
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HomePage : ContentPage
    {
        Boolean isFabsOpen;
        readonly Task autoCloseFabs;
        private bool isRefreshing;
        public ObservableCollection<Ricetta> Ricette { get; private set; }
        public HomePage()
        {
            isFabsOpen = false;
            InitializeComponent();
            ICommand refreshCommand = new Command(async () =>
            {
                // IsRefreshing is true
                // Refresh data here
                await RefreshData();
                refreshView.IsRefreshing = false;
            });
            refreshView.Command = refreshCommand;
            LoadRicette();
            LoadLastTen();
        }

        private async Task RefreshData()
        {
            await LoadRicette();
            await LoadLastTen();
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
            Console.WriteLine(TheCarousel.Position.ToString());
            Console.WriteLine(Ricette[TheCarousel.Position].Titolo);
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

        public void DisableFilter()
        {
            checkAntipasto.IsVisible = false;
            checkPrimo.IsVisible = false;
            checkSecondo.IsVisible = false;
            checkContorno.IsVisible = false;
            checkDolce.IsVisible = false;
        }

        public void FilterByAntipasto(object sender, EventArgs args)
        {
            if (checkAntipasto.IsVisible)
            {
                checkAntipasto.IsVisible = false;
                LoadRicette();
            }
            else
            {
                DisableFilter();
                checkAntipasto.IsVisible = true;
                LoadRicetteFilter("Antipasto");
            }
            
        }

        public void FilterByPrimo(object sender, EventArgs args)
        {
            if (checkPrimo.IsVisible)
            {
                checkPrimo.IsVisible = false;
                LoadRicette();
            }
            else
            {
                DisableFilter();
                checkPrimo.IsVisible = true;
                LoadRicetteFilter("Primo");
            }
        }

        public void FilterBySecondo(object sender, EventArgs args)
        {
            if (checkSecondo.IsVisible)
            {
                checkSecondo.IsVisible = false;
                LoadRicette();
            }
            else
            {
                DisableFilter();
                checkSecondo.IsVisible = true;
                LoadRicetteFilter("Secondo");
            }
        }

        public void FilterByContorno(object sender, EventArgs args)
        {
            if (checkContorno.IsVisible)
            {
                checkContorno.IsVisible = false;
                LoadRicette();
            }
            else
            {
                DisableFilter();
                checkContorno.IsVisible = true;
                LoadRicetteFilter("Contorno");
            }
            
        }

        public void FilterByDolce(object sender, EventArgs args)
        {
            if (checkDolce.IsVisible)
            {
                checkDolce.IsVisible = false;
                LoadRicette();
            }
            else
            {
                DisableFilter();
                checkDolce.IsVisible = true;
                LoadRicetteFilter("Dolce");
            }
        }

        public async Task LoadRicetteFilter(string category)
        {
            var group = await CrossCloudFirestore.Current.
               Instance.
               GetCollection("Ricette").
               WhereEqualsTo("isApproved", true).
               WhereEqualsTo("Categoria",category).
               GetDocumentsAsync();
            List<Ricetta> ricette = group.ToObjects<Ricetta>().ToList();
            Ricette = new ObservableCollection<Ricetta>(ricette);
            TheCarousel.ItemsSource = Ricette;
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
               WhereEqualsTo("isApproved",true).
               GetDocumentsAsync();
            List<Ricetta> ricette = group.ToObjects<Ricetta>().ToList();
            Ricette = new ObservableCollection<Ricetta>(ricette);
            TheCarousel.ItemsSource = Ricette;
        }

        public async Task LoadLastTen()
        {
            var group = await CrossCloudFirestore.Current.
               Instance.
               GetCollection("Ricette").
               WhereEqualsTo("isApproved", true).
               OrderBy("Timestamp",true).
               LimitTo(10).
               GetDocumentsAsync();
            List<Ricetta> ricette = group.ToObjects<Ricetta>().ToList();
            Ricette = new ObservableCollection<Ricetta>(ricette);
            BindableLayout.SetItemsSource(LastTenRecipes, Ricette);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}