using System;
using Component.DB.iOS.Services.PlatformDependency; 
using Component.DB.Services.PlatformDependency;
using Foundation;
using UIKit;

[assembly: Xamarin.Forms.Dependency(typeof(NotificationPopup))]
namespace Component.DB.iOS.Services.PlatformDependency
{
    public class NotificationPopup : INotificationPopup
    {
        const double LONG_DELAY = 3.5;
        const double SHORT_DELAY = 2.0;

        NSTimer alertDelay;
        UIAlertController alert;

        public void longPopup(string message)
        {
            ShowAlert(message, LONG_DELAY);
        }

        public void shortPopup(string message)
        {
            ShowAlert(message, SHORT_DELAY);
        }

        void ShowAlert(string message, double seconds)
        {
            alertDelay = NSTimer.CreateScheduledTimer(seconds, (obj) =>
            {
                dismissMessage();
            });
            alert = UIAlertController.Create(null, message, UIAlertControllerStyle.Alert);
            UIApplication.SharedApplication.KeyWindow.RootViewController.PresentViewController(alert, true, null);
        }

        void dismissMessage()
        {
            if (alert != null)
            {
                alert.DismissViewController(true, null);
            }
            if (alertDelay != null)
            {
                alertDelay.Dispose();
            }
        }
    }
}
