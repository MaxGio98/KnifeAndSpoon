using Plugin.CloudFirestore;
using Plugin.FirebaseAuth;
using Plugin.GoogleClient;
using Plugin.GoogleClient.Shared;
using System;
using System.ComponentModel;
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

        private async void CheckUser()
        {
            if (CrossFirebaseAuth.Current.Instance.CurrentUser != null)
            {
                if (CrossFirebaseAuth.Current.Instance.CurrentUser.IsAnonymous)
                {
                    //apertura della pagina principale
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        App.Current.MainPage = new NavigationPage(new HomePage());
                    });

                }
                else
                {
                    loadOverlay.IsVisible = true;
                    //controllo se l'utente si è registrato con la mail selezionata
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


        private void login(object sender, EventArgs args)
        {
            LoginAsync();
        }

        private void loginAnonymous(object sender, EventArgs args)
        {
            anonimous.IsEnabled = false;
            google.IsEnabled = false;
            LoginAnonimousAsync();
        }

        private async void LoginAnonimousAsync()
        {
            loadOverlay.IsVisible = true;
            await CrossFirebaseAuth.Current.Instance.SignInAnonymouslyAsync();
            loadOverlay.IsVisible = false;
            anonimous.IsEnabled = true;
            google.IsEnabled = true;
            App.Current.MainPage = new NavigationPage(new HomePage());
        }

        private async void LoginAsync()
        {
            anonimous.IsEnabled = false;
            google.IsEnabled = false;
            //Start google client manager (Google Login Ui)
            _googleClientManager.OnLogin += OnLoginCompleted;
            try
            {
                await _googleClientManager.LoginAsync();
            }
            catch (GoogleClientSignInNetworkErrorException e)
            {
                await Navigation.PushModalAsync(new ErrorDialog(e.Message));
                anonimous.IsEnabled = true;
                google.IsEnabled = true;
            }
            catch (GoogleClientSignInCanceledErrorException e)
            {
                await Navigation.PushModalAsync(new ErrorDialog(e.Message));
                anonimous.IsEnabled = true;
                google.IsEnabled = true;
            }
            catch (GoogleClientSignInInvalidAccountErrorException e)
            {
                await Navigation.PushModalAsync(new ErrorDialog(e.Message));
                anonimous.IsEnabled = true;
                google.IsEnabled = true;
            }
            catch (GoogleClientSignInInternalErrorException e)
            {
                await Navigation.PushModalAsync(new ErrorDialog(e.Message));
                anonimous.IsEnabled = true;
                google.IsEnabled = true;
            }
            catch (GoogleClientNotInitializedErrorException e)
            {
                await Navigation.PushModalAsync(new ErrorDialog(e.Message));
                anonimous.IsEnabled = true;
                google.IsEnabled = true;
            }
            catch (GoogleClientBaseException e)
            {
                await Navigation.PushModalAsync(new ErrorDialog(e.Message));
                anonimous.IsEnabled = true;
                google.IsEnabled = true;
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

        private async void finalizeLogin(string idToken, string accessToken)
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

        private async void PushPage(ContentPage page)
        {
            await Navigation.PushAsync(page);
        }

        private void Button_Clicked(object sender, EventArgs e)
        {

        }
    }
}
