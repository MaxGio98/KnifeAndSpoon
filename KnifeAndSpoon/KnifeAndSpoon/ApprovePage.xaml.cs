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
    public partial class ApprovePage : ContentPage
    {
        private Utente utente;
        public ApprovePage(Utente usr)
        {
            InitializeComponent();
            utente = usr;
            LoadAsync();
        }

        //carica ricette da approvare (isApproved==false)
        private async void LoadAsync()
        {
            loadOverlay.IsVisible = true;
            var group = await CrossCloudFirestore.Current.
               Instance.
               GetCollection("Ricette").
               WhereEqualsTo("isApproved", false).
               GetDocumentsAsync();
            List<Ricetta> ricette = group.ToObjects<Ricetta>().ToList();
            BindableLayout.SetItemsSource(List, new ObservableCollection<Ricetta>(ricette));
            loadOverlay.IsVisible = false;
            if (ricette.Count == 0)
            {
                await Navigation.PushModalAsync(new ErrorDialog("Non ci sono ricette da approvare", new Command(() => { Navigation.PopAsync(); })));
            }
        }

        private void Back(object sender, EventArgs args)
        {
            Navigation.PopAsync();
        }

        private void OpenRicettaById(object sender, EventArgs args)
        {
            String value = ((Button)sender).CommandParameter.ToString();
            System.Collections.ObjectModel.ObservableCollection<Ricetta> temp = (ObservableCollection<Ricetta>)BindableLayout.GetItemsSource(List);
            for (int i = 0; i < temp.Count; i++)
            {
                if (temp[i].Id.Equals(value))
                {
                    ShowPage page = new ShowPage((Ricetta)temp[i], "Admin", utente);
                    page.enableBackReturn(new Command(() =>
                    {
                        LoadAsync();
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