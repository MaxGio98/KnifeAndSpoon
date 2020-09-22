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
    public partial class ConfirmDialog : ContentPage
    {
        public ConfirmDialog(string text, Command okComm, Command noComm)
        {
            InitializeComponent();
            label.Text = text;
            ok.Command = new Command(() =>
            {
                ok.IsEnabled = false;
                okComm.Execute(okComm);
            });
            no.Command = noComm;
            abort.Command = new Command(() => { Navigation.PopModalAsync(); });
        }

        public ConfirmDialog(string text, Command okComm)
        {
            InitializeComponent();
            label.Text = text;
            ok.Command = new Command(() =>
            {
                ok.IsEnabled = false;
                okComm.Execute(okComm);
            });
            no.Command = new Command(() => { Navigation.PopModalAsync(); });
            abort.IsEnabled = false;
            abort.IsVisible = false;
        }
    }
}