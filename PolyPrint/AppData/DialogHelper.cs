using System.Windows;

namespace PolyPrint.AppData
{
    public static class DialogHelper
    {
        private const string Caption = "PolyPrint";

        public static void ShowSuccess(string message)
        {
            MessageBox.Show(message, Caption, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public static void ShowWarning(string message)
        {
            MessageBox.Show(message, Caption, MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        public static void ShowError(string message)
        {
            MessageBox.Show(message, Caption, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public static bool Confirm(string message)
        {
            return MessageBox.Show(message, Caption, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;
        }
    }
}
