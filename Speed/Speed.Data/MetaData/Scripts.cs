//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace Speed.Data.MetaData
//{
//    //TODO: trazer DbSqlDatabase de SqlDbtools pra cá e dar suporte a multi-database

//    public static class Scripts
//    {

//        #region Scripts

//        public static string scptAllObjects =
//@"
//select ObjectName = o.name,
//	SchemaName = user_name(o.uid),
//	IsTable = OBJECTPROPERTY(o.id, N'IsTable'),
//	DateCreated = o.crdate,
//	ObjectId = o.id, 
//    parent_obj OwnerId,
//    o.xtype xtype,
//	N'IsSystemObject' =  IsNull((case when (OBJECTPROPERTY(o.id, N'IsMSShipped')=1) then 1 else OBJECTPROPERTY(o.id, N'IsSystemObject') end ), 0),
//	o.category Category, 
//	N'IsFakeTable' = (case when (OBJECTPROPERTY(o.id, N'tableisfake')=1) then 1 else 0 end),
//	(case when (OBJECTPROPERTY(o.id, N'IsQuotedIdentOn')=1) then 1 else 0 end) IsQuotedIdentOn,
//	(case when (OBJECTPROPERTY(o.id, N'IsAnsiNullsOn')=1) then 1 else 0 end) IsAnsiNullsOn
//from  
//	dbo.sysobjects o
//where 
//    o.xtype like '{0}' 
//	and not (o.xtype = 'P' and substring(o.name, 1, 3) = 'dt_')
//order by 
//    ObjectName, SchemaName  
//";

//        public static string scptAllTexts = "select o.id ObjectId, text 'Text' from syscomments c join sysobjects o on c.id = o.id where o.xtype not in ('S', 'U') and encrypted = 0 order by o.id, colid";

//        public static string scptAllColumns80 =
//@"
//select 
//	o.id OwnerId, 
//	o.name OwnerName,
//	db_name() OwnerCatalog,
//	user_name(o.uid) OwnerSchemaName,
//	c.colid ColumnId,
//	c.name ColumnName, 
//	c.xtype DataTypeId,
//	t.name DataTypeName,
//	IsNull((select [text] from syscomments m where m.id = c.cdefault), '') DefaultValue,
//	IsNull((select top 1 [text] from syscomments m where m.id = o.id and m.number = c.colid), '') Formula,
//	c.xusertype UserTypeId,
//	case when t.name = 'xml' then '' when columnproperty(o.id, c.name, 'Precision') = -1 then 'MAX' else cast(columnproperty(o.id, c.name, 'Precision') as varchar) end RealLength,
//	c.length Length,
//	c.xprec [Precision],
//	IsNull(c.xscale, 0) Scale,
//	c.colid OrdinalPosition,
//	c.domain Domain,
//	IsNull(columnproperty(o.id, c.name, 'IsIdentity'), 0) IsIdentity,
//	iscomputed IsComputed,
//	isoutparam IsOutParam,
//	isnullable IsNullable,
//	IsNull(c.collation, '') Collation
//FROM
//	sysobjects AS o INNER JOIN
//    syscolumns AS c ON c.id = o.id INNER JOIN
//    systypes AS t ON c.xtype = t.xtype and c.usertype = t.usertype
//order 
//    by o.id, c.colid
//";


//        public static string scptAllColumns90 =
//@"
//select 
//	o.id OwnerId, 
//	o.name OwnerName,
//	db_name() OwnerCatalog,
//	schema_name(o.uid) OwnerSchemaName,
//	c.colid ColumnId,
//	c.name ColumnName, 
//	c.xtype DataTypeId,
//	t.name DataTypeName,
//	IsNull((select [text] from syscomments m where m.id = c.cdefault), '') DefaultValue,
//	IsNull((select top 1 [text] from syscomments m where m.id = o.id and m.number = c.colid), '') Formula,
//	c.xusertype UserTypeId,
//	case when t.name = 'xml' then '' when columnproperty(o.id, c.name, 'Precision') = -1 then 'MAX' else cast(columnproperty(o.id, c.name, 'Precision') as varchar) end RealLength,
//	c.length Length,
//	c.xprec Precision,
//	IsNull(c.xscale, 0) Scale,
//	c.colid OrdinalPosition,
//	c.domain Domain,
//	IsNull(columnproperty(o.id, c.name, 'IsIdentity'), 0) IsIdentity,
//	iscomputed IsComputed,
//	isoutparam IsOutParam,
//	isnullable IsNullable,
//	IsNull(c.collation, '') Collation
//FROM
//	sysobjects AS o INNER JOIN
//    syscolumns AS c ON c.id = o.id INNER JOIN
//    systypes AS t ON c.xtype = t.xtype and c.usertype = t.usertype
//order 
//    by o.id, c.colid
//";


