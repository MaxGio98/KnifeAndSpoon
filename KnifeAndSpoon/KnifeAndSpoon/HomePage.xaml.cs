using KnifeAndSpoon.Model;
using Plugin.CloudFirestore;
using Plugin.Connectivity;
using Plugin.FirebaseAuth;
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
        private Utente utente;
        CancellationTokenSource cts;
        public ObservableCollection<Ricetta> Ricette { get; private set; }
        public HomePage()
        {
            isFabsOpen = false;
            InitializeComponent();
            if (CrossFirebaseAuth.Current.Instance.CurrentUser.IsAnonymous)
            {
                favouriteFab.BackgroundColor = Color.FromHex("#aa00000");
                addFab.BackgroundColor = Color.FromHex("#aa00000");
            }
            ICommand refreshCommand = new Command(async () =>
            {
                // IsRefreshing is true
                // Refresh data here
                await RefreshData();
                refreshView.IsRefreshing = false;
            });
            refreshView.Command = refreshCommand;
            LoadUtente();
            LoadRicette();
            LoadLastTen();
            TheCarousel.Position = 0;
            //checkConnection();
        }

        private async void checkConnection()
        {
            if (!CrossConnectivity.Current.IsConnected)
            {
                await DisplayAlert("aeeee","ehhh",null);
            }
            CrossConnectivity.Current.ConnectivityChanged += async (sender, agrs) =>
            {
                if (!CrossConnectivity.Current.IsConnected)
                {
                    await DisplayAlert("aeeee", "ehhh", "oooh");
                }
                else
                {
                    Navigation.PopModalAsync();
                }
            };
        }

        private void DisplayAlert(string v)
        {
            throw new NotImplementedException();
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
                try
                {
                    if (cts!=null)
                    {
                        cts.Cancel();
                        cts = null;
                    }
                    cts = new CancellationTokenSource();
                    Task.Run(async () =>
                    {
                        await Task.Delay(5000,cts.Token);
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
                    },cts.Token);
                }
                catch (OperationCanceledException e)
                {}
                catch (Exception e)
                {}

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
                if (cts == null)
                {
                    cts.Cancel();
                    cts = null;
                }
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

        public void OpenRicettaById(object sender, EventArgs args)
        {
            String value = ((Button)sender).CommandParameter.ToString();
            ObservableCollection<Ricetta> temp = (ObservableCollection<Ricetta>)BindableLayout.GetItemsSource(LastTenRecipes);
            for (int i = 0; i < temp.Count; i++)
            {
                if (temp[i].Id.Equals(value))
                {
                    PushPage(new ShowPage((Ricetta)temp[i], "Show", utente));
                }
            }

        }

        public void OpenRicetta(object sender, EventArgs args)
        {
            String id = ((ImageButton)sender).CommandParameter.ToString();
            for (int i = 0; i < Ricette.Count; i++)
            {
                if (Ricette[i].Id.Equals(id))
                {
                    PushPage(new ShowPage(Ricette[i], "Show", utente));
                }
            }
        }

        public void SettingsRedirect(object sender, EventArgs args)
        {
            if (CrossFirebaseAuth.Current.Instance.CurrentUser.IsAnonymous)
            {
                SettingsPage page = new SettingsPage("Normal", utente);
                PushPage(page);
            }
            else
            {
                if (utente.isAdmin)
                {
                    SettingsPage page = new SettingsPage("Admin", utente);
                    page.enableBackReturn(new Command(() => {
                        LoadUtente();
                    }));
                    PushPage(page);
                }
                else
                {
                    SettingsPage page = new SettingsPage("Normal", utente);
                    page.enableBackReturn(new Command(() => {
                        LoadUtente();
                    }));
                    PushPage(page);
                }
            }
        }

        public void openFavorite(object sender, EventArgs args)
        {
            if (CrossFirebaseAuth.Current.Instance.CurrentUser.IsAnonymous)
            {
                Navigation.PushModalAsync(new ErrorDialog("Questa funziona è disponibile solo per chi è registrato"));
            }
            else
            {
                PushPage(new FavouritePage(utente));
            }
        }

        public void SearchRedirect(object sender, EventArgs args)
        {
            PushPage(new SearchPage(utente));
        }

        public void AddRedirect(object sender, EventArgs args)
        {
            if (CrossFirebaseAuth.Current.Instance.CurrentUser.IsAnonymous)
            {
                Navigation.PushModalAsync(new ErrorDialog("Questa funziona è disponibile solo per chi è registrato"));
            }
            else
            {
                PushPage(new InsertPage(utente));
            }
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
               WhereEqualsTo("Categoria", category).
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
               WhereEqualsTo("isApproved", true).
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
               OrderBy("Timestamp", true).
               LimitTo(10).
               GetDocumentsAsync();
            List<Ricetta> ricette = group.ToObjects<Ricetta>().ToList();
            Ricette = new ObservableCollection<Ricetta>(ricette);
            BindableLayout.SetItemsSource(LastTenRecipes, Ricette);
        }

        public async Task LoadUtente()
        {
            var glob = await CrossCloudFirestore.Current.Instance.GetCollection("Utenti").WhereEqualsTo("Mail", CrossFirebaseAuth.Current.Instance.CurrentUser.Email).GetDocumentsAsync();
            List<Utente> list = glob.ToObjects<Utente>().ToList();
            userName.Text = list[0].Nome;
            ImgUtente.Source = list[0].Immagine;
            this.utente = list[0];
        }

        protected override bool OnBackButtonPressed()
        {
            var time = System.DateTime.Now;
            //Are you sure?
            Navigation.PushModalAsync(new ConfirmDialog("Sei sicuro di voler uscire?", new Command(async () =>
            {
                await Navigation.PopModalAsync();
                await Navigation.PopAsync();
            })));
            return true;
        }
    }
}