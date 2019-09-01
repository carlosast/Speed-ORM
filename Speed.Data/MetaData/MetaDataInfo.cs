//using System.Data;
//using System.Data.OleDb;

//namespace Speed.Data.MetaData
//{

//    /// <summary>
//    /// Metadata
//    /// </summary>
//    internal static class MetaDataInfo
//    {

//        public static DataTable GetReferentialConstrainsByInformationSchema(Database db)
//        {
//            string sql = "select * from INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS";
//            if (db.ProviderType == EnumDbProviderType.MySql)
//                sql = sql += " WHERE CONSTRAINT_SCHEMA = '" + db.DatabaseName + "'";
//            return db.ExecuteDataTable(sql);
//        }

//        public static DataTable GetReferentialConstrainsSQLite(Database db)
//        {
//            return GetTableContraintsSQLite(db);
//        }

//        public static DataTable GetReferentialConstrainsOleDb(Database db)
//        {
//            var cno = (OleDbConnection)db.Connection;
//            var tb = cno.GetOleDbSchemaTable(OleDbSchemaGuid.Referential_Constraints, new object[] { null, null, null });
//            return tb;
//        }

//        public static DataTable GetReferentialConstrainsOracle(Database db)
//        {
//            var tb = GetTableContraintsOracle(db);
//            foreach (DataRow row in tb.Select("CONSTRAINT_TYPE <> 'FOREIGN_KEY'"))
//                row.Delete();
//            tb.AcceptChanges();
//            return tb;

//        }

//        public static DataTable GetReferentialConstrainsFirebird(Database db)
//        {
//            string sql =
//@"SELECT DISTINCT rc.RDB$CONSTRAINT_TYPE CONSTRAINT_TYPE,
//    null CONSTRAINT_CATALOG, null CONSTRAINT_SCHEMA, rc.RDB$CONSTRAINT_NAME CONSTRAINT_NAME, null UNIQUE_CONSTRAINT_CATALOG, null UNIQUE_CONSTRAINT_SCHEMA, rc2.RDB$CONSTRAINT_NAME UNIQUE_CONSTRAINT_NAME
///*
//s.RDB$FIELD_NAME AS field_name,
//rc.RDB$CONSTRAINT_TYPE AS constraint_type,
//-- i.RDB$DESCRIPTION AS description,
//rc.RDB$DEFERRABLE AS is_deferrable,
//rc.RDB$INITIALLY_DEFERRED AS is_deferred,
//refc.RDB$UPDATE_RULE AS on_update,
//refc.RDB$DELETE_RULE AS on_delete,
//refc.RDB$MATCH_OPTION AS match_type,
//i2.RDB$RELATION_NAME AS references_table,
//-- s2.RDB$FIELD_NAME AS references_field,
//(s.RDB$FIELD_POSITION + 1) AS field_position,
//refc.RDB$CONST_NAME_UQ
//*/
//FROM RDB$INDEX_SEGMENTS s
//LEFT JOIN RDB$INDICES i ON i.RDB$INDEX_NAME = s.RDB$INDEX_NAME
//LEFT JOIN RDB$RELATION_CONSTRAINTS rc ON rc.RDB$INDEX_NAME = s.RDB$INDEX_NAME
//LEFT JOIN RDB$REF_CONSTRAINTS refc ON rc.RDB$CONSTRAINT_NAME = refc.RDB$CONSTRAINT_NAME
//LEFT JOIN RDB$RELATION_CONSTRAINTS rc2 ON rc2.RDB$CONSTRAINT_NAME = refc.RDB$CONST_NAME_UQ
//LEFT JOIN RDB$INDICES i2 ON i2.RDB$INDEX_NAME = rc2.RDB$INDEX_NAME
//-- LEFT JOIN RDB$INDEX_SEGMENTS s2 ON i2.RDB$INDEX_NAME = s2.RDB$INDEX_NAME
//WHERE
//-- i.RDB$RELATION_NAME='b' AND       -- table name
//-- rc.RDB$CONSTRAINT_NAME='FK_b' AND -- constraint name
//rc.RDB$CONSTRAINT_TYPE = 'FOREIGN KEY'
//ORDER BY s.RDB$FIELD_POSITION
//";
//            return db.ExecuteDataTable(sql);
//        }

