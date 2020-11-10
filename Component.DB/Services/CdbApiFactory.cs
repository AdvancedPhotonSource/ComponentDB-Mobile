/*
 * Copyright (c) UChicago Argonne, LLC. All rights reserved.
 * See LICENSE file.
 */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Gov.ANL.APS.CDB.Api;
using Gov.ANL.APS.CDB.Client;
using Gov.ANL.APS.CDB.Model;
using Newtonsoft.Json;
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
        private LocationItemsApi locationApiInstance; 
        private PropertyValueApi propertyValueApiInstance;
        private PropertyTypeApi propertyTypeApiInstance;  
        private UsersApi usersApiInstace;
        private TestApi testApiInstance; 

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
            locationApiInstance = new LocationItemsApi(host);
            authApiInstance = new AuthenticationApi(host);
            propertyValueApiInstance = new PropertyValueApi(host);
            usersApiInstace = new UsersApi(host);
            testApiInstance = new TestApi(host);
            propertyTypeApiInstance = new PropertyTypeApi(host); 

            if (auth != null)
            {
                var token = auth.AuthToken;
                applyAuthToken(token); 
            }
        }

        public void LogoutActiveUser()
        {
            
            try
            {
                authApiInstance.LogOut();
            } catch (ApiException ex)
            {                
                if (ex.ErrorCode == 401)
                {
                    // Ignore unauthorized, the user must have invalid session already.
                    Debug.WriteLine(ex);
                } else
                {
                    throw ex;
                }
            }
            
            mobileAppStorage.clearActiveAuth();
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
            locationApiInstance.Configuration.AddDefaultHeader(TOKEN_KEY, token);
            propertyTypeApi.Configuration.AddDefaultHeader(TOKEN_KEY, token);
            authApiInstance.Configuration.AddDefaultHeader(TOKEN_KEY, token);
            propertyValueApiInstance.Configuration.AddDefaultHeader(TOKEN_KEY, token);
            usersApi.Configuration.AddDefaultHeader(TOKEN_KEY, token);
            testApiInstance.Configuration.AddDefaultHeader(TOKEN_KEY, token);
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

        public LocationItemsApi locationItemApi
        {
            get
            {
                return locationApiInstance; 
            }
        }

        public PropertyValueApi propertyValueApi
        {
            get
            {
                return propertyValueApiInstance;
            }
        }

        public PropertyTypeApi propertyTypeApi
        {
            get
            {
                return propertyTypeApiInstance; 
            }
        }


        public UsersApi usersApi
        {
            get
            {
                return usersApiInstace;
            }
        }

        public TestApi testApi
        {
            get
            {
                return testApiInstance; 
            }
        }

        public static String ConvertStreamDataToBase64(Stream stream)
        {
            // Now read s into a byte buffer with a little padding.
            byte[] bytes = new byte[stream.Length + 10];
            int numBytesToRead = (int)stream.Length;
            int numBytesRead = 0;
            do
            {
                // Read may return anything from 0 to 10.
                int n = stream.Read(bytes, numBytesRead, 10);
                numBytesRead += n;
                numBytesToRead -= n;
            } while (numBytesToRead > 0);
            stream.Close();

            return Convert.ToBase64String(bytes);
        }

        public static ApiExceptionMessage ParseApiException(Exception exception) 
        {
            ApiExceptionMessage exceptionMessage = null;

            if (exception.GetType() == typeof(ApiException))
            {
                ApiException apiException = (Gov.ANL.APS.CDB.Client.ApiException)exception;
                if (apiException.ErrorCode == 0)
                {
                    exceptionMessage = new ApiExceptionMessage
                    {
                        SimpleName = "Connection Error",
                        Message = exception.Message
                    };
                } else
                {
                    var json = apiException.ErrorContent;
                    try
                    {
                        exceptionMessage = JsonConvert.DeserializeObject<ApiExceptionMessage>(json);
                    } catch (Exception ex)
                    {
                        Debug.WriteLine(ex);
                    }
                }
            } 

            if (exceptionMessage == null)
            {
                exceptionMessage = new ApiExceptionMessage
                {
                    SimpleName = "Error",
                    Message = exception.Message 
                };
            }

            return exceptionMessage;
        }
    }
}
