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
            Approva.Command = new Command(() =>
            {
                Approva.IsEnabled = false;
                approveComm.Execute(approveComm);
            });
            NonApprova.Command = new Command(() =>
            {
                NonApprova.IsEnabled = false;
                nonComm.Execute(approveComm);
            });
            abort.Command = new Command(() => { Navigation.PopModalAsync(); });
        }

        public ApproveDialog(string text, Command approveComm, Command nonComm, Command abortCommand)
        {
            InitializeComponent();
            label.Text = text;
            Approva.Command = new Command(() =>
            {
                Approva.IsEnabled = false;
                approveComm.Execute(approveComm);
            });
            NonApprova.Command = new Command(() =>
            {
                NonApprova.IsEnabled = false;
                nonComm.Execute(approveComm);
            });
            abort.Command = abortCommand;
        }
    }
}