//        public static DataTable GetForeign_Keys(Database db)
//        {
//            var cno = (OleDbConnection)db.Connection;
//            var tb = cno.GetOleDbSchemaTable(OleDbSchemaGuid.Foreign_Keys, new object[] { null, null, null, null });
//            return tb;
//        }

//        public static DataTable GetColumnConstraintsByInformationSchema(Database db)
//        {
//            string sql = "select * from INFORMATION_SCHEMA.KEY_COLUMN_USAGE";
//            if (db.ProviderType == EnumDbProviderType.MySql)
//                sql = sql += " WHERE TABLE_SCHEMA = '" + db.DatabaseName + "'";
//            sql += " ORDER BY CONSTRAINT_CATALOG, CONSTRAINT_SCHEMA, CONSTRAINT_NAME, ORDINAL_POSITION";
//            return db.ExecuteDataTable(sql);
//        }

//        public static DataTable GetColumnConstraintsOracle(Database db)
//        {
//            //DataTable tb = db.GetSchema("ForeignKeyColumns");
//            string sql =
//@"SELECT 
//    C.OWNER, C.CONSTRAINT_NAME, 
//    DECODE(CONSTRAINT_TYPE, 'C', 'CHECK',
//        'P', 'PRIMARY_KEY',
//        'U', 'UNIQUE_KEY',
//        'R', 'FOREIGN_KEY',
//        'V', 'VIEW',
//        'O', 'VIEW') CONSTRAINT_TYPE,
//    C.TABLE_NAME, C.R_OWNER, C.R_CONSTRAINT_NAME, C.INDEX_OWNER, C.INDEX_NAME, COL.COLUMN_NAME, COL.POSITION
//FROM USER_CONSTRAINTS C
//    INNER JOIN USER_CONS_COLUMNS COL ON C.CONSTRAINT_NAME = COL.CONSTRAINT_NAME 
//WHERE
//    CONSTRAINT_TYPE != 'C'
//";
//            DataTable tb = db.ExecuteDataTable(sql);
//            return tb;
//        }

//        public static DataTable GetColumnConstraintsSQLite(Database db)
//        {
//            DataTable tb = db.GetSchema("ForeignKeys");
//            return tb;

//            /*
//            DataTable tbfk = db.GetSchema("ForeignKeys");
//            DataTable tbin =db.GetSchema("Indexes");

//            tbin.Columns["INDEX_CATALOG"].ColumnName = "CONSTRAINT_CATALOG";
//            tbin.Columns["INDEX_SCHEMA"].ColumnName = "CONSTRAINT_SCHEMA";
//            tbin.Columns["INDEX_NAME"].ColumnName = "CONSTRAINT_NAME";

//            tbin.Columns.Add("FKEY_TO_CATALOG");
//            tbin.Columns.Add("FKEY_TO_SCHEMA");
//            tbin.Columns.Add("FKEY_TO_TABLE");
//            tbin.Columns.Add("FKEY_TO_COLUMN");

//            foreach (DataRow rowfk in tbfk.Rows)
//            {
//                // IndexColumns
//                // CONSTRAINT_CATALOG | CONSTRAINT_SCHEMA |     CONSTRAINT_NAME | TABLE_CATALOG | TABLE_SCHEMA |   TABLE_NAME |  COLUMN_NAME | ORDINAL_POSITION |                    INDEX_NAME | COLLATION_NAME | SORT_MODE | CONFLICT_OPTION
//                // ForeignKeys
//                // CONSTRAINT_CATALOG | CONSTRAINT_SCHEMA |     CONSTRAINT_NAME | TABLE_CATALOG | TABLE_SCHEMA |   TABLE_NAME | CONSTRAINT_TYPE | IS_DEFERRABLE | INITIALLY_DEFERRED | FKEY_ID | FKEY_FROM_COLUMN | FKEY_FROM_ORDINAL_POSITION | FKEY_TO_CATALOG | FKEY_TO_SCHEMA | FKEY_TO_TABLE | FKEY_TO_COLUMN | FKEY_ON_UPDATE | FKEY_ON_DELETE | FKEY_MATCH

