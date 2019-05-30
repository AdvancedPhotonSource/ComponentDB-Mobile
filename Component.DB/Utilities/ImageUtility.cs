/*
 * Copyright (c) UChicago Argonne, LLC. All rights reserved.
 * See LICENSE file.
 */
using System;
using Component.DB.Services;

namespace Component.DB.Utilities
{
    public class ImageUtility
    {
        public ImageUtility()
        {

        }

        public static string getImageUrl(string imageName, string imageType)
        {
            return CdbMobileAppStorage.ActiveConfigurationHostAddress 
                + Constants.ImageAddressPath + imageName  + imageType;
        }
    }
}
