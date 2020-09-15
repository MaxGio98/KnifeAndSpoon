using Android.App;
using Android.Widget;

using KnifeAndSpoon.Droid.Implementation;

using Xamarin.Forms;

[assembly: Dependency(typeof(YesNoPopupLoader))]
namespace KnifeAndSpoon.Droid.Implementation
{
    public class YesNoPopupLoader : IYesNoPopupLoader
{
    public void ShowPopup(CustomYesNoBox popup)
    {
        var alert = new AlertDialog.Builder(Forms.Context);

        var textView = new TextView(Forms.Context) { Text = popup.Text };
        alert.SetView(textView);

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