//                var rowin = tbin.NewRow();
//                rowin["CONSTRAINT_CATALOG"] = rowfk["CONSTRAINT_CATALOG"];
//                rowin["CONSTRAINT_SCHEMA"] = rowfk["CONSTRAINT_SCHEMA"];
//                rowin["CONSTRAINT_NAME"] = rowfk["CONSTRAINT_NAME"];
//                rowin["TABLE_CATALOG"] = rowfk["TABLE_CATALOG"];
//                rowin["TABLE_SCHEMA"] = rowfk["TABLE_SCHEMA"];
//                rowin["TABLE_NAME"] = rowfk["TABLE_NAME"];
//                rowin["COLUMN_NAME"] = rowfk["FKEY_FROM_COLUMN"];
//                rowin["ORDINAL_POSITION"] = rowfk["FKEY_FROM_ORDINAL_POSITION"];

//                rowin["FKEY_TO_CATALOG"] = rowfk["FKEY_TO_CATALOG"];
//                rowin["FKEY_TO_SCHEMA"] = rowfk["FKEY_TO_SCHEMA"];
//                rowin["FKEY_TO_TABLE"] = rowfk["FKEY_TO_TABLE"];
//                rowin["FKEY_TO_COLUMN"] = rowfk["FKEY_TO_COLUMN"];

//                //bool isPrimeryKey = Conv.ToBoolean(rowin["PRIMARY_KEY"]);
//                //rowin["CONSTRAINT_TYPE"] = isPrimeryKey ? "PRIMARY KEY" : "INDEX";
//                //tbin.Rows.Add(rowfk);

//                tbin.Rows.Add(rowin);
//            }
//            return tbin;
//            */
//        }

//        public static DataTable GetColumnConstraintsOleDb(Database db)
//        {
//            var cno = (OleDbConnection)db.Connection;
//            var tb = cno.GetOleDbSchemaTable(OleDbSchemaGuid.Constraint_Column_Usage, new object[] { null, null, null, null });
//            return tb;
//        }

//        public static DataTable GetTableContraintsByInformationSchema(Database db)
//        {
//            string sql = "select * from INFORMATION_SCHEMA.TABLE_CONSTRAINTS";
//            if (db.ProviderType == EnumDbProviderType.MySql)
//                sql = sql += " WHERE TABLE_SCHEMA = '" + db.DatabaseName + "'";
//            return db.ExecuteDataTable(sql);
//        }

//        public static DataTable GetTableContraintsSQLite(Database db)
//        {
//            DataTable tbfk = db.GetSchema("ForeignKeys");
//            DataTable tbin = db.GetSchema("Indexes");

//            foreach (DataRow rowin in tbin.Rows)
//            {
//                // Indexes
//                // TABLE_CATALOG | TABLE_SCHEMA |   TABLE_NAME | INDEX_CATALOG | INDEX_SCHEMA |                    INDEX_NAME | PRIMARY_KEY | UNIQUE | CLUSTERED | TYPE | FILL_FACTOR | INITIAL_SIZE | NULLS | SORT_BOOKMARKS | AUTO_UPDATE | NULL_COLLATION | ORDINAL_POSITION | COLUMN_NAME | COLUMN_GUID | COLUMN_PROPID | COLLATION | CARDINALITY | PAGES | FILTER_CONDITION | INTEGRATED | INDEX_DEFINITION
//                // ForeignKeys
//                // CONSTRAINT_CATALOG | CONSTRAINT_SCHEMA |     CONSTRAINT_NAME | TABLE_CATALOG | TABLE_SCHEMA |   TABLE_NAME | CONSTRAINT_TYPE | IS_DEFERRABLE | INITIALLY_DEFERRED | FKEY_ID | FKEY_FROM_COLUMN | FKEY_FROM_ORDINAL_POSITION | FKEY_TO_CATALOG | FKEY_TO_SCHEMA | FKEY_TO_TABLE | FKEY_TO_COLUMN | FKEY_ON_UPDATE | FKEY_ON_DELETE | FKEY_MATCH

//                var rowfk = tbfk.NewRow();
//                rowfk["CONSTRAINT_CATALOG"] = rowin["INDEX_CATALOG"];
//                rowfk["CONSTRAINT_SCHEMA"] = rowin["INDEX_SCHEMA"];
//                rowfk["CONSTRAINT_NAME"] = rowin["INDEX_NAME"];
//                rowfk["TABLE_CATALOG"] = rowin["TABLE_CATALOG"];
//                rowfk["TABLE_SCHEMA"] = rowin["TABLE_SCHEMA"];
//                rowfk["TABLE_NAME"] = rowin["TABLE_NAME"];

