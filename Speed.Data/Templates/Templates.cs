﻿#if !DEBUG
//[System.Diagnostics.DebuggerStepThrough]
#endif
public static class Templates
{

    internal static string GEN_WARNING = @"// ****** SPEED ******
//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
";

    #region RECORD_TEMPLATE

    public const string RECORD_TEMPLATE =
@"using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Speed;
using Speed.Data;
[UsingAdd]

namespace [NameSpace]
{

    [DbTable([SchemaName], '[TableName]', '[SequenceColumn]', '[SequenceName]')]
    [Serializable]
    [DataContract(Name = '[ClassName]', Namespace = '')]
//    [System.Diagnostics.DebuggerStepThrough]
    [DataAnnotation]
    public partial class [ClassName] : Record, ICloneable, INotifyPropertyChanged
    {

[Columns]        
        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null))
            {
                propertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        #region IsEqual

        public bool IsEqual([ClassName] value)
        {
            if (value == null)
                return false;
            return
[ColumnsIsEqual]
        }

        #endregion IsEqual

        #region Clone

        public override object Clone()
        {
            return CloneT();
        }

        public [ClassName] CloneT()
        {
            [ClassName] value = new [ClassName]();
            [!POCO]value.RecordStatus = this.RecordStatus;
            [!POCO]value.RecordTag = this.RecordTag;

[ColumnsClone]
            return value;
        }

        #endregion Clone

        #region Create

        public static [ClassName] Create([CreateParams])
        {
            [ClassName] __value = new [ClassName]();

[CreateColumns]
            return __value;
        }

        #endregion Create

   }

}
";
    #endregion RECORD_TEMPLATE

    #region RECORD_EXT_TEMPLATE

    public const string RECORD_EXT_TEMPLATE =
@"using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Speed;
using Speed.Data;

namespace [NameSpace]
{

    public partial class [ClassName]
    {

    }

}
";
    #endregion RECORD_EXT_TEMPLATE

    #region RECORD_COLUMN_TEMPLATE

    /// <summary>
    /// 
    /// </summary>
    public const string RECORD_COLUMN_TEMPLATE_RAISEPROPERTYCHANGED =
@"        /// <summary>
        /// Column [ColumnName]
        /// </summary>
        private [DataTypeNullable] z_[PopertyName];
        [DbColumn('[ColumnName]')]
        [DataMember]
        [DataAnnotation]
        public [DataTypeNullable] [PopertyName]
        {
            get { return z_[PopertyName]; }
            set
            {
                if (z_[PopertyName] != value)
                {
                    z_[PopertyName] = value;
                    this.RaisePropertyChanged('[PopertyName]');
                }
            }
        }

";

    public const string RECORD_COLUMN_TEMPLATE_POCO =
@"        /// <summary>
        /// Column [ColumnName]
        /// </summary>
        [DbColumn('[ColumnName]')]
        [DataAnnotation]
        public [DataTypeNullable] [PopertyName] { get; set; }

";

    public const string RECORD_COLUMN_TEMPLATE_IDENTITY =
@"
        private [DataTypeNullable] z_[ColumnName];

        /// <summary>
        /// Column [ColumnName]
        /// </summary>
        [DbColumn('[ColumnName]')]
        public [DataTypeNullable] [ColumnName]
        {
            get { return z_[ColumnName]; }
        }";

    #endregion RECORD_COLUMN_TEMPLATE

    #region RECORD_FILTER_TEMPLATE

    public const string RECORD_FILTER_TEMPLATE =
@"using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ComponentModel;
using Speed;
using Speed.Data;
[UsingAdd]

namespace [NameSpace]
{

    [Serializable]
    [DataContract(Name = '[FilterClassName]', Namespace = '')]
    public partial class [FilterClassName]
    {

[Columns]
    }

}
";
    #endregion RECORD_FILTER_TEMPLATE

    #region RECORD_FILTER_COLUMN_TEMPLATE

    public const string RECORD_FILTER_COLUMN_TEMPLATE =
@"        public [DataTypeNullable] [PopertyName] { get; set; }
        public EnumDbSqlOperation [PopertyName]Operation { get; set; }

";

    #endregion RECORD_FILTER_COLUMN_TEMPLATE


    #region BL_TEMPLATE

    public const string BL_TEMPLATE =
@"using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Speed.Data;
using [NameSpaceEntity];
[UsingAdd]
namespace [NameSpaceBL]
{

