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
        Command custom;

        public ErrorDialog(string text)
        {
            InitializeComponent();
            label.Text = text;
        }

        public ErrorDialog(string text, Command customOk)
        {
            InitializeComponent();
            label.Text = text;
            custom = customOk;
        }

        private void okButton(object sender, EventArgs args)
        {
            Navigation.PopModalAsync();
            if (custom != null)
            {
                custom.Execute(custom);
            }
        }
    }
}