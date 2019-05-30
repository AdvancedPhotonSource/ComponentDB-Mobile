/*
 * Copyright (c) UChicago Argonne, LLC. All rights reserved.
 * See LICENSE file.
 */
using System;
using SQLite;

namespace Component.DB.Services.CdbMobileAppStoreModel
{
    [Table("User_Authentication")]
    public class UserAuthentication
    {
        [PrimaryKey, AutoIncrement, Column("id")]
        public int Id { get; set; }

        [MaxLength(64)]
        public string Username { get; set; }

        [MaxLength(64)]
        public string AuthToken { get; set; }

        public int applicableConfigurationId { get; set; }
    }
}
