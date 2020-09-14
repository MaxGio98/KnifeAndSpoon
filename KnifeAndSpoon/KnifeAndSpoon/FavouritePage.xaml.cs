using KnifeAndSpoon.Model;
using Plugin.CloudFirestore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace KnifeAndSpoon
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FavouritePage : ContentPage
    {
        Utente utente;
        List<String> oldFav;
        public ObservableCollection<Ricetta> Ricette { get; private set; }

        public FavouritePage(Utente usr)
        {
            InitializeComponent();
            utente = usr;
            loadFavRicette();
        }

        public async Task loadFavRicette()
        {
            if(utente.Preferiti.Count==0)
            {
                Console.WriteLine("OHHHHHHH");
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

        public void Back(object sender, EventArgs args)
        {
            Navigation.PopAsync();
        }

        public void OpenRicettaById(object sender, EventArgs args)
        {
            oldFav = utente.Preferiti;
            String value = ((Button)sender).CommandParameter.ToString();
            System.Collections.ObjectModel.ObservableCollection<Ricetta> temp = (ObservableCollection<Ricetta>)BindableLayout.GetItemsSource(FavouriteList);
            for (int i = 0; i < temp.Count; i++)
            {
                if (temp[i].Id.Equals(value))
                {
                    PushPage(new ShowPage((Ricetta)temp[i], "Show", utente));
                }
            }

        }
        public async void PushPage(ContentPage page)
        {
            await Navigation.PushAsync(page);
        }

        protected virtual void OnResume()
        {
            loadFavRicette();
        }
    }
}