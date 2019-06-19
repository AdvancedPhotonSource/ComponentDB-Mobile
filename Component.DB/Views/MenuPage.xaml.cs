/*
 * Copyright (c) UChicago Argonne, LLC. All rights reserved.
 * See LICENSE file.
 */
using System.Collections.Generic;
using System.ComponentModel;
using Component.DB.Services.CdbEventArgs;
using Component.DB.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Component.DB.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(true)]
    public partial class MenuPage : ContentPage
    {
        MainPage RootPage { get => Application.Current.MainPage as MainPage; }
        List<HomeMenuItem> menuItems;
        public MenuPage()
        {
            InitializeComponent();

            menuItems = new List<HomeMenuItem>
            {
                new HomeMenuItem {Id = MenuItemType.ScanQRCode, Title="Scan QR Code", Icon=Constants.fontIconCamera },
                new HomeMenuItem {Id = MenuItemType.RelocateItems, Title="Relocate Items", Icon=Constants.fontIconLocation },
                new HomeMenuItem {Id = MenuItemType.BrowseCatalog, Title="Browse Catalog", Icon=Constants.fontIconBook },
                new HomeMenuItem {Id = MenuItemType.BrowseInventory, Title="Browse Inventory", Icon=Constants.fontIconBook },
                new HomeMenuItem {Id = MenuItemType.Settings, Title="Configuration", Icon=Constants.fontIconConfig },
                new HomeMenuItem {Id = MenuItemType.About, Title="About", Icon=Constants.fontIconQuestion }
            };

            ListViewMenu.ItemsSource = menuItems;

            ListViewMenu.SelectedItem = menuItems[(int)MenuItemType.BrowseCatalog];
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            ListViewMenu.ItemSelected += MenuItemNavigationItemSelected;
            RootPage.NewMenuNavigation += NewMenuNavigationFromApp;
        }

        public void NewMenuNavigationFromApp(object sender, NewRootPageNavigationEventArgs e)
        {
            var curPage = e.CurrentPageMenuItem;
            ListViewMenu.SelectedItem = menuItems[(int)curPage];
        }

        public async void MenuItemNavigationItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null)
                return;

            var activePage = (int)RootPage.ActiveMenuItemPage; 
            var reqPage = (int)((HomeMenuItem)e.SelectedItem).Id;

            if (activePage == reqPage)
            {
                return; 
            }

            await RootPage.NavigateFromMenu(reqPage);
        }

        public void ClearSelection()
        {
            ListViewMenu.SelectedItem = null; 
        }
    }
}