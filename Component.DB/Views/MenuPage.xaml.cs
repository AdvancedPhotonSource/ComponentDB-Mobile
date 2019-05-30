/*
 * Copyright (c) UChicago Argonne, LLC. All rights reserved.
 * See LICENSE file.
 */
using System.Collections.Generic;
using System.ComponentModel;
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
                new HomeMenuItem {Id = MenuItemType.ScanQRCode, Title="Scan QR Code" },
                new HomeMenuItem {Id = MenuItemType.BrowseCatalog, Title="Browse Catalog" },
                new HomeMenuItem {Id = MenuItemType.BrowseInventory, Title="Browse Inventory" },
                new HomeMenuItem {Id = MenuItemType.Settings, Title="Configuration" },
                new HomeMenuItem {Id = MenuItemType.About, Title="About" }
            };

            ListViewMenu.ItemsSource = menuItems;

            ListViewMenu.SelectedItem = menuItems[(int)MenuItemType.BrowseCatalog];
            ListViewMenu.ItemSelected += async (sender, e) =>
            {
                if (e.SelectedItem == null)
                    return;

                var id = (int)((HomeMenuItem)e.SelectedItem).Id;
                await RootPage.NavigateFromMenu(id);
            };
        }

        public void ClearSelection()
        {
            ListViewMenu.SelectedItem = null; 
        }
    }
}