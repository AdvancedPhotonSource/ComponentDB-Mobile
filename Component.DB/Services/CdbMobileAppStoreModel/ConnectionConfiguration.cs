/*
 * Copyright (c) UChicago Argonne, LLC. All rights reserved.
 * See LICENSE file.
 */
using System;
using SQLite;

namespace Component.DB.Services.CdbMobileAppStoreModel
{

    [Table("Connection_Configuration")]
    public class ConnectionConfiguration
    {
        [PrimaryKey, AutoIncrement, Column("id")]
        public int Id { get; set; }

        [MaxLength(32)]
        public string Name { get; set; }

        [MaxLength(128)]
        public string Description { get; set; }

        [MaxLength(128)]
        public string CdbAddress { get; set; }

        public int ActiveAuthId { get; set; }
    }
}
