using System;
using System.Collections.Generic;
using System.Text;

namespace KnifeAndSpoon
{
    public interface IAndroidPopUp
    {
        void ShowToast(string message);
        void ShowSnackbar(string message);
    }
}
