/*
 * Copyright (c) UChicago Argonne, LLC. All rights reserved.
 * See LICENSE file.
 */
using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Plugin.CurrentActivity;
#if EMDK
using Symbol.XamarinEMDK;
using Symbol.XamarinEMDK.Barcode;
#endif
using System.Collections;
using System.Collections.Generic;
using Xamarin.Forms;
using Component.DB.Services;

namespace Component.DB.Droid
{
    [Activity(Label = "Component.DB", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
#if EMDK
        , EMDKManager.IEMDKListener
#endif
    {

#if EMDK
        EMDKManager emdkManager = null;
        BarcodeManager barcodeManager = null;
        Scanner scanner = null;
#endif

        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);

            ZXing.Net.Mobile.Forms.Android.Platform.Init();
            Stormlion.PhotoBrowser.Droid.Platform.Init(this);
            CrossCurrentActivity.Current.Init(this, savedInstanceState);
#if EMDK
            setUpZebraScanner();
#endif

            LoadApplication(new App());
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            Plugin.Permissions.PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

#if EMDK
        private void setUpZebraScanner()
        {
            try
            {
                EMDKResults results = EMDKManager.GetEMDKManager(Android.App.Application.Context, this);
                if (results.StatusCode != EMDKResults.STATUS_CODE.Success)
                {
                    Console.WriteLine("Status: EMDKManager object creation failed ...");
                }
                else
                {
                    Console.WriteLine("Status: EMDKManager object creation succeeded ...");
                    Toast.MakeText(Android.App.Application.Context, "Scan a cdb qrid at any time using the built in scanner.", ToastLength.Long).Show();
                    Toast.MakeText(Android.App.Application.Context, "Scan a cdb qrid at any time using the built in scanner.", ToastLength.Long).Show();
                }
            } catch (Exception e)
            {
                Console.WriteLine(e);
            }

        }

        /// <summary>
        /// When Zebra scanner connection colses. 
        /// </summary>
        public void OnClosed()
        {
            if (emdkManager != null)
            {
                emdkManager.Release();
                emdkManager = null;
            }
        }

        /// <summary>
        /// When Zebra scanner connection opens. 
        /// </summary>
        /// <param name="emdkManager">P0.</param>
        public void OnOpened(EMDKManager emdkManager)
        {
            this.emdkManager = emdkManager;

            InitScanner();
        }

        void DeinitScanner()
        {
            if (emdkManager != null)
            {

                if (scanner != null)
                {
                    try
                    {

                        scanner.Data -= scanner_Data;
                        scanner.Status -= scanner_Status;

                        scanner.Disable();


                    }
                    catch (ScannerException e)
                    {
                        Console.WriteLine(this.Class.SimpleName, "Exception:" + e.Result.Description);
                    }
                }

                if (barcodeManager != null)
                {
                    emdkManager.Release(EMDKManager.FEATURE_TYPE.Barcode);
                }
                barcodeManager = null;
                scanner = null;
            }
        }


        void InitScanner()
        {
            if (emdkManager != null)
            {

                if (barcodeManager == null)
                {
                    try
                    {

                        //Get the feature object such as BarcodeManager object for accessing the feature.
                        barcodeManager = (BarcodeManager)emdkManager.GetInstance(EMDKManager.FEATURE_TYPE.Barcode);

                        scanner = barcodeManager.GetDevice(BarcodeManager.DeviceIdentifier.Default);

                        if (scanner != null)
                        {

                            //Attahch the Data Event handler to get the data callbacks.
                            scanner.Data += scanner_Data;

                            //Attach Scanner Status Event to get the status callbacks.
                            scanner.Status += scanner_Status;

                            scanner.Enable();

                            //EMDK: Configure the scanner settings
                            ScannerConfig config = scanner.GetConfig();
                            config.SkipOnUnsupported = ScannerConfig.SkipOnUnSupported.None;
                            config.ScanParams.DecodeLEDFeedback = true;
                            config.ReaderParams.ReaderSpecific.ImagerSpecific.PickList = ScannerConfig.PickList.Enabled;
                            config.DecoderParams.Code39.Enabled = true;
                            config.DecoderParams.Code128.Enabled = false;
                            scanner.SetConfig(config);

                        }
                        else
                        {
                            Console.WriteLine("Failed to enable scanner.\n");
                        }
                    }
                    catch (ScannerException e)
                    {
                        Console.WriteLine("Error: " + e.Message);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error: " + ex.Message);
                    }
                }
            }
        }


        void scanner_Data(object sender, Scanner.DataEventArgs e)
        {
            ScanDataCollection scanDataCollection = e.P0;

            if ((scanDataCollection != null) && (scanDataCollection.Result == ScannerResults.Success))
            {
                IList<ScanDataCollection.ScanData> scanData = scanDataCollection.GetScanData();

                foreach (ScanDataCollection.ScanData data in scanData)
                {
                    Console.WriteLine(data.LabelType + " : " + data.Data);
                    var topic = QrMessage.MESSAGE_SCANNED_TOPIC;
                    var message = new QrMessage(data.LabelType.ToString(), data.Data);

                    MessagingCenter.Send<QrMessage>(message, topic); 
                }
            }
        }

        void scanner_Status(object sender, Scanner.StatusEventArgs e)
        {
            String statusStr = "";

            //EMDK: The status will be returned on multiple cases. Check the state and take the action.
            StatusData.ScannerStates state = e.P0.State;

            if (state == StatusData.ScannerStates.Idle)
            {
                statusStr = "Scanner is idle and ready to submit read.";
                try
                {
                    if (scanner.IsEnabled && !scanner.IsReadPending)
                    {   
                        scanner.Read();
                    }
                }
                catch (ScannerException e1)
                {
                    statusStr = e1.Message;
                }
            }
            if (state == StatusData.ScannerStates.Waiting)
            {
                statusStr = "Waiting for Trigger Press to scan";
            }
            if (state == StatusData.ScannerStates.Scanning)
            {
                statusStr = "Scanning in progress...";
            }
            if (state == StatusData.ScannerStates.Disabled)
            {
                statusStr = "Scanner disabled";
            }
            if (state == StatusData.ScannerStates.Error)
            {
                statusStr = "Error occurred during scanning";

            }
            Console.WriteLine(statusStr);
        }

        protected override void OnResume()
        {
            base.OnResume();
            InitScanner();
        }


        protected override void OnPause()
        {
            base.OnPause();
            DeinitScanner();
        }



        protected override void OnDestroy()
        {
            base.OnDestroy();

            //Clean up the emdkManager
            if (emdkManager != null)
            {
                //EMDK: Release the EMDK manager object
                emdkManager.Release();
                emdkManager = null;
            }
        }
#endif
    }
}