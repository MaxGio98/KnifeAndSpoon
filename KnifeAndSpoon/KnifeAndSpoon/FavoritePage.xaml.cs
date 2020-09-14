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
    public partial class FavoritePage : ContentPage
    {
        List<Utente> list;
        public ObservableCollection<Ricetta> Ricette { get; private set; }

        public FavoritePage(List<Utente>list)
        {
            InitializeComponent();
            this.list = list;
            loadFavRicette();
        }

        public async Task loadFavRicette()
        {
            if(list[0].Preferiti.Count==0)
            {
                Console.WriteLine("OHHHHHHH");
            }
            else
            {
                for (int i = 0; i < list[0].Preferiti.Count; i++)
                {
                    var group = await CrossCloudFirestore.Current.Instance.GetCollection("Ricette").WhereEqualsTo(FieldPath.DocumentId, list[0].Preferiti[i]).GetDocumentsAsync();
                    List<Ricetta> ricette = group.ToObjects<Ricetta>().ToList();
                    Ricette = new ObservableCollection<Ricetta>(ricette);
                    TheCarousel.ItemsSource = Ricette;
                }
                

            }

        }
        public void OpenRicetta(object sender, EventArgs args)
        {
            Console.WriteLine(TheCarousel.Position.ToString());
            Console.WriteLine(Ricette[TheCarousel.Position].Titolo);
            PushPage(new ShowPage(Ricette[TheCarousel.Position], "Show", list));
        }

        public async void PushPage(ContentPage page)
        {
            await Navigation.PushAsync(page);
        }
    }
}