//        public static string scptAllIndexes80 =
//@"
//set nocount on
//SELECT  -- TOP 100
//  I.INDID AS IndexId,
//  I.NAME AS IndexName,
//  I.ID AS ObjectId,
//  OBJECT_NAME(I.ID) AS ObjectName,
//  REPLICATE(' ',4000) AS [ColumnsList] ,
//  INDEXPROPERTY (I.ID,I.NAME,'IsUnique') AS IsUnique,
//  case when OBJECTPROPERTY(i.id, 'IsTable') = 1 then 1 else 0 end IsTable,
//  I.Status as Status,
//  case when OBJECTPROPERTY(object_id(i.name), 'IsPrimaryKey ') = 1 then 1 else 0 end IsPrimaryKey,
//  INDEXPROPERTY (I.ID,I.NAME,'IsClustered') AS IsClustered,
//  INDEXPROPERTY (I.ID,I.NAME,'IsAutoStatistics ') AS IsAutoStatistics,
//  INDEXPROPERTY (I.ID,I.NAME,'IsDisabled') AS IsDisabled,
//  INDEXPROPERTY (I.ID,I.NAME,'IsHypothetical') AS IsHypothetical,
//  (select count(*) from sysobjects where name = i.name and xtype = 'UQ')  AS IsUniqueConstraint,
//  INDEXPROPERTY (I.ID,I.NAME,'IsFulltextKey') AS IsFulltextKey,
//  INDEXPROPERTY (I.ID,I.NAME,'IsStatistics') AS IsStatistics,
//  INDEXPROPERTY (I.ID,I.NAME,'IsMSShipped') AS IsMSShipped,
//  case when INDEXPROPERTY( i.id , i.name , 'IsAutoStatistics' ) = 1 or 
//			INDEXPROPERTY( i.id , i.name , 'IsStatistics'   ) = 1 or
//			INDEXPROPERTY( i.id , i.name , 'IsHypothetical'   ) = 1 or 
//			INDEXPROPERTY( i.id , i.name , 'IsMSShipped'   ) = 1 then 1 else 0 end as IsSystemObject,
//  IsNull(INDEXPROPERTY (I.ID,I.NAME,'[FILLFACTOR]'), 0) AS [Fillfactor],
//  filegroup_name(groupid) FileGroup
//  INTO #TMP
//  FROM SYSINDEXES I
//  WHERE I.INDID > 0 
//  AND I.INDID < 255 
//  AND (I.STATUS & 64)=0
//  AND OBJECTPROPERTY(i.id, 'IsUserTable') = 1
//--uncomment below to eliminate PK or UNIQUE indexes;
//--what i call 'normal' indexes
//  --AND   INDEXPROPERTY (I.ID,I.NAME,'ISUNIQUE')       =0
//  --AND   INDEXPROPERTY (I.ID,I.NAME,'ISCLUSTERED') =0
//
//DECLARE
//  @ISQL VARCHAR(4000),
//  @TABLEID INT,
//  @INDEXID INT,
//  @MAXTABLELENGTH INT,
//  @MAXINDEXLENGTH INT
//  --USED FOR FORMATTING ONLY
//    SELECT @MAXTABLELENGTH=MAX(LEN(ObjectName)) FROM #TMP
//    SELECT @MAXINDEXLENGTH=MAX(LEN(INDEXNAME)) FROM #TMP
//
//    DECLARE C1 CURSOR FOR
//      SELECT ObjectId,INDEXID FROM #TMP  
//    OPEN C1
//      FETCH NEXT FROM C1 INTO @TABLEID,@INDEXID
//        WHILE @@FETCH_STATUS <> -1
//          BEGIN
//	SET @ISQL = ''
//	SELECT @ISQL=@ISQL + '['    + ISNULL(SYSCOLUMNS.NAME,'') + ']' + 
//	(case when indexkey_property(SYSINDEXKEYS.id, SYSINDEXKEYS.indid, SYSINDEXKEYS.keyno  ,'IsDescending') = 1 then ' DESC' else '' end) +
//',' FROM SYSINDEXES I
//	INNER JOIN SYSINDEXKEYS ON I.ID=SYSINDEXKEYS.ID AND I.INDID=SYSINDEXKEYS.INDID
//	INNER JOIN SYSCOLUMNS ON SYSINDEXKEYS.ID=SYSCOLUMNS.ID AND SYSINDEXKEYS.COLID=SYSCOLUMNS.COLID
//	WHERE I.INDID > 0 
//	AND I.INDID < 255 
//	AND (I.STATUS & 64)=0
//	AND I.ID=@TABLEID AND I.INDID=@INDEXID
//	ORDER BY SYSINDEXKEYS.KEYNO
//	UPDATE #TMP SET [ColumnsList]=@ISQL WHERE ObjectId=@TABLEID AND INDEXID=@INDEXID
//
//	FETCH NEXT FROM C1 INTO @TABLEID,@INDEXID
//         END
//      CLOSE C1
//      DEALLOCATE C1
//  --AT THIS POINT, THE '[ColumnsList]' COLUMN HAS A TRAILING COMMA
//  UPDATE #TMP SET [ColumnsList]=LEFT([ColumnsList],LEN([ColumnsList]) -1)
//  SELECT * FROM #TMP -- where ObjectName = 'table_1'
//  DROP TABLE #TMP
//set nocount off
//";

