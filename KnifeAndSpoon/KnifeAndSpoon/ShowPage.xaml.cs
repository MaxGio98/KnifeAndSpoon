using KnifeAndSpoon.Model;
using Plugin.CloudFirestore;
using Plugin.FirebaseAuth;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace KnifeAndSpoon
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ShowPage : ContentPage
    {
        private Utente Autore;
        private String Mode;
        private Ricetta r;
        private Boolean isFav = false;
        private Utente utente;
        private Command backReturn;
        public ShowPage(Ricetta ricetta, String Mode, Utente usr)
        {
            this.Mode = Mode;
            InitializeComponent();
            r = ricetta;
            utente = usr;
            //Apply show mode
            if (Mode.Equals("Show"))
            {
                multiFab.ImageSource = "favourite";
                if (!CrossFirebaseAuth.Current.Instance.CurrentUser.IsAnonymous)
                {
                    setPreferiti();
                }
                else
                {
                    multiFab.BackgroundColor = Color.FromHex("#aa00000");
                }
            }
            else if (Mode.Equals("Admin"))
            {
                multiFab.ImageSource = "more";
            }

            //Load Recipe
            Thumbnail.Source = ricetta.Thumbnail;
            Titolo.Text = ricetta.Titolo;
            Tempo.Text = ricetta.TempoPreparazione + " minuti";
            if (int.Parse(ricetta.NumeroPersone) > 1)
            {
                Porzioni.Text = ricetta.NumeroPersone + " persone";
            }
            else
            {
                Porzioni.Text = ricetta.NumeroPersone + " persona";
            }

            showCategoria(ricetta.Categoria);

            //Analisi ingredienti
            List<Ingrediente> ingredienti = new List<Ingrediente>();
            for (int i = 0; i < ricetta.Ingredienti.Count; i++)
            {
                string correctform = getCorrectUtForm(ricetta.Ingredienti[i]["Unità misura"].ToString(), ricetta.Ingredienti[i]["Quantità"].ToString());
                if (double.Parse(ricetta.Ingredienti[i]["Quantità"].ToString()) > 0)
                {
                    ingredienti.Add(new Ingrediente(ricetta.Ingredienti[i]["Nome"].ToString(), ricetta.Ingredienti[i]["Quantità"].ToString(), correctform));
                }
                else
                {
                    ingredienti.Add(new Ingrediente(ricetta.Ingredienti[i]["Nome"].ToString(), "", correctform));
                }

            }
            ObservableCollection<Ingrediente> Ingredienti = new ObservableCollection<Ingrediente>(ingredienti);
            BindableLayout.SetItemsSource(lst_ingredienti, Ingredienti);

            //Analisi passaggi
            List<Passaggio> passaggi = new List<Passaggio>();
            for (int i = 0; i < ricetta.Passaggi.Count; i++)
            {
                passaggi.Add(new Passaggio(i + 1, ricetta.Passaggi[i]));
            }
            ObservableCollection<Passaggio> Passaggi = new ObservableCollection<Passaggio>(passaggi);
            BindableLayout.SetItemsSource(lst_passaggi, Passaggi);
            LoadUtente(ricetta.Autore);
        }

        public void enableBackReturn(Command command)
        {
            backReturn = command;
        }

        private void showCategoria(String categoria)
        {
            NomeCategoria.Text = categoria;
            switch (categoria)
            {
                case "Primo":
                    ImgCategoria.Source = "primo";
                    break;
                case "Secondo":
                    ImgCategoria.Source = "secondo";
                    break;
                case "Antipasto":
                    ImgCategoria.Source = "antipasto";
                    break;
                case "Contorno":
                    ImgCategoria.Source = "contorno";
                    break;
                case "Dolce":
                    ImgCategoria.Source = "dolce";
                    break;
                default: break;
            }
        }

        private async Task LoadUtente(String id)
        {

            var result = await CrossCloudFirestore.Current.Instance.GetDocument("Utenti/" + id).GetDocumentAsync();

            Console.WriteLine(result.Data["Nome"].ToString());
            Console.WriteLine(result.Data["Immagine"].ToString());
            ImgAutore.Source = result.Data["Immagine"].ToString();
            NomeAutore.Text = result.Data["Nome"].ToString();
        }

        private String getCorrectUtForm(String ut, String qt)
        {
            Double quant = Double.Parse(qt);
            String correctForm = "";
            if (quant == 1)
            {
                if (ut.Equals("grammi"))
                {
                    correctForm = "grammo";
                }
                else if (ut.Equals("litri"))
                {
                    correctForm = "litro";
                }
                else if (ut.Equals("millilitri"))
                {
                    correctForm = "millilitro";
                }
                else
                {
                    correctForm = ut;
                }
            }
            else
            {
                if (ut.Equals("bicchiere"))
                {
                    if (quant >= 2)
                    {
                        correctForm = "bicchieri";
                    }
                    else
                    {
                        correctForm = ut;
                    }
                }
                else if (ut.Equals("cucchiaio"))
                {
                    correctForm = "cucchiai";
                }
                else if (ut.Equals("cucchiaino"))
                {
                    correctForm = "cucchiaini";
                }
                else
                {
                    correctForm = ut;
                }
            }
            return correctForm;
        }

        private async void setPreferiti()
        {
            for (int i = 0; i < utente.Preferiti.Count; i++)
            {
                if (utente.Preferiti[i].Equals(r.Id))
                {
                    isFav = true;
                }
            }
            if (isFav)
            {
                multiFab.ImageSource = "favourite_full";
            }
            else
            {
                multiFab.ImageSource = "favourite";
            }
        }

        private async void multiFabAction(object sender, EventArgs e)
        {
            if (Mode.Equals("Show"))
            {
                if (!CrossFirebaseAuth.Current.Instance.CurrentUser.IsAnonymous)
                {
                    if (isFav)
                    {
                        isFav = false;
                        utente.Preferiti.Remove(r.Id);
                        await CrossCloudFirestore.Current.Instance.GetCollection("Utenti").GetDocument(utente.Id).UpdateDataAsync("Preferiti", utente.Preferiti);
                        multiFab.ImageSource = "favourite";
                    }
                    else
                    {
                        isFav = true;
                        utente.Preferiti.Add(r.Id);
                        await CrossCloudFirestore.Current.Instance.GetCollection("Utenti").GetDocument(utente.Id).UpdateDataAsync("Preferiti", utente.Preferiti);
                        multiFab.ImageSource = "favourite_full";
                    }
                }
                else
                {
                    await Navigation.PushModalAsync(new ConfirmDialog("Questa funzione è disponibile solo per chi è registrato\nRegistrati ora", new Command(() =>
                    {
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            CrossFirebaseAuth.Current.Instance.SignOut();
                            App.Current.MainPage = new NavigationPage(new MainPage());
                        });
                    })));
                }
                
            }
            else
            {
                multiFab.IsEnabled = false;
                await Navigation.PushModalAsync(new ApproveDialog("Cosa vuoi fare?",
                    new Command(() =>
                    {
                        approveRicetta();
                    }),
                    new Command(() =>
                    {
                        removeRicetta();
                    }),
                    new Command(async() =>
                    {
                        await Navigation.PopModalAsync();
                        multiFab.IsEnabled = true;
                    })));
            }
        }

        private void approveRicetta()
        {
            Navigation.PushModalAsync(new ConfirmDialog("Sei sicuro?",
                    new Command(async () =>
                    {
                        await Navigation.PopModalAsync();
                        await Navigation.PopModalAsync();
                        //Modifica Firebase
                        await CrossCloudFirestore.Current
                         .Instance
                         .GetCollection("Ricette")
                         .GetDocument(r.Id)
                         .UpdateDataAsync(new { isApproved = true });
                        await Navigation.PopAsync();
                        //Aggiorna lista
                        backReturn.Execute(backReturn);
                    })
                    ));
        }

        private void removeRicetta()
        {
            Navigation.PushModalAsync(new ConfirmDialog("Sei sicuro?",
                    new Command(async () =>
                    {
                        await Navigation.PopModalAsync();
                        await Navigation.PopModalAsync();
                        //Rimuove da Firebase
                        await CrossCloudFirestore.Current
                         .Instance
                         .GetCollection("Ricette")
                         .GetDocument(r.Id)
                         .DeleteDocumentAsync();
                        await Navigation.PopAsync();
                        //Aggiorna lista
                        backReturn.Execute(backReturn);
                    })
                    ));
        }

        private void Back(object sender, EventArgs args)
        {
            Navigation.PopAsync();
            if (backReturn != null)
            {
                backReturn.Execute(backReturn);
            }
        }

        protected override bool OnBackButtonPressed()
        {
            if (backReturn != null)
            {
                backReturn.Execute(backReturn);
            }
            return false;
        }

    }
}