    //[System.Diagnostics.DebuggerStepThrough]
    public partial class [BLName] : BLClass<[NameSpaceEntity].[Entity]>
    {

[SelectByPk]
[DeleteByPk]
[ParentRelations]
[ChildRelations]
    }

}
";

    public const string BL_TEMPLATE_SELECT_BY_FILTER =
@"        public static [NameSpaceEntity].[Entity] SelectSingle(Database db, [NameSpaceEntity].[Entity])
        {
            return db.SelectSingle<[NameSpaceEntity].[Entity]>(string.Format('[PkColumnsWhere]', [PkColumnsValue]));
        }
        public static List<[NameSpaceEntity].[Entity]> Select(Database db, [NameSpaceEntity].[Entity])
        {
            return db.Select<[NameSpaceEntity].[Entity]>(string.Format('[PkColumnsWhere]', [PkColumnsValue]));
        }
";

public const string BL_TEMPLATE_SELECT_BY_PK =
@"        public static [NameSpaceEntity].[Entity] SelectByPk(Database db, [PkDeclarationNullable])
        {
            return db.SelectSingle<[NameSpaceEntity].[Entity]>(string.Format('[PkColumnsWhere]', [PkColumnsValue]));
        }
        public static [NameSpaceEntity].[Entity] SelectByPk([PkDeclarationNullable])
        {
            using (var db = Sys.NewDb())
                return db.SelectSingle<[NameSpaceEntity].[Entity]>(string.Format('[PkColumnsWhere]', [PkColumnsValue]));
        }
";

    public const string BL_TEMPLATE_DELETE_BY_PK =
@"        public static int DeleteByPk(Database db, [PkDeclarationNullable])
        {
            return db.Delete<[NameSpaceEntity].[Entity]>(string.Format('[PkColumnsWhere]', [PkColumnsValue]));
        }
        public static int DeleteByPk([PkDeclarationNullable])
        {
            using (var db = Sys.NewDb())
                return db.Delete<[NameSpaceEntity].[Entity]>(string.Format('[PkColumnsWhere]', [PkColumnsValue]));
        }
";

    public const string BL_TEMPLATE_SELECT_BY_PARENT_RELATIONS =
@"//        public static [NameSpaceEntity].[ForeignEntity] Select_Parent_[MethodName](Database db, [FkDeclarationNullable])
//        {
//            return db.SelectSingle<[NameSpaceEntity].[ForeignEntity]>(string.Format('[FkColumnsWhere]', [FkColumnsValue]));
//        }
//          public static [NameSpaceEntity].[ForeignEntity] Select_Parent_[MethodName]([FkDeclarationNullable])
//        {
//            using (var db = Sys.NewDb())
//                return db.SelectSingle<[NameSpaceEntity].[ForeignEntity]>(string.Format('[FkColumnsWhere]', [FkColumnsValue]));
//        }
//        public static [NameSpaceEntity].[ForeignEntity] Select_Parent_[MethodName](Database db, [NameSpaceEntity].[ClassName] rec)
//        {
//            return Select_Parent_[MethodName](db, [FkColumnsValueEntity]);
//        }
//        public static [NameSpaceEntity].[ForeignEntity] Select_Parent_[MethodName]([NameSpaceEntity].[ClassName] rec)
//        {
//            using (var db = Sys.NewDb())
//                return Select_Parent_[MethodName](db, [FkColumnsValueEntity]);
//        }
";

    public const string BL_TEMPLATE_SELECT_BY_CHILD_RELATIONS =
@"//        public static List<[NameSpaceEntity].[ForeignEntity]> Select_Children_[MethodName](Database db, [FkDeclarationNullable])
//        {
//            return db.Select<[NameSpaceEntity].[ForeignEntity]>(string.Format('[FkColumnsWhere]', [FkColumnsValue]));
//        }
//        public static List<[NameSpaceEntity].[ForeignEntity]> Select_Children_[MethodName]([FkDeclarationNullable])
//        {
//            using (var db = Sys.NewDb())
//                return db.Select<[NameSpaceEntity].[ForeignEntity]>(string.Format('[FkColumnsWhere]', [FkColumnsValue]));
//        }
//        public static List<[NameSpaceEntity].[ForeignEntity]> Select_Children_[MethodName](Database db, [NameSpaceEntity].[ClassName] rec)
//        {
//            return Select_Children_[MethodName](db, [FkColumnsValueEntity]);
//        }
//        public static List<[NameSpaceEntity].[ForeignEntity]> Select_Children_[MethodName]([NameSpaceEntity].[ClassName] rec)
//        {
//            using (var db = Sys.NewDb())
//                return Select_Children_[MethodName](db, [FkColumnsValueEntity]);
//        }
";

    #endregion BL_TEMPLATE

    #region BL_EXT_TEMPLATE

    public const string BL_EXT_TEMPLATE =
