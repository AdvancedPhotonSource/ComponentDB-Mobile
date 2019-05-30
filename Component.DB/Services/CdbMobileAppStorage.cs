/*
 * Copyright (c) UChicago Argonne, LLC. All rights reserved.
 * See LICENSE file.
 */
using System;
using System.IO;
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
            string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), SQLITE_DB_PATH);
            connection = new SQLiteConnection(dbPath);
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
