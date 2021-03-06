﻿using KnifeAndSpoon.Model;
using Plugin.CloudFirestore;
using Plugin.FirebaseAuth;
using Plugin.FirebaseStorage;
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

        private void longClickMultiFab(object sender, EventArgs e)
        {
            if (Mode.Equals("Show"))
            {
                if (isFav)
                {
                    DependencyService.Get<IAndroidPopUp>().ShowSnackbar("Rimuovi dai preferiti");
                }
                else
                {
                    DependencyService.Get<IAndroidPopUp>().ShowSnackbar("Aggiungi ai preferiti");

                }
            }
            else if (Mode.Equals("Admin"))
            {
                DependencyService.Get<IAndroidPopUp>().ShowSnackbar("Approvare o non approvare la ricetta");
            }
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

        //imposta la grafica del preferito 
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

        //metodo per la gestione del FAB multiFAB
        private async void multiFabAction(object sender, EventArgs e)
        {
            //se l'utente vuole visualizzare una ricetta approvata
            if (Mode.Equals("Show"))
            {
                //se l'utente non è anonimo allora il pulsante dei preferiti deve funzionare come aggiunta/rimozione del preferito
                if (!CrossFirebaseAuth.Current.Instance.CurrentUser.IsAnonymous)
                {
                    if (isFav)
                    {
                        isFav = false;
                        utente.Preferiti.Remove(r.Id);
                        await CrossCloudFirestore.Current.Instance.GetCollection("Utenti").GetDocument(utente.Id).UpdateDataAsync("Preferiti", utente.Preferiti);
                        multiFab.ImageSource = "favourite";
                        DependencyService.Get<IAndroidPopUp>().ShowSnackbar("Rimosso dai preferiti");

                    }
                    else
                    {
                        isFav = true;
                        utente.Preferiti.Add(r.Id);
                        await CrossCloudFirestore.Current.Instance.GetCollection("Utenti").GetDocument(utente.Id).UpdateDataAsync("Preferiti", utente.Preferiti);
                        multiFab.ImageSource = "favourite_full";
                        DependencyService.Get<IAndroidPopUp>().ShowSnackbar("Aggiunto ai preferiti");

                    }
                }
                else
                {
                    //se l'utente è anonimo il FAB non deve caricare niente su firebase, funge da eventuale redirect alla pagina di login 
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
                //se l'utente admin accede alla sezione di approvazione delle ricette visualizzerà un FAB con icona dedicata che chiederà l'approvazione per la pubblicazione della ricetta
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
                    new Command(async () =>
                    {
                        await Navigation.PopModalAsync();
                        multiFab.IsEnabled = true;
                    })));
            }
        }

        //metodo per approvare la ricetta
        private void approveRicetta()
        {
            Navigation.PushModalAsync(new ConfirmDialog("Sei sicuro?",
                    new Command(async () =>
                    {
                        await Navigation.PopModalAsync();
                        await Navigation.PopModalAsync();
                        try
                        {
                            //Modifica Firebase
                            await CrossCloudFirestore.Current.
                            Instance.GetCollection("Ricette").
                            GetDocument(r.Id).
                            UpdateDataAsync(new { isApproved = true });
                            await Navigation.PopAsync();
                            //Aggiorna lista
                            backReturn.Execute(backReturn);
                        }
                        catch (Exception e)
                        {
                            //questo catch potrebbe verificarsi, ad esempio, quando due admin si trovano nella stessa ricetta e il primo non la approva e in questo caso il secondo la approva
                            Navigation.PushModalAsync(new ErrorDialog("Si è verificato un errore.", new Command(async () =>
                            {
                                await Navigation.PopAsync();
                                backReturn.Execute(backReturn);
                            })));
                        }
                    })
                    ));
        }

        //metodo per non approvare la ricetta
        private void removeRicetta()
        {
            Navigation.PushModalAsync(new ConfirmDialog("Sei sicuro?",
                    new Command(async () =>
                    {
                        await Navigation.PopModalAsync();
                        await Navigation.PopModalAsync();
                        try
                        {
                            //Rimuove da Firebase
                            await CrossCloudFirestore.Current
                            .Instance
                            .GetCollection("Ricette")
                            .GetDocument(r.Id)
                            .DeleteDocumentAsync();
                            await CrossFirebaseStorage.Current.Instance.GetReferenceFromUrl(r.Thumbnail).DeleteAsync();
                            await Navigation.PopAsync();
                            //Aggiorna lista
                            backReturn.Execute(backReturn);
                        }
                        catch (Exception e)
                        {
                            //questo catch potrebbe verificarsi, ad esempio, quando due admin si trovano nella stessa ricetta e il primo non la approva e in questo caso il secondo non la approva
                            Navigation.PushModalAsync(new ErrorDialog("Si è verificato un errore.", new Command(async () =>
                            {
                                await Navigation.PopAsync();
                                backReturn.Execute(backReturn);
                            })));
                        }
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