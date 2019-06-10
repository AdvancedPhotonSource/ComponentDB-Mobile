﻿/*
 * Copyright (c) UChicago Argonne, LLC. All rights reserved.
 * See LICENSE file.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Component.DB.Services;
using Component.DB.ViewModels;
using Component.DB.Views.PreferencePages;
using Xamarin.Forms;

namespace Component.DB.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(true)]
    public partial class MainPage : MasterDetailPage
    {
        Dictionary<int, NavigationPage> MenuPages = new Dictionary<int, NavigationPage>();
        public MainPage()
        {
            InitializeComponent();

            MasterBehavior = MasterBehavior.Popover;
            MenuPages.Add((int)MenuItemType.BrowseCatalog, (NavigationPage)Detail);

            MessagingCenter.Subscribe<QrMessage>(this, QrMessage.MESSAGE_SCANNED_TOPIC, ItemScannedAction);
        }

        public async Task NavigateFromMenu(int id)
        {
            if (!MenuPages.ContainsKey(id))
            {
                switch (id)
                {
                    case (int)MenuItemType.ScanQRCode:
                        MenuPages.Add(id, new NavigationPage(new QrScannerPage()));
                        break;
                    case (int)MenuItemType.BrowseCatalog:
                        MenuPages.Add(id, new NavigationPage(new ItemsPage(MenuItemType.BrowseCatalog)));
                        break;
                    case (int)MenuItemType.BrowseInventory:
                        MenuPages.Add(id, new NavigationPage(new ItemsPage(MenuItemType.BrowseInventory)));
                        break;
                    case (int)MenuItemType.About:
                        MenuPages.Add(id, new NavigationPage(new AboutPage()));
                        break;
                    case (int)MenuItemType.Settings:
                        MenuPages.Add(id, new NavigationPage(new CdbConfigurationPage()));
                        break;
                    case ((int)MenuItemType.ItemDetails):
                        MenuPages.Add(id, new NavigationPage(new ItemDetailPage()));
                        break;
                }
            }

            NavigationPage newPage = MenuPages[id];
            await NavigateToNewPage(newPage);
        }

        public void ItemScannedAction(QrMessage message)
        {
            try
            {
                var qrId = message.ParseQrCode();
                var detailsModel = new ItemDetailViewModel();
                detailsModel.loadFromQrId(qrId);
                var detailsPage = new ItemDetailPage(detailsModel);
                var newPage = new NavigationPage(detailsPage);
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await NavigateToNewPageFromApp(newPage);
                });
            }
            catch (Exception ex)
            {
                var exMessage = CdbApiFactory.ParseApiException(ex);
                Debug.WriteLine(exMessage);
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await DisplayAlert(exMessage.SimpleName, exMessage.Message, "OK");
                });
            }


        }

        public async Task NavigateToNewPageFromApp(NavigationPage newPage)
        {
            MenuPageObj.ClearSelection();
            await NavigateToNewPage(newPage);
        }

        private async Task NavigateToNewPage(NavigationPage newPage)
        {
            if (newPage != null && Detail != newPage)
            {
                Detail = newPage;

                if (Device.RuntimePlatform == Device.Android)
                    await Task.Delay(100);

                IsPresented = false;
            }
        }
    }
}