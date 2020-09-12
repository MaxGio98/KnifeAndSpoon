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
    public partial class SearchPage : ContentPage
    {
        public SearchPage()
        {
            InitializeComponent();
        }

        public void Search(object sender, EventArgs args)
        {
            if (searchField.Text != null)
            {
                SearchAsync(searchField.Text);
            }
        }

        public async void SearchAsync(string value)
        {
            Console.WriteLine(value);
            loadOverlay.IsVisible = true;
            var group = await CrossCloudFirestore.Current.
               Instance.
               GetCollection("Ricette").
               WhereEqualsTo("isApproved", true).
               WhereEqualsTo("Titolo",value).
               GetDocumentsAsync();
            List<Ricetta> ricette = group.ToObjects<Ricetta>().ToList();
            Console.WriteLine(ricette.Count);
            BindableLayout.SetItemsSource(SearchList, new ObservableCollection<Ricetta>(ricette));
            loadOverlay.IsVisible = false;
        }

        public void Back(object sender, EventArgs args)
        {
            Navigation.PopAsync();
        }

        public void OpenRicettaById(object sender, EventArgs args)
        {
            String value = ((Button)sender).CommandParameter.ToString();
            System.Collections.ObjectModel.ObservableCollection<Ricetta> temp = (ObservableCollection<Ricetta>)BindableLayout.GetItemsSource(SearchList);
            for (int i = 0; i < temp.Count; i++)
            {
                if (temp[i].Id.Equals(value))
                {
                    PushPage(new ShowPage((Ricetta)temp[i], "Show"));
                }
            }

        }
        public async void PushPage(ContentPage page)
        {
            await Navigation.PushAsync(page);
        }
    }
}