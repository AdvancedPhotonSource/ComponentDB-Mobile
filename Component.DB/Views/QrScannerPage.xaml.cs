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
            var v = CrossVibrate.Current;
            v.Vibration(TimeSpan.FromSeconds(0.5));

            string scanContents = result.Text;
            var topic = QrMessage.MESSAGE_SCANNED_TOPIC;
            QrMessage message = new QrMessage(result.BarcodeFormat.ToString(), result.Text);

            MessagingCenter.Send<QrMessage>(message, topic); 
        }

    }
}
