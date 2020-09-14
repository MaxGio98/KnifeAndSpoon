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
    public partial class ApproveDialog : ContentPage
    {
        public ApproveDialog(string text, Command approveComm, Command nonComm)
        {
            InitializeComponent();
            label.Text = text;
            Approva.Command = approveComm;
            NonApprova.Command = nonComm;
            abort.Command = new Command(() => { Navigation.PopModalAsync(); });
        }
    }
}