//        public static string scptAllIndexes90 =
//@"
//set nocount on
//SELECT  -- TOP 100
//  I.INDID AS IndexId,
//  I.NAME AS IndexName,
//  I.ID AS ObjectId,
//  OBJECT_NAME(I.ID) AS ObjectName,
//  REPLICATE(' ',4000) AS [ColumnsList] ,
//  INDEXPROPERTY (I.ID,I.NAME,'IsUnique') AS IsUnique,
//  case when OBJECTPROPERTY(i.id, 'IsTable') = 1 then 1 else 0 end IsTable,
//  I.Status as Status,
//  case when OBJECTPROPERTY(object_id(i.name), 'IsPrimaryKey ') = 1 then 1 else 0 end IsPrimaryKey,
//  INDEXPROPERTY (I.ID,I.NAME,'IsClustered') AS IsClustered,
//  INDEXPROPERTY (I.ID,I.NAME,'IsAutoStatistics ') AS IsAutoStatistics,
//  INDEXPROPERTY (I.ID,I.NAME,'IsDisabled') AS IsDisabled,
//  INDEXPROPERTY (I.ID,I.NAME,'IsHypothetical') AS IsHypothetical,
//  (select is_unique_constraint from sys.indexes i2 where i2.object_id = i.id and i2.index_id = i.indid) IsUniqueConstraint,
//  INDEXPROPERTY (I.ID,I.NAME,'IsFulltextKey') AS IsFulltextKey,
//  INDEXPROPERTY (I.ID,I.NAME,'IsStatistics') AS IsStatistics,
//  INDEXPROPERTY (I.ID,I.NAME,'IsMSShipped') AS IsMSShipped,
//  case when INDEXPROPERTY( i.id , i.name , 'IsAutoStatistics' ) = 1 or 
//			INDEXPROPERTY( i.id , i.name , 'IsStatistics'   ) = 1 or
//			INDEXPROPERTY( i.id , i.name , 'IsHypothetical'   ) = 1 or 
//			INDEXPROPERTY( i.id , i.name , 'IsMSShipped'   ) = 1 then 1 else 0 end as IsSystemObject,
//  IsNull(INDEXPROPERTY (I.ID,I.NAME,'[FILLFACTOR]'), 0) AS [Fillfactor],
//  filegroup_name(groupid) FileGroup
//  INTO #TMP
//  FROM SYSINDEXES I
//  WHERE I.INDID > 0 
//  AND I.INDID < 255 
//  AND (I.STATUS & 64)=0
//  AND OBJECTPROPERTY(i.id, 'IsUserTable') = 1
//--uncomment below to eliminate PK or UNIQUE indexes;
//--what i call 'normal' indexes
//  --AND   INDEXPROPERTY (I.ID,I.NAME,'ISUNIQUE')       =0
//  --AND   INDEXPROPERTY (I.ID,I.NAME,'ISCLUSTERED') =0
//
//DECLARE
//  @ISQL VARCHAR(4000),
//  @TABLEID INT,
//  @INDEXID INT,
//  @MAXTABLELENGTH INT,
//  @MAXINDEXLENGTH INT
//  --USED FOR FORMATTING ONLY
//    SELECT @MAXTABLELENGTH=MAX(LEN(ObjectName)) FROM #TMP
//    SELECT @MAXINDEXLENGTH=MAX(LEN(INDEXNAME)) FROM #TMP
//
//    DECLARE C1 CURSOR FOR
//      SELECT ObjectId,INDEXID FROM #TMP  
//    OPEN C1
//      FETCH NEXT FROM C1 INTO @TABLEID,@INDEXID
//        WHILE @@FETCH_STATUS <> -1
//          BEGIN
//	SET @ISQL = ''
//	SELECT @ISQL=@ISQL + '['    + ISNULL(SYSCOLUMNS.NAME,'') + ']' + 
//	(case when indexkey_property(SYSINDEXKEYS.id, SYSINDEXKEYS.indid, SYSINDEXKEYS.keyno  ,'IsDescending') = 1 then ' DESC' else '' end) +
//',' FROM SYSINDEXES I
//	INNER JOIN SYSINDEXKEYS ON I.ID=SYSINDEXKEYS.ID AND I.INDID=SYSINDEXKEYS.INDID
//	INNER JOIN SYSCOLUMNS ON SYSINDEXKEYS.ID=SYSCOLUMNS.ID AND SYSINDEXKEYS.COLID=SYSCOLUMNS.COLID
//	WHERE I.INDID > 0 
//	AND I.INDID < 255 
//	AND (I.STATUS & 64)=0
//	AND I.ID=@TABLEID AND I.INDID=@INDEXID
//	ORDER BY SYSINDEXKEYS.KEYNO
//	UPDATE #TMP SET [ColumnsList]=@ISQL WHERE ObjectId=@TABLEID AND INDEXID=@INDEXID
//
//	FETCH NEXT FROM C1 INTO @TABLEID,@INDEXID
//         END
//      CLOSE C1
//      DEALLOCATE C1
//  --AT THIS POINT, THE '[ColumnsList]' COLUMN HAS A TRAILING COMMA
//  UPDATE #TMP SET [ColumnsList]=LEFT([ColumnsList],LEN([ColumnsList]) -1)
//  SELECT * FROM #TMP -- where ObjectName = 'table_1'
//  DROP TABLE #TMP
//set nocount off
//";

