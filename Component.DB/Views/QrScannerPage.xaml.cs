/*
 * Copyright (c) UChicago Argonne, LLC. All rights reserved.
 * See LICENSE file.
 */
using System;
using Component.DB.Services;
using Gov.ANL.APS.CDB.Api;
using Gov.ANL.APS.CDB.Model;
using Plugin.Vibrate;
using Xamarin.Forms;
using ZXing;

namespace Component.DB.Views
{
    public partial class QrScannerPage : CdbBasePage
    {
        ItemApi itemApi; 

        MainPage RootPage { get => Application.Current.MainPage as MainPage; }

        public QrScannerPage()
        {
            InitializeComponent();
            itemApi = CdbApiFactory.Instance.itemApi;
        }

        public async void Handle_OnScanResult(Result result)
        {
            string scanContents = result.Text;

            if (scanContents.Contains("qrId="))
            {
                var v = CrossVibrate.Current;
                v.Vibration(TimeSpan.FromSeconds(1));

                // Parse out the qr number for fetching.
                int qrIdStartIdx = scanContents.IndexOf("qrId=");

                if (qrIdStartIdx > 0)
                {
                    qrIdStartIdx += 5;
                    string qrIdStr = scanContents.Substring(qrIdStartIdx);

                    try
                    {
                        int qrId = Int32.Parse(qrIdStr);
                        Item item = await itemApi.GetItemByQrIdAsync(qrId);
                        NavigateToScannedItem(item);
                    } catch (Exception ex)
                    {
                        Device.BeginInvokeOnMainThread(async () =>
                        { 
                            HandleException(ex);
                        }); 
                    }
                }
            }
        }

        void NavigateToScannedItem(Item item)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                ItemDetailPage idp = new ItemDetailPage(new ViewModels.ItemDetailViewModel(item)); 
                await RootPage.NavigateToNewPageFromApp(new NavigationPage(idp));
            });
        }
    }
}
