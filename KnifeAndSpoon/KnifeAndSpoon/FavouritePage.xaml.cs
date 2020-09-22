using KnifeAndSpoon.Model;
using Plugin.CloudFirestore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace KnifeAndSpoon
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FavouritePage : ContentPage
    {
        private Utente utente;
        public ObservableCollection<Ricetta> Ricette { get; private set; }

        public FavouritePage(Utente usr)
        {
            InitializeComponent();
            utente = usr;
            loadFavRicette();
        }

        //carica ricette preferite dell'utente
        private async Task loadFavRicette()
        {
            if (utente.Preferiti.Count == 0)
            {
                await Navigation.PushModalAsync(new ErrorDialog("Non ci sono ricette nei prefeiriti\nAggiungile ora", new Command(() => { Navigation.PopAsync(); })));
            }
            else
            {
                List<Ricetta> ricette = new List<Ricetta>();
                for (int i = 0; i < utente.Preferiti.Count; i++)
                {
                    var group = await CrossCloudFirestore.Current.Instance.GetCollection("Ricette").WhereEqualsTo(FieldPath.DocumentId, utente.Preferiti[i]).GetDocumentsAsync();
                    ricette.AddRange(group.ToObjects<Ricetta>().ToList());

                }
                Ricette = new ObservableCollection<Ricetta>(ricette);
                BindableLayout.SetItemsSource(FavouriteList, Ricette);
            }

        }

        private void Back(object sender, EventArgs args)
        {
            Navigation.PopAsync();
        }

        private void OpenRicettaById(object sender, EventArgs args)
        {
            String value = ((Button)sender).CommandParameter.ToString();
            System.Collections.ObjectModel.ObservableCollection<Ricetta> temp = (ObservableCollection<Ricetta>)BindableLayout.GetItemsSource(FavouriteList);
            for (int i = 0; i < temp.Count; i++)
            {
                if (temp[i].Id.Equals(value))
                {
                    ShowPage page = new ShowPage((Ricetta)temp[i], "Show", utente);
                    page.enableBackReturn(new Command(() =>
                    {
                        loadFavRicette();
                    }));
                    PushPage(page);
                }
            }

        }
        private async void PushPage(ContentPage page)
        {
            await Navigation.PushAsync(page);
        }

    }
}