//        public static string DropIndexTunningWizardTrash =
//@"
//DECLARE @strSQL nvarchar(1024)
//DECLARE @objid int
//DECLARE @indid tinyint
//DECLARE ITW_Stats CURSOR FOR SELECT id, indid FROM sysindexes WHERE name LIKE 'hind_%' ORDER BY name
//OPEN ITW_Stats
//FETCH NEXT FROM ITW_Stats INTO @objid, @indid
//WHILE (@@FETCH_STATUS <> -1)
//BEGIN
//SELECT @strSQL = (SELECT case when INDEXPROPERTY(i.id, i.name, 'IsStatistics') = 1 then 'drop statistics [' else 'drop index [' end + OBJECT_NAME(i.id) + '].[' + i.name + ']'
//FROM sysindexes i join sysobjects o on i.id = o.id
//WHERE i.id = @objid and i.indid = @indid AND
//(INDEXPROPERTY(i.id, i.name, 'IsHypothetical') = 1 OR
//(INDEXPROPERTY(i.id, i.name, 'IsStatistics') = 1 AND
//INDEXPROPERTY(i.id, i.name, 'IsAutoStatistics') = 0)))
//EXEC(@strSQL)
//FETCH NEXT FROM ITW_Stats INTO @objid, @indid
//END
//CLOSE ITW_Stats
//DEALLOCATE ITW_Stats
//";

//        public static string sctpAllFks =
//@"
//select constid ObjectId, object_name(constid) ObjectName, 
//fkeyid TableId, object_name(fkeyid) TableName, 
//rkeyid RefTableId, object_name(rkeyid) RefTableName
//, (select name from syscolumns c where c.id = fkeyid and c.colid = fkey) ColumnName
//, (select name from syscolumns c where c.id = rkeyid and c.colid = rkey) RefColumnName
//from sysforeignkeys f order by ObjectName, keyno
//";

//        #endregion Scripts

//    }

//}