@"using System;
using System.Collections.Generic;
using System.Data;
using Speed.Data;
using [NameSpaceEntity];

namespace [NameSpaceBL]
{

    public partial class [BLName]
    {

    }

}
";

    #endregion BL_EXT_TEMPLATE

    #region DATA_ENUM_TEMPLATE

    public const string ENUM_TEMPLATE =
@"    public enum [EnumName] : [Type]
	{
[EnumColumns]    }
";

    public const string ENUM_CLASS_TEMPLATE =
@"using System;

namespace [NameSpace]
{

[EnumCode]
}
";
    #endregion DATA_ENUM_TEMPLATE

    /*
     * DeleteAll
        StringBuilder b = new StringBuilder();
        string sql = @'delete from TableTeste where Id = @OldId';
        using (DbCommand cmd = db.NewCommand(sql))
        {
            for (int i = 0; i < instances.Length; i++ )
            {
                DAL.TableTeste value = (DAL.TableTeste)instances[i];
                DAL.TableTeste old = value.RecordOriginal != null ? (DAL.TableTeste)value.RecordOriginal : value;
                string parName = '@Old' + i;
                b.AppendLine(sql.Replace('@Old', parName));
                cmd.Parameters.AddWithValue(parName, old.Id);
            }
            cmd.CommandText = b.ToString();
            return db.ExecuteNonQuery(cmd);
        }
*/

    #region PROCEDURE_TEMPLATE

    public const string PROCEDURE_TEMPLATE_USING =
@"using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Speed;
using Speed.Data;

namespace [NameSpace]
{
";

    public const string PROCEDURE_TEMPLATE_RECORD =
@"
    [DbProcedure('[ProcedureName]')]
    [Serializable]
    public partial class [TableName] : Record
    {

[Columns]

    }
";

    public const string PROCEDURE_TEMPLATE_COMMAND_LIST =
@"
        List<[ProcedureNameRecord]> [MethodName](Database db, [ProcedureParameters], int commandTimeout = db.CommandTimeout)
        {
            List<[ReturnTypeName]> list = new List<ReturnTypeName>();
            using (DbCommand cmd = db.NewCommand('[ProcedureName]', commandTimeout, CommandType.StoredProcedure))
            {
[SetParameters]
                while (reader.Read())
                {
                    [ReturnTypeName] rec = new [ReturnTypeName];
[SetReadParameters]
                return list.Add(rec);
            }
            return list;
        }
";

    // AddWithValue(DbCommand cmd, string parameterName, DbType dbType, ParameterDirection direction, object value, int size = 0)
    public const string PROCEDURE_TEMPLATE_PARAMETER =
        "\t\t\t\tAddParameter(cmd, '[ParameterName]', [DbType], [ParameterDirection], rec.[PropertyName]);\r\n";
    
    public const string PROCEDURE_TEMPLATE_PARAMETER_SIZE =
        "\t\t\t\tAddParameter(cmd, '[ParameterName]', [DbType], [ParameterDirection], rec.[PropertyName], [Size]);\r\n";

    #endregion PROCEDURE_TEMPLATE

 #region SELECT_FILTER_TEMPLATE

    public const string SELECT_FILTER_TEMPLATE =

@"            if (filter.[PropertyName] != null)
            {
                if (++index > 1)
                    where.Append(op);
                db.AddWithValue(cmd, '[ParameterName]', '[DataType]', filter.[PropertyName]);
                where.AppendFormat('[ColumnName] = {0} ', '[ParameterName]');
            }
";

    public const string SELECT_FILTER_TEMPLATE_STRING =

@"            if (!string.IsNullOrEmpty(filter.[PropertyName]))
            {
                if (++index > 1)
                    where.Append(op);
        
                if (mode == EnumDbFilter.AndLike || mode == EnumDbFilter.OrLike)
                {
                    db.AddWithValue(cmd, '[ParameterName]', '[DataType]', '%'+  filter.[PropertyName] + '%');
                    where.AppendFormat('[ColumnName] LIKE {0} ', '[ParameterName]');
                }
                else
                {
                    db.AddWithValue(cmd, '[ParameterName]', '[DataType]', filter.[PropertyName]);
                    where.AppendFormat('[ColumnName] = {0} ', '[ParameterName]');
                }
            }
";

 #endregion SELECT_FILTER_TEMPLATE

}