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
            if (usr.Trim().Equals(""))
            {
                //Utils.errorDialog(this, R.string.error_empty_name, R.string.error_ok);
            }
            else if (usr.Contains(" ") && (usr.StartsWith(" ") && usr.EndsWith(" ")))
            {
                //Utils.errorDialog(this, R.string.error_name_space, R.string.error_ok);
            }
            else if (usr.Length < 6 || usr.Length > 20)
            {
                //Utils.errorDialog(this, R.string.error_lenght_name, R.string.error_ok);
            }
            else
            {
                //Controllo se il nome utente è già stato utilizzato
                finalizeRegist(usr);
            }
        }

        private async void finalizeRegist(String usr)
        {
            var glob = await CrossCloudFirestore.Current.Instance.GetCollection("Utenti").WhereEqualsTo("Nome", usr).GetDocumentsAsync();
            if (glob.Count == 0)
            {
                //Completamento registrazione
                await CrossCloudFirestore.Current
                         .Instance
                         .GetCollection("Utenti")
                         .AddDocumentAsync(new Utente(CrossFirebaseAuth.Current.Instance.CurrentUser.PhotoUrl.ToString(), CrossFirebaseAuth.Current.Instance.CurrentUser.Email,usr,false));
            }
            else
            {
                //Nome utente già utilizzato
            }
        }

    }
}