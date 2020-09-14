using KnifeAndSpoon.Model;
using Plugin.CloudFirestore;
using Plugin.FirebaseAuth;
using Plugin.FirebaseStorage;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
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
            string dio=CrossFirebaseAuth.Current.Instance.CurrentUser.PhotoUrl.AbsoluteUri;
            String path=System.Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            File.WriteAllBytes(path+"test.jpg",await DownloadImageAsync(dio));
            FileStream file = File.Open(path + "test.jpg", FileMode.Open);
            //Upload immagine profilo
            string filename = usr + ".jpg";
            var reference = CrossFirebaseStorage.Current.Instance.RootReference.GetChild(filename);
            var uploadProgress = new Progress<IUploadState>();
            uploadProgress.ProgressChanged += (sender, e) =>
            {
                var progress = e.TotalByteCount > 0 ? 100.0 * e.BytesTransferred / e.TotalByteCount : 0;
            };
            await reference.PutStreamAsync(file, progress: uploadProgress);
            var glob = await CrossCloudFirestore.Current.Instance.GetCollection("Utenti").WhereEqualsTo("Nome", usr).GetDocumentsAsync();
            if (glob.Count == 0)
            {
                //Completamento registrazione
                Utente temp = new Utente();
                //temp.Immagine = //CrossFirebaseAuth.Current.Instance.CurrentUser.PhotoUrl.ToString();
                temp.Mail = CrossFirebaseAuth.Current.Instance.CurrentUser.Email;
                temp.Nome = usr;
                temp.isAdmin = false;
                var imagePath= await reference.GetDownloadUrlAsync();
                temp.Immagine = imagePath.ToString();
                temp.Preferiti = new List<string>();
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
        public async Task<byte[]> DownloadImageAsync(string imageUrl)
        {
            var _httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(15) };

            try
            {
                using (var httpResponse = await _httpClient.GetAsync(imageUrl))
                {
                    if (httpResponse.StatusCode == HttpStatusCode.OK)
                    {
                        return await httpResponse.Content.ReadAsByteArrayAsync();
                    }
                    else
                    {
                        //Url is Invalid
                        return null;
                    }
                }
            }
            catch (Exception)
            {
                //Handle Exception
                return null;
            }
        }
    }
}