using System;
using System.Collections.Generic;
using System.Linq;

using Xamarin.Forms;

public class CustomYesNoBox
{
    public string Text { get; set; }
    public string Title { get; set; }
    public List<string> Buttons { get; set; }

    public CustomYesNoBox(string title, string text, params string[] buttons)
    {
        Title = title;
        Text = text;
        Buttons = buttons.ToList();
    }

    public CustomYesNoBox(string title, string text) : this(title, text, "Riprova")
    {
    }

    public event EventHandler<CustomYesNoBoxClosedArgs> PopupClosed;
    public void OnPopupClosed(CustomYesNoBoxClosedArgs e)
    {
        var handler = PopupClosed;
        if (handler != null)
            handler(this, e);
    }

    public void Show()
    {
        DependencyService.Get<IYesNoPopupLoader>().ShowPopup(this);
    }
}

public class CustomYesNoBoxClosedArgs : EventArgs
{
    public string Button { get; set; }
}

public interface IYesNoPopupLoader
{
    void ShowPopup(CustomYesNoBox reference);
}