//                bool isPrimeryKey = Conv.ToBoolean(rowin["PRIMARY_KEY"]);
//                rowfk["CONSTRAINT_TYPE"] = isPrimeryKey ? "PRIMARY KEY" : "INDEX";
//                tbfk.Rows.Add(rowfk);
//            }

//            tbfk.AcceptChanges();
//            return tbfk;
//        }

//        public static DataTable GetTableContraintsOracle(Database db)
//        {
//            string sql =
//@"
//SELECT DISTINCT CONSTRAINT_NAME,
//    DECODE(CONSTRAINT_TYPE, 'C', 'CHECK',
//        'P', 'PRIMARY_KEY',
//        'U', 'UNIQUE_KEY',
//        'R', 'FOREIGN_KEY',
//        'V', 'VIEW',
//        'O', 'VIEW') CONSTRAINT_TYPE,
//    TABLE_NAME,
//    OWNER CONSTRAINT_SCHEMA,
//    R_OWNER UNIQUE_CONSTRAINT_SCHEMA,
//    R_CONSTRAINT_NAME UNIQUE_CONSTRAINT_NAME
//    -- INDEX_OWNER UNIQUE_CONSTRAINT_SCHEMA,
//    -- INDEX_NAME UNIQUE_CONSTRAINT_NAME
//FROM
//    USER_CONSTRAINTS
//WHERE
//    CONSTRAINT_TYPE != 'C'
//";

//            DataTable tb = db.ExecuteDataTable(sql);
//            return tb;

