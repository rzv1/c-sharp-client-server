using MsBox.Avalonia;

namespace CsharpMPP.Models;
public class Utils
{
    public static void ShowAlert(string message)
    {
        var messageBoxStandardWindow = MessageBoxManager
            .GetMessageBoxStandard("Error", message);
        messageBoxStandardWindow.ShowWindowAsync();
    }
}