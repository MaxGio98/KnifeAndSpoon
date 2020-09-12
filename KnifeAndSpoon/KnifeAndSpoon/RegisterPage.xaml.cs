using KnifeAndSpoon.Model;
using Plugin.CloudFirestore;
using Plugin.FirebaseAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace KnifeAndSpoon
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RegisterPage : ContentPage
    {
        public RegisterPage()
        {
            InitializeComponent();
            ImgUtente.Source = CrossFirebaseAuth.Current.Instance.CurrentUser.PhotoUrl;
        }

        public void Register(object sender, EventArgs args)
        {
            string usr = NomeUtente.Text;
            //Controllo nome inserito
            if (usr == null)
            {
                Navigation.PushModalAsync(new ErrorDialog("Il nome non può essere vuoto"));
            }
            else if (usr.Trim().Equals(""))
            {
                Navigation.PushModalAsync(new ErrorDialog("Il nome non può essere vuoto"));
            }
            else if (usr.Contains(" ") && (usr.StartsWith(" ") && usr.EndsWith(" ")))
            {
                Navigation.PushModalAsync(new ErrorDialog("Il nome non può contenere spazi"));
            }
            else if (usr.Length < 6 || usr.Length > 20)
            {
                Navigation.PushModalAsync(new ErrorDialog("Il nome deve essere almeno 6 caratteri e al massimo 20"));
            }
            else
            {
                //Controllo se il nome utente è già stato utilizzato
                loadOverlay.IsVisible = true;
                finalizeRegist(usr);
            }
        }

        private async void finalizeRegist(String usr)
        {
            var glob = await CrossCloudFirestore.Current.Instance.GetCollection("Utenti").WhereEqualsTo("Nome", usr).GetDocumentsAsync();
            if (glob.Count == 0)
            {
                //Completamento registrazione
                Utente temp = new Utente();
                temp.Immagine = CrossFirebaseAuth.Current.Instance.CurrentUser.PhotoUrl.ToString();
                temp.Mail = CrossFirebaseAuth.Current.Instance.CurrentUser.Email;
                temp.Nome = usr;
                temp.isAdmin = false;
                await CrossCloudFirestore.Current
                         .Instance
                         .GetCollection("Utenti")
                         .AddDocumentAsync(temp);
                App.Current.MainPage = new NavigationPage(new HomePage());
            }
            else
            {
                //Nome utente già utilizzato
                Navigation.PushModalAsync(new ErrorDialog("Il nome è già stato utilizzato"));
            }
            loadOverlay.IsVisible = false;
        }

    }
}