//            #region Helper
//            //  OWNER |                  CONSTRAINT_NAME | CONSTRAINT_TYPE |                       TABLE_NAME | SEARCH_CONDITION | R_OWNER | R_CONSTRAINT_NAME | DELETE_RULE |    STATUS |       DEFERRABLE |    DEFERRED |   VALIDATED |        GENERATED |  BAD | RELY |           LAST_CHANGE | INDEX_OWNER |                       INDEX_NAME | INVALID | VIEW_RELATED
//            //'SPEED' | 'BIN$TuvvX5WpQfWyWJ8ncFJQWA==$0' |             'C' | 'BIN$8e4C91sPQCyqrHfJAkD7Lg==$0' |               '' |    NULL |              NULL |        NULL | 'ENABLED' | 'NOT DEFERRABLE' | 'IMMEDIATE' | 'VALIDATED' | 'GENERATED NAME' | NULL | NULL | '09/05/2013 15:08:42' |        NULL |                             NULL |    NULL |         NULL
//            //'SPEED' | 'BIN$cIpi6QiVQB2Gi5dign+/+w==$0' |             'C' | 'BIN$8e4C91sPQCyqrHfJAkD7Lg==$0' |               '' |    NULL |              NULL |        NULL | 'ENABLED' | 'NOT DEFERRABLE' | 'IMMEDIATE' | 'VALIDATED' | 'GENERATED NAME' | NULL | NULL | '09/05/2013 15:08:42' |        NULL |                             NULL |    NULL |         NULL
//            //'SPEED' | 'BIN$yrXCZtcVRp6ycrT36aV6dQ==$0' |             'P' | 'BIN$8e4C91sPQCyqrHfJAkD7Lg==$0' |             NULL |    NULL |              NULL |        NULL | 'ENABLED' | 'NOT DEFERRABLE' | 'IMMEDIATE' | 'VALIDATED' | 'GENERATED NAME' | NULL | NULL | '09/05/2013 15:08:42' |     'SPEED' | 'BIN$logiB/SLTA+NIAYNgzDZfw==$0' |    NULL |         NULL
//            //'SPEED' | 'BIN$+yhnjH8US9+TM4+K7QV65g==$0' |             'C' | 'BIN$+lf/qZHMSSmAWDtgE0PcmQ==$0' |               '' |    NULL |              NULL |        NULL | 'ENABLED' | 'NOT DEFERRABLE' | 'IMMEDIATE' | 'VALIDATED' |      'USER NAME' | NULL | NULL | '09/05/2013 15:10:36' |        NULL |                             NULL |    NULL |         NULL
//            //'SPEED' | 'BIN$Nlw3Bp9HS+qTclRbnRv4jA==$0' |             'C' | 'BIN$+lf/qZHMSSmAWDtgE0PcmQ==$0' |               '' |    NULL |              NULL |        NULL | 'ENABLED' | 'NOT DEFERRABLE' | 'IMMEDIATE' | 'VALIDATED' |      'USER NAME' | NULL | NULL | '09/05/2013 15:10:36' |        NULL |                             NULL |    NULL |         NULL
//            //'SPEED' | 'BIN$WgWH4ZIlRxuG9dSDt0Doow==$0' |             'P' | 'BIN$+lf/qZHMSSmAWDtgE0PcmQ==$0' |             NULL |    NULL |              NULL |        NULL | 'ENABLED' | 'NOT DEFERRABLE' | 'IMMEDIATE' | 'VALIDATED' |      'USER NAME' | NULL | NULL | '09/05/2013 15:10:36' |     'SPEED' | 'BIN$d6RG9gpLRhSghpmNTzGGRQ==$0' |    NULL |         NULL
//            //'SPEED' |                   'SYS_C0011064' |             'C' |                       'CUSTOMER' |               '' |    NULL |              NULL |        NULL | 'ENABLED' | 'NOT DEFERRABLE' | 'IMMEDIATE' | 'VALIDATED' | 'GENERATED NAME' | NULL | NULL | '09/05/2013 15:24:52' |        NULL |                             NULL |    NULL |         NULL
//            //'SPEED' | 'BIN$0utEbMiwS/OTfB4GSqrrCg==$0' |             'C' | 'BIN$yfZqiay1RIKcqxxE0zCV0Q==$0' |               '' |    NULL |              NULL |        NULL | 'ENABLED' | 'NOT DEFERRABLE' | 'IMMEDIATE' | 'VALIDATED' |      'USER NAME' | NULL | NULL | '09/05/2013 15:12:19' |        NULL |                             NULL |    NULL |         NULL
//            //'SPEED' | 'BIN$J9VhDEmHQX6Izz+G0A9vJQ==$0' |             'C' | 'BIN$yfZqiay1RIKcqxxE0zCV0Q==$0' |               '' |    NULL |              NULL |        NULL | 'ENABLED' | 'NOT DEFERRABLE' | 'IMMEDIATE' | 'VALIDATED' |      'USER NAME' | NULL | NULL | '09/05/2013 15:12:19' |        NULL |                             NULL |    NULL |         NULL
//            //'SPEED' | 'BIN$An6tgfdlQhaydQgNTyb3Vg==$0' |             'P' | 'BIN$yfZqiay1RIKcqxxE0zCV0Q==$0' |             NULL |    NULL |              NULL |        NULL | 'ENABLED' | 'NOT DEFERRABLE' | 'IMMEDIATE' | 'VALIDATED' |      'USER NAME' | NULL | NULL | '09/05/2013 15:12:19' |     'SPEED' | 'BIN$wEobQjQ7TQCRjb+FYjRydg==$0' |    NULL |         NULL
//            //'SPEED' |                   'SYS_C0011065' |             'C' |                       'CUSTOMER' |               '' |    NULL |              NULL |        NULL | 'ENABLED' | 'NOT DEFERRABLE' | 'IMMEDIATE' | 'VALIDATED' | 'GENERATED NAME' | NULL | NULL | '09/05/2013 15:24:52' |        NULL |                             NULL |    NULL |         NULL
//            //'SPEED' |                   'SYS_C0011066' |             'P' |                       'CUSTOMER' |             NULL |    NULL |              NULL |        NULL | 'ENABLED' | 'NOT DEFERRABLE' | 'IMMEDIATE' | 'VALIDATED' | 'GENERATED NAME' | NULL | NULL | '09/05/2013 15:24:52' |     'SPEED' |                   'SYS_C0011066' |    NULL |         NULL
//            //'SPEED' |                   'SYS_C0011067' |             'C' |                           'SALE' |               '' |    NULL |              NULL |        NULL | 'ENABLED' | 'NOT DEFERRABLE' | 'IMMEDIATE' | 'VALIDATED' | 'GENERATED NAME' | NULL | NULL | '09/05/2013 15:24:57' |        NULL |                             NULL |    NULL |         NULL
//            //'SPEED' |                   'SYS_C0011068' |             'C' |                           'SALE' |               '' |    NULL |              NULL |        NULL | 'ENABLED' | 'NOT DEFERRABLE' | 'IMMEDIATE' | 'VALIDATED' | 'GENERATED NAME' | NULL | NULL | '09/05/2013 15:24:57' |        NULL |                             NULL |    NULL |         NULL
//            //'SPEED' |                   'SYS_C0011069' |             'P' |                           'SALE' |             NULL |    NULL |              NULL |        NULL | 'ENABLED' | 'NOT DEFERRABLE' | 'IMMEDIATE' | 'VALIDATED' | 'GENERATED NAME' | NULL | NULL | '09/05/2013 15:24:57' |     'SPEED' |                   'SYS_C0011069' |    NULL |         NULL
//            //'SPEED' |                   'SYS_C0011070' |             'R' |                           'SALE' |             NULL | 'SPEED' |    'SYS_C0011066' |   'CASCADE' | 'ENABLED' | 'NOT DEFERRABLE' | 'IMMEDIATE' | 'VALIDATED' | 'GENERATED NAME' | NULL | NULL | '09/05/2013 15:24:57' |        NULL |                             NULL |    NULL |         NULL
//            //'SPEED' | 'BIN$zlyKnJr4TKWayc+1GisrNg==$0' |             'C' | 'BIN$Wc1XCCf4RymsVLcBHyS+UQ==$0' |               '' |    NULL |              NULL |        NULL | 'ENABLED' | 'NOT DEFERRABLE' | 'IMMEDIATE' | 'VALIDATED' |      'USER NAME' | NULL | NULL | '09/05/2013 15:25:02' |        NULL |                             NULL |    NULL |         NULL
//            //'SPEED' | 'BIN$Uzs9rvMCSfe3ezC22yE4ew==$0' |             'C' | 'BIN$Wc1XCCf4RymsVLcBHyS+UQ==$0' |               '' |    NULL |              NULL |        NULL | 'ENABLED' | 'NOT DEFERRABLE' | 'IMMEDIATE' | 'VALIDATED' |      'USER NAME' | NULL | NULL | '09/05/2013 15:25:02' |        NULL |                             NULL |    NULL |         NULL
//            //'SPEED' | 'BIN$OxzcZdG5S127UtLJfkkfhg==$0' |             'P' | 'BIN$Wc1XCCf4RymsVLcBHyS+UQ==$0' |             NULL |    NULL |              NULL |        NULL | 'ENABLED' | 'NOT DEFERRABLE' | 'IMMEDIATE' | 'VALIDATED' |      'USER NAME' | NULL | NULL | '09/05/2013 15:25:02' |     'SPEED' | 'BIN$KaS2w/QBQ62yDHceFQelSg==$0' |    NULL |         NULL
//            //'SPEED' |                   'SYS_C0011075' |             'C' |                    'SALE_DETAIL' |               '' |    NULL |              NULL |        NULL | 'ENABLED' | 'NOT DEFERRABLE' | 'IMMEDIATE' | 'VALIDATED' | 'GENERATED NAME' | NULL | NULL | '09/05/2013 15:26:13' |        NULL |                             NULL |    NULL |         NULL
//            //'SPEED' |                   'SYS_C0011076' |             'C' |                    'SALE_DETAIL' |               '' |    NULL |              NULL |        NULL | 'ENABLED' | 'NOT DEFERRABLE' | 'IMMEDIATE' | 'VALIDATED' | 'GENERATED NAME' | NULL | NULL | '09/05/2013 15:26:13' |        NULL |                             NULL |    NULL |         NULL
//            //'SPEED' |                   'SYS_C0011077' |             'P' |                    'SALE_DETAIL' |             NULL |    NULL |              NULL |        NULL | 'ENABLED' | 'NOT DEFERRABLE' | 'IMMEDIATE' | 'VALIDATED' | 'GENERATED NAME' | NULL | NULL | '09/05/2013 15:26:13' |     'SPEED' |                   'SYS_C0011077' |    NULL |         NULL
//            //'SPEED' |                   'SYS_C0011078' |             'R' |                    'SALE_DETAIL' |             NULL | 'SPEED' |    'SYS_C0011069' | 'NO ACTION' | 'ENABLED' | 'NOT DEFERRABLE' | 'IMMEDIATE' | 'VALIDATED' | 'GENERATED NAME' | NULL | NULL | '09/05/2013 15:26:13' |        NULL |                             NULL |    NULL |         NULL

