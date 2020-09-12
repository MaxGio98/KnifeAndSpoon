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
    public partial class ErrorDialog : ContentPage
    {
        public ErrorDialog(string text)
        {
            InitializeComponent();
            label.Text = text;
        }

        public void okButton(object sender, EventArgs args)
        {
            Navigation.PopModalAsync();
        }
    }
}