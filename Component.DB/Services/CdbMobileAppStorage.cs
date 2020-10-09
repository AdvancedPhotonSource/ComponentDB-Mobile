/*
 * Copyright (c) UChicago Argonne, LLC. All rights reserved.
 * See LICENSE file.
 */
using System;
using System.ComponentModel;
using System.IO;
using Component.DB.Services.CdbEventArgs;
using Component.DB.Services.CdbMobileAppStoreModel;
using SQLite;
using Xamarin.Essentials;

namespace Component.DB.Services
{
    public class CdbMobileAppStorage
    {
        private static readonly Lazy<CdbMobileAppStorage> lazy = new Lazy<CdbMobileAppStorage>(() => new CdbMobileAppStorage());
        private static string _ActiveConfigurationHostAddress;

        private const string SQLITE_DB_PATH = "localstore.db";
        private const string ACTIVE_CONFIGURATION_ID_KEY = "ActiveConfiguration";

        public enum ScanningAction
        {
            DetailsPage,
            RelocateItem
        }

        public enum BrowseMode
        {
            All,
            Favorites
        }

        private const string DEFAULT_SCANNING_ACTION_KEY = "DefaultScanningAction";
        private const string BROWSE_CATALOG_MODE_KEY = "BrowseCatalogMode";

        public event EventHandler<BrowseModeChangeEventArgs> CatalogBrowseModeChangedEvent;

        private SQLiteConnection connection;
        private ConnectionConfiguration ActiveConfiguration;

        public static CdbMobileAppStorage Instance
        {
            get
            {
                return lazy.Value;
            }
        }

        private CdbMobileAppStorage()
        {
            var appPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData); 
            string dbPath = Path.Combine(appPath, SQLITE_DB_PATH);
            Directory.CreateDirectory(appPath); 
            connection = new SQLiteConnection(dbPath);
        }

        public void UpdateCatalogBrowseMode(BrowseMode browseMode)
        {
            Preferences.Set(BROWSE_CATALOG_MODE_KEY, (int)browseMode);

            if (CatalogBrowseModeChangedEvent != null)
            {
                var args = new BrowseModeChangeEventArgs(browseMode);
                CatalogBrowseModeChangedEvent(this, args);
            }
        }

        public BrowseMode GetCatalogBrowseMode()
        {
            var result = Preferences.Get(BROWSE_CATALOG_MODE_KEY, (int)BrowseMode.All);
            return (BrowseMode)result;
        }

        public void UpdateScanningAction(ScanningAction action)
        {
            Preferences.Set(DEFAULT_SCANNING_ACTION_KEY, (int)action);
        }

        public ScanningAction GetScanningAction()
        {
            var result = Preferences.Get(DEFAULT_SCANNING_ACTION_KEY, (int)ScanningAction.DetailsPage); 
            return (ScanningAction)result; 
        }

        public void addConnection(string hostPath)
        {
            connection.CreateTable<ConnectionConfiguration>();

            ConnectionConfiguration config = new ConnectionConfiguration();
            config.CdbAddress = hostPath;
            connection.Insert(config);

            Preferences.Set(ACTIVE_CONFIGURATION_ID_KEY, config.Id);
        }

        public void addActiveAuthToken(String token, String username)
        {
            connection.CreateTable<UserAuthentication>();

            var activeConfig = getActiveConfiguration();
            UserAuthentication auth = new UserAuthentication
            {
                Username = username,
                AuthToken = token,
                applicableConfigurationId = activeConfig.Id
            };


            connection.Insert(auth);

            activeConfig.ActiveAuthId = auth.Id;
            connection.Update(activeConfig);
        }

        public UserAuthentication getActiveAuth()
        {
            var config = getActiveConfiguration();

            if (config.ActiveAuthId > 0)
            {
                return connection.Get<UserAuthentication>(config.ActiveAuthId);
            }

            return null;
        }

        public ConnectionConfiguration getActiveConfiguration()
        {
            if (ActiveConfiguration == null)
            {
                if (Preferences.ContainsKey(ACTIVE_CONFIGURATION_ID_KEY))
                {
                    int id = Preferences.Get(ACTIVE_CONFIGURATION_ID_KEY, -1);
                    ActiveConfiguration = connection.Get<ConnectionConfiguration>(id);
                    _ActiveConfigurationHostAddress = ActiveConfiguration.CdbAddress;
                }
            }
            return ActiveConfiguration;
        }

        public void clearActiveAuth()
        {
            var activeAuth = getActiveAuth();
            connection.Delete(activeAuth);

            var config = getActiveConfiguration();
            config.ActiveAuthId = -1;

            connection.Update(config);
        }

        public String CdbHostAddress
        {
            get
            {
                var config = getActiveConfiguration();
                if (config != null)
                {
                    return config.CdbAddress;
                }
                return null;
            }
        }

        public static string ActiveConfigurationHostAddress
        {
            get
            {
                return _ActiveConfigurationHostAddress; 
            }
        }

    }
}
