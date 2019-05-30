/*
 * Copyright (c) UChicago Argonne, LLC. All rights reserved.
 * See LICENSE file.
 */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Gov.ANL.APS.CDB.Api;
using Gov.ANL.APS.CDB.Client;
using Xamarin.Essentials;

namespace Component.DB.Services
{
    public sealed class CdbApiFactory
    {
        // Singleton instnance variable
        private static readonly Lazy<CdbApiFactory> lazy = new Lazy<CdbApiFactory>(() => new CdbApiFactory());

        private const string TOKEN_KEY = "token";
        private const string CONN_REFUSED_ERR = "Error: ConnectFailure (Connection refused)";

        private CdbMobileAppStorage mobileAppStorage;

        private AuthenticationApi authApiInstance;
        private ItemApi itemApiInstance;
        private PropertyApi propertyApiInstance;
        private UsersApi usersApiInstace;

        public static CdbApiFactory Instance
        {
            get
            {
                return lazy.Value;
            }
        }

        private CdbApiFactory()
        {
            setActiveHost(); 
        }

        public void setActiveHost()
        {
            mobileAppStorage = CdbMobileAppStorage.Instance;
            var config = mobileAppStorage.getActiveConfiguration();
            var auth = mobileAppStorage.getActiveAuth();
            var host = config.CdbAddress; 

            itemApiInstance = new ItemApi(host);
            authApiInstance = new AuthenticationApi(host);
            propertyApiInstance = new PropertyApi(host);
            usersApiInstace = new UsersApi(host);

            if (auth != null)
            {
                var token = auth.AuthToken;
                applyAuthToken(token); 
            }
        }

        public async Task<bool> authenticateUserAsync(string username, string password)
        {
            ApiResponse<object> resp = null;
            try
            {
                resp = await authApiInstance.AuthenticateUserAsyncWithHttpInfo(username, password);
            } catch (ApiException ex)
            {
                Debug.WriteLine(ex);

                var content = ex.ErrorContent;

                string errorStr = content.ToString(); 

                if (errorStr.Equals(CONN_REFUSED_ERR))
                {
                    throw new Exception("Connection to CDB server was refused");
                }
                return false;
            }

            var headers = resp.Headers;

            if (headers.ContainsKey(TOKEN_KEY))
            {
                var token = headers[TOKEN_KEY];

                applyAuthToken(token);

                if ((bool)await verifyUserAuthenticated())
                {
                    mobileAppStorage.addActiveAuthToken(token, username);
                    return true;
                }
            }
            return false;
        }

        private void applyAuthToken(String token)
        {
            itemApiInstance.Configuration.AddDefaultHeader(TOKEN_KEY, token);
            authApiInstance.Configuration.AddDefaultHeader(TOKEN_KEY, token);
            propertyApiInstance.Configuration.AddDefaultHeader(TOKEN_KEY, token);
            usersApi.Configuration.AddDefaultHeader(TOKEN_KEY, token);
        }

        public async Task<bool?> verifyUserAuthenticated()
        {
            try
            {
                return await authApiInstance.VerifyAuthenticatedAsync();
            } catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            return false;
        }

        public static async Task<bool> verifyConnectionAsync(String host)
        {        
            try
            {
                TestApi test = new TestApi(host);
                bool? result = await test.VerifyConnectionAsync();
                return (bool)result; 
            } catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            return false;
        }

        public ItemApi itemApi
        {
            get
            {
                return itemApiInstance;
            }
        }

        public PropertyApi propertyApi
        {
            get
            {
                return propertyApiInstance;
            }
        }


        public UsersApi usersApi
        {
            get
            {
                return usersApiInstace;
            }
        }
    }
}
