﻿using Xamarin.Forms;
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

        public ApproveDialog(string text, Command approveComm, Command nonComm, Command abortCommand)
        {
            InitializeComponent();
            label.Text = text;
            Approva.Command = approveComm;
            NonApprova.Command = nonComm;
            abort.Command = abortCommand;
        }
    }
}