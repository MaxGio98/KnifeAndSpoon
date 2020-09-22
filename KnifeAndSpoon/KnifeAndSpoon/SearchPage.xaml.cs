using KnifeAndSpoon.Model;
using Plugin.CloudFirestore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace KnifeAndSpoon
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SearchPage : ContentPage
    {
        private Utente utente;
        public SearchPage(Utente usr)
        {
            InitializeComponent();
            utente = usr;
            searchField.Completed += (sender, e) =>
            {
                Search(sender, e);
            };
        }

        private void Search(object sender, EventArgs args)
        {
            //se è stato inserito qualche carattere che non sia lo spazio, allora si lancia la ricerca
            if (searchField.Text != null)
            {
                if (!(searchField.Text.Trim().Equals("")))
                {
                    SearchAsync(searchField.Text);
                }
                else
                {
                    DependencyService.Get<IAndroidPopUp>().ShowSnackbar("Inserisci qualcosa da ricercare!");
                }
            }
            else
            {
                DependencyService.Get<IAndroidPopUp>().ShowSnackbar("Inserisci qualcosa da ricercare!");
            }
        }

        private async void SearchAsync(string value)
        {
            loadOverlay.IsVisible = true;
            var group = await CrossCloudFirestore.Current.
               Instance.
               GetCollection("Ricette").
               WhereEqualsTo("isApproved", true).
               GetDocumentsAsync();
            List<Ricetta> ricette = group.ToObjects<Ricetta>().ToList();
            List<Ricetta> ricetteFiltered = new List<Ricetta>();
            for (int i = 0; i < ricette.Count; i++)
            {
                //se ciò che è stato cercato è contenuto nel titolo delle ricette approvate, allora si aggiunge il tutto alle ricette filtrate da visualizzare
                if (ricette[i].Titolo.ToLower().Contains(value.ToLower()))
                {
                    ricetteFiltered.Add(ricette[i]);
                }
            }
            BindableLayout.SetItemsSource(SearchList, new ObservableCollection<Ricetta>(ricetteFiltered));
            loadOverlay.IsVisible = false;
            if (ricetteFiltered.Count == 0)
            {
                noResult.IsVisible = true;
            }
            else
            {
                noResult.IsVisible = false;
            }
        }

        private void Back(object sender, EventArgs args)
        {
            Navigation.PopAsync();
        }

        private void OpenRicettaById(object sender, EventArgs args)
        {
            String value = ((Button)sender).CommandParameter.ToString();
            System.Collections.ObjectModel.ObservableCollection<Ricetta> temp = (ObservableCollection<Ricetta>)BindableLayout.GetItemsSource(SearchList);
            for (int i = 0; i < temp.Count; i++)
            {
                if (temp[i].Id.Equals(value))
                {
                    PushPage(new ShowPage((Ricetta)temp[i], "Show", utente));
                }
            }

        }
        private async void PushPage(ContentPage page)
        {
            await Navigation.PushAsync(page);
        }
    }
}