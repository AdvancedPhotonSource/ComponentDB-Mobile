using System;
namespace Component.DB.Services.PlatformDependency
{
    public interface INotificationPopup
    {
        void shortPopup(String message);
        void longPopup(String message); 
    }
}
