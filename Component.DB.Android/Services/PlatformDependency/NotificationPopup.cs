using System;
using Android.App;
using Android.Widget;
using Component.DB.Droid.Services.PlatformDependency;
using Component.DB.Services.PlatformDependency;

[assembly: Xamarin.Forms.Dependency(typeof(NotificationPopup))]
namespace Component.DB.Droid.Services.PlatformDependency
{
    public class NotificationPopup : INotificationPopup
    {
        public void longPopup(string message)
        {
            Toast.MakeText(Application.Context, message, ToastLength.Long).Show();
        }

        public void shortPopup(string message)
        {
            Toast.MakeText(Application.Context, message, ToastLength.Short).Show();
        }
    }
}
