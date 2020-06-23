using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace KnifeAndSpoon.Droid
{
    [Activity(Theme = "@style/SplashTheme", MainLauncher = true, NoHistory = true)]
    public class Splash : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        static readonly string TAG = "X:" + typeof(Splash).Name;

        public override void OnCreate(Bundle savedInstanceState, PersistableBundle persistentState)
        {
            base.OnCreate(savedInstanceState, persistentState);
        }

        // Launches the startup task
        protected override void OnResume()
        {
            base.OnResume();
            StartActivity(new Intent(Application.Context, typeof(MainActivity)));
        }
    }
}