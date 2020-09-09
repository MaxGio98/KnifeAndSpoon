using KnifeAndSpoon.Model;
using Plugin.CloudFirestore;
using Plugin.FirebaseAuth;
using Plugin.GoogleClient;
using Plugin.GoogleClient.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace KnifeAndSpoon
{

    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        private readonly IGoogleClientManager _googleClientManager;

        public MainPage()
        {
            _googleClientManager = CrossGoogleClient.Current;
            InitializeComponent();
            CheckUser();
        }

        public async void CheckUser()
        {
            Console.WriteLine("Inizio");
            var glob = await CrossCloudFirestore.Current.Instance.GetCollection("Utenti").WhereEqualsTo("Mail", CrossFirebaseAuth.Current.Instance.CurrentUser.Email).GetDocumentsAsync();
            Console.WriteLine("Caricato");
            Console.WriteLine(glob.Count);
            if (glob.Count == 0)
            {
                Console.WriteLine("Apertura");
                PushPage(new RegisterPage());
                Console.WriteLine("Fine");
            }
            else
            {
                Console.WriteLine("Apertura 2");
                //Apertura pagina principale
                App.Current.MainPage = new NavigationPage(new HomePage());
                Console.WriteLine("Fine 2");
            }
        }


        public void login(object sender, EventArgs args)
        {
            LoginAsync();
        }

        public void loginAnonymous(object sender,EventArgs args)
        {
            //login anonimo
        }

        public async void LoginAsync()
        {
            //Start google client manager (Google Login Ui)
            _googleClientManager.OnLogin += OnLoginCompleted;
            try
            {
                await _googleClientManager.LoginAsync();
            }
            catch (GoogleClientSignInNetworkErrorException e)
            {
                await App.Current.MainPage.DisplayAlert("Error", e.Message, "OK");
            }
            catch (GoogleClientSignInCanceledErrorException e)
            {
                await App.Current.MainPage.DisplayAlert("Error", e.Message, "OK");
            }
            catch (GoogleClientSignInInvalidAccountErrorException e)
            {
                await App.Current.MainPage.DisplayAlert("Error", e.Message, "OK");
            }
            catch (GoogleClientSignInInternalErrorException e)
            {
                await App.Current.MainPage.DisplayAlert("Error", e.Message, "OK");
            }
            catch (GoogleClientNotInitializedErrorException e)
            {
                await App.Current.MainPage.DisplayAlert("Error", e.Message, "OK");
            }
            catch (GoogleClientBaseException e)
            {
                await App.Current.MainPage.DisplayAlert("Error", e.Message, "OK");
            }

        }


        private void OnLoginCompleted(object sender, GoogleClientResultEventArgs<GoogleUser> loginEventArgs)
        {
            if (loginEventArgs.Data != null)
            {
                //Se il login è stato effettuato, e ottenuto il token, registrarlo su firebase
                GoogleUser googleUser = loginEventArgs.Data;
                finalizeLogin(CrossGoogleClient.Current.IdToken, CrossGoogleClient.Current.AccessToken);
            }
            else
            {
                App.Current.MainPage.DisplayAlert("Error", loginEventArgs.Message, "OK");
            }

            _googleClientManager.OnLogin -= OnLoginCompleted;

        }

        private async void finalizeLogin(string idToken,string accessToken)
        {
            //Registrazione utente su firebase
            var credential = CrossFirebaseAuth.Current.GoogleAuthProvider.GetCredential(idToken, accessToken);
            var result = await CrossFirebaseAuth.Current.Instance.SignInWithCredentialAsync(credential);
            //Controllo se utente è registrato alla piattaforma
            var glob = await CrossCloudFirestore.Current.Instance.GetCollection("Utenti").WhereEqualsTo("Mail", CrossFirebaseAuth.Current.Instance.CurrentUser.Email).GetDocumentsAsync();
            if (glob.Count == 0)
            {
                //Apertura pagina registrazione
                PushPage(new RegisterPage());
            }
            else
            {
                //Apertura pagina principale
                App.Current.MainPage = new NavigationPage(new HomePage());
            }
            
        }

        public async void PushPage(ContentPage page)
        {
            await Navigation.PushAsync(page);
        }

        private void Button_Clicked(object sender, EventArgs e)
        {

        }
    }
}
