using System;
using Component.DB.ViewModels;

namespace Component.DB.Services.CdbEventArgs
{
    public class NewRootPageNavigationEventArgs
    {
        public MenuItemType CurrentPageMenuItem { get; }

        public NewRootPageNavigationEventArgs(MenuItemType CurrentPageMenuItem)
        {
            this.CurrentPageMenuItem = CurrentPageMenuItem;
        }
    }
}
