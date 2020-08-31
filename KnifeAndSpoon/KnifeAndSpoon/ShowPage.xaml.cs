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
    public partial class ShowPage : ContentPage
    {
        Utente Autore;
        String Mode;
        public ShowPage(Ricetta ricetta,String Mode)
        {
            this.Mode = Mode;
            InitializeComponent();

            //Apply show mode
            if (Mode == "Show")
            {
                multiFab.ImageSource = "favourite";
            }else if (Mode == "Admin")
            {
                multiFab.ImageSource = "more";
            }

            //Load Recipe
            Thumbnail.Source = ricetta.Thumbnail;
            Titolo.Text = ricetta.Titolo;
            Tempo.Text = ricetta.TempoPreparazione + " minuti";
            if ( int.Parse(ricetta.NumeroPersone) > 1)
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
                    ingredienti.Add(new Ingrediente(ricetta.Ingredienti[i]["Nome"].ToString(), "" , correctform));
                }
                
            }
            ObservableCollection<Ingrediente> Ingredienti = new ObservableCollection<Ingrediente>(ingredienti);
            BindableLayout.SetItemsSource(lst_ingredienti,Ingredienti);

            //Analisi passaggi
            List<Passaggio> passaggi = new List<Passaggio>();
            for(int i = 0; i < ricetta.Passaggi.Count; i++)
            {
                passaggi.Add(new Passaggio(i+1, ricetta.Passaggi[i]));
            }
            ObservableCollection<Passaggio> Passaggi = new ObservableCollection<Passaggio>(passaggi);
            BindableLayout.SetItemsSource(lst_passaggi, Passaggi);
            LoadUtente(ricetta.Autore);
        }

        public void showCategoria(String categoria)
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
                default:break;
            }
        }

        public async Task LoadUtente(String id)
        {
            var result = await CrossCloudFirestore.Current.Instance.GetDocument("Utenti/" + id).GetDocumentAsync();
            Utente utente = result.ToObject<Utente>();
            Autore = utente;
            ImgAutore.Source = Autore.Immagine;
            NomeAutore.Text = Autore.Nome;
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

    }
}