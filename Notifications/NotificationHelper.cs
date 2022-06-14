using Notification.Wpf;
using System;
using System.Windows.Media.Imaging;

namespace ShopOffice.Notifications
{
    internal class NotificationHelper
    {
        public static void ShowWarning(String message)
        {
            NotificationHelper.Show(message, NotificationType.Warning);
        }
        public static void ShowError(String message)
        {
            NotificationHelper.Show(message, NotificationType.Error);
        }
        public static void Show(String message, NotificationType type)
        {
            BitmapImage logo = new BitmapImage(new Uri("pack://application:,,,/ShopOffice;component/Resources/icon.png"));

            var notificationManager = new NotificationManager();
            notificationManager.Show(Properties.Resources.str_0001, message, type,
                icon: logo);
        }
    }
}
