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
    public partial class ImageSelectionDialog : ContentPage
    {
        public ImageSelectionDialog(string text, Command cameraComm, Command galleryComm)
        {
            InitializeComponent();
            label.Text = text;
            Fotocamera.Command = cameraComm;
            Galleria.Command = galleryComm;
            abort.Command = new Command(() => { Navigation.PopModalAsync(); });
        }
    }
}