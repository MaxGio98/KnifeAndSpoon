using Android.App;
using KnifeAndSpoon.Droid.Implementation;
using Xamarin.Forms;

[assembly: Dependency(typeof(YesNoPopupLoader))]
namespace KnifeAndSpoon.Droid.Implementation
{
    public class YesNoPopupLoader : IYesNoPopupLoader
    {
        public void ShowPopup(CustomBox popup)
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(Forms.Context);
            var alert = new AlertDialog.Builder(Forms.Context);
            alert.SetMessage(popup.Text);
            alert.SetTitle(popup.Title);

            var buttons = popup.Buttons;
            alert.SetPositiveButton(buttons[0], (senderAlert, args) =>
            {
                popup.OnPopupClosed(new CustomYesNoBoxClosedArgs
                {
                    Button = buttons[0]
                });
            });
            alert.SetCancelable(false);
            alert.Show();
        }
    }
}