//            //C (check constraint on a table)
//            //P (primary key)
//            //U (unique key)
//            //R (referential integrity)
//            //V (with check option, on a view)
//            //O (with read only, on a view)
//            #endregion Helper
//        }

//        public static DataTable GetTableContraintsOleDb(Database db)
//        {
//            var cno = (OleDbConnection)db.Connection;
//            var tb = cno.GetOleDbSchemaTable(OleDbSchemaGuid.Table_Constraints, new object[] { null, null, null, null });
//            return tb;
//        }

//        public static DataTable GetTableContraintsFirebird(Database db)
//        {
//            DataTable tbfk = db.GetSchema("ForeignKeys");
//            if (!tbfk.Columns.Contains("CONSTRAINT_TYPE"))
//                tbfk.Columns.Add("CONSTRAINT_TYPE", typeof(string));

//            DataTable tbin = db.GetSchema("Indexes");

//            foreach (DataRow rowin in tbin.Rows)
//            {
//                // Indexes
//                // TABLE_CATALOG | TABLE_SCHEMA |   TABLE_NAME | INDEX_CATALOG | INDEX_SCHEMA |                    INDEX_NAME | PRIMARY_KEY | UNIQUE | CLUSTERED | TYPE | FILL_FACTOR | INITIAL_SIZE | NULLS | SORT_BOOKMARKS | AUTO_UPDATE | NULL_COLLATION | ORDINAL_POSITION | COLUMN_NAME | COLUMN_GUID | COLUMN_PROPID | COLLATION | CARDINALITY | PAGES | FILTER_CONDITION | INTEGRATED | INDEX_DEFINITION
//                // ForeignKeys
//                // CONSTRAINT_CATALOG | CONSTRAINT_SCHEMA |     CONSTRAINT_NAME | TABLE_CATALOG | TABLE_SCHEMA |   TABLE_NAME | CONSTRAINT_TYPE | IS_DEFERRABLE | INITIALLY_DEFERRED | FKEY_ID | FKEY_FROM_COLUMN | FKEY_FROM_ORDINAL_POSITION | FKEY_TO_CATALOG | FKEY_TO_SCHEMA | FKEY_TO_TABLE | FKEY_TO_COLUMN | FKEY_ON_UPDATE | FKEY_ON_DELETE | FKEY_MATCH

//                var rowfk = tbfk.NewRow();
//                rowfk["CONSTRAINT_CATALOG"] = Conv.Trim(rowin["TABLE_CATALOG"]);
//                rowfk["CONSTRAINT_SCHEMA"] = Conv.Trim(rowin["TABLE_SCHEMA"]);
//                rowfk["CONSTRAINT_NAME"] = Conv.Trim(rowin["INDEX_NAME"]);
//                rowfk["TABLE_CATALOG"] = Conv.Trim(rowin["TABLE_CATALOG"]);
//                rowfk["TABLE_SCHEMA"] = Conv.Trim(rowin["TABLE_SCHEMA"]);
//                rowfk["TABLE_NAME"] = Conv.Trim(rowin["TABLE_NAME"]);

//                bool isPrimaryKey = false;
//                if (tbin.Columns.Contains("PRIMARY_KEY"))
//                    isPrimaryKey = Conv.ToBoolean(rowin["PRIMARY_KEY"]);
//                if (tbin.Columns.Contains("IS_UNIQUE"))
//                    isPrimaryKey = Conv.ToBoolean(rowin["IS_UNIQUE"]);

//                rowfk["CONSTRAINT_TYPE"] = isPrimaryKey ? "PRIMARY KEY" : "INDEX";
//                tbfk.Rows.Add(rowfk);
//            }

//            tbfk.AcceptChanges();
//            return tbfk;
//        }

//    }

//}
