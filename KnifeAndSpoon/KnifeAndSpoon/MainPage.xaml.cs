using KnifeAndSpoon.Model;
using Plugin.CloudFirestore;
using Plugin.Connectivity;
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
            if (CrossFirebaseAuth.Current.Instance.CurrentUser != null)
            {
                Console.WriteLine("Ci sta");
                if (CrossFirebaseAuth.Current.Instance.CurrentUser.IsAnonymous)
                {
                    Device.BeginInvokeOnMainThread(()=>
                    {
                        App.Current.MainPage = new NavigationPage(new HomePage());
                    });
                    
                }
                else
                {
                    loadOverlay.IsVisible = true;
                    var glob = await CrossCloudFirestore.Current.Instance.GetCollection("Utenti").WhereEqualsTo("Mail", CrossFirebaseAuth.Current.Instance.CurrentUser.Email).GetDocumentsAsync();
                    if (glob.Count == 0)
                    {
                        //Apertura pagina registrazione
                        PushPage(new RegisterPage());
                    }
                    else
                    {
                        //Apertura pagina principale
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            App.Current.MainPage = new NavigationPage(new HomePage());
                        });
                    }
                    loadOverlay.IsVisible = false;
                }
            }

        }


        public void login(object sender, EventArgs args)
        {
            LoginAsync();
        }

        public void loginAnonymous(object sender,EventArgs args)
        {
            LoginAnonimousAsync();
        }

        public async void LoginAnonimousAsync()
        {
            loadOverlay.IsVisible = true;
            await CrossFirebaseAuth.Current.Instance.SignInAnonymouslyAsync();
            loadOverlay.IsVisible = false;
            App.Current.MainPage = new NavigationPage(new HomePage());
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
                await Navigation.PushModalAsync(new ErrorDialog(e.Message));
            }
            catch (GoogleClientSignInCanceledErrorException e)
            {
                await Navigation.PushModalAsync(new ErrorDialog(e.Message));
            }
            catch (GoogleClientSignInInvalidAccountErrorException e)
            {
                await Navigation.PushModalAsync(new ErrorDialog(e.Message));
            }
            catch (GoogleClientSignInInternalErrorException e)
            {
                await Navigation.PushModalAsync(new ErrorDialog(e.Message));
            }
            catch (GoogleClientNotInitializedErrorException e)
            {
                await Navigation.PushModalAsync(new ErrorDialog(e.Message));
            }
            catch (GoogleClientBaseException e)
            {
                await Navigation.PushModalAsync(new ErrorDialog(e.Message));
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
                Navigation.PushModalAsync(new ErrorDialog(loginEventArgs.Message));
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
