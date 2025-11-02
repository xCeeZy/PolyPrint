using System;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace PolyPrint.AppData
{
    public static class NavigationHelper
    {
        public static bool Navigate(Frame frame, Page page, object parameter = null)
        {
            if (frame == null || page == null)
            {
                return false;
            }

            if (parameter != null)
            {
                frame.Navigate(page, parameter);
            }
            else if (frame.Content == null || frame.Content.GetType() != page.GetType())
            {
                frame.Navigate(page);
            }
            else
            {
                return false;
            }

            return true;
        }

        public static bool CanGoBack(Frame frame)
        {
            return frame != null && frame.CanGoBack;
        }

        public static void GoBack(Frame frame)
        {
            if (frame != null && frame.CanGoBack)
            {
                frame.GoBack();
            }
        }

        public static void ClearJournal(Frame frame)
        {
            if (frame?.NavigationService is NavigationService navigationService)
            {
                navigationService.RemoveBackEntry();
                while (navigationService.RemoveBackEntry() != null)
                {
                }
            }
        }
    }
}
