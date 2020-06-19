using System;
using Android.App;
using Android.Gms.Common;
using Android.Gms.Common.Apis;
using Android.Gms.Extensions;
using Android.Gms.Auth.Api;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Firebase.Auth;
using Firebase;
using Android.Gms.Auth.Api.SignIn;
using Android.Content;
using System.Threading.Tasks;

namespace KnifeAndSpoon
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity, GoogleApiClient.IOnConnectionFailedListener
    {
        const string TAG = "GoogleActivity";
        const int RC_SIGN_IN = 9001;

        // [START declare_auth]
        FirebaseAuth mAuth;
        // [END declare_auth]

        GoogleApiClient mGoogleApiClient;
        TextView mStatusTextView;
        TextView mDetailTextView;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            FloatingActionButton fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
            fab.Click += FabOnClick;
            // [START config_signin]
            // Configure Google Sign In
            var gso = new GoogleSignInOptions.Builder(GoogleSignInOptions.DefaultSignIn)
                    .RequestIdToken("938877840628-68ppr05or0uh0onkgmdlph64dvn6u035.apps.googleusercontent.com")
                    .RequestEmail()
                    .Build();
            // [END config_signin]
            
            mGoogleApiClient = new GoogleApiClient.Builder(this)
                    .EnableAutoManage(this /* FragmentActivity */, this /* OnConnectionFailedListener */)
                    .AddApi(Auth.GOOGLE_SIGN_IN_API, gso)
                    .Build();
            Firebase.FirebaseApp.InitializeApp(this);
            // [START initialize_auth]
            mAuth = FirebaseAuth.Instance;
            // [END initialize_auth]
        }

        //OAUTH THINGS
        void AuthStateChanged(object sender, FirebaseAuth.AuthStateEventArgs e)
        {
            var user = e.Auth.CurrentUser;
            if (user != null)
            {
                // User is signed in
                Android.Util.Log.Debug(TAG, "onAuthStateChanged:signed_in:" + user.Uid);
            }
            else
            {
                // User is signed out
                Android.Util.Log.Debug(TAG, "onAuthStateChanged:signed_out");
            }
            // [START_EXCLUDE]
            // [END_EXCLUDE]
        }

        // [START on_start_add_listener]
        protected override void OnStart()
        {
            base.OnStart();
            mAuth.AuthState += AuthStateChanged;
        }
        // [END on_start_add_listener]

        // [START on_stop_remove_listener]
        protected override void OnStop()
        {
            base.OnStop();
            mAuth.AuthState -= AuthStateChanged;
        }
        // [END on_stop_remove_listener]

        protected override async void OnActivityResult(int requestCode, Android.App.Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            // Result returned from launching the Intent from GoogleSignInApi.getSignInIntent(...);
            if (requestCode == RC_SIGN_IN)
            {
                var result = Android.Gms.Auth.Api.Auth.GoogleSignInApi.GetSignInResultFromIntent(data);
                if (result.IsSuccess)
                {
                    // Google Sign In was successful, authenticate with Firebase
                    await FirebaseAuthWithGoogle(result.SignInAccount);
                }
                else
                {
                    // Google Sign In failed, update UI appropriately
                    // [START_EXCLUDE]
                    // [END_EXCLUDE]
                }
            }
        }

        // [END onactivityresult]

        // [START auth_with_google]
        private async Task FirebaseAuthWithGoogle(GoogleSignInAccount acct)
        {
            Android.Util.Log.Debug(TAG, "firebaseAuthWithGoogle:" + acct.Id);
            // [START_EXCLUDE silent]
            // [END_EXCLUDE]

            AuthCredential credential = GoogleAuthProvider.GetCredential(acct.IdToken, null);

            try
            {
                await mAuth.SignInWithCredentialAsync(credential);
            }
            catch
            {
                Toast.MakeText(this, "Authentication failed.", ToastLength.Short).Show();
            }
            // [START_EXCLUDE]
            // [END_EXCLUDE]
        }

        // [START signin]
        void SignIn()
        {
            if (FirebaseAuth.Instance.CurrentUser != null)
            {

            }
            Intent signInIntent = Android.Gms.Auth.Api.Auth.GoogleSignInApi.GetSignInIntent(mGoogleApiClient);
            StartActivityForResult(signInIntent, RC_SIGN_IN);
        }
        // [END signin]

        async Task SignOut()
        {
            // Firebase sign out
            mAuth.SignOut();

            await Auth.GoogleSignInApi.SignOut(mGoogleApiClient);

        }

        async Task RevokeAccess()
        {
            // Firebase sign out
            mAuth.SignOut();

            // Google revoke access
            await Auth.GoogleSignInApi.RevokeAccess(mGoogleApiClient);

        }

        public void OnConnectionFailed(ConnectionResult connectionResult)
        {
            // An unresolvable error has occurred and Google APIs (including Sign-In) will not
            // be available.
            Android.Util.Log.Debug(TAG, "onConnectionFailed:" + connectionResult);
            Toast.MakeText(this, "Google Play Services error.", ToastLength.Short).Show();
        }

    //STOP OAUTH THINGS

    public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            if (id == Resource.Id.action_settings)
            {
                return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        private void FabOnClick(object sender, EventArgs eventArgs)
        {
            /*View view = (View) sender;
            Snackbar.Make(view, "Replace with your own action", Snackbar.LengthLong)
                .SetAction("Action", (Android.Views.View.IOnClickListener)null).Show();*/
            SignIn();
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
	}
}
