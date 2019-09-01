using System;

namespace Speed.Data
{

    /// <summary>
    /// Attributes of tables and views
    /// </summary>
#if !DEBUG
    [System.Diagnostics.DebuggerStepThrough]
#endif
    public class DbTableAttribute : Attribute
    {

        public string TableName { get; set; }
        public string SchemaName { get; set; }
        public string SequenceColumn{ get; set; }
        public string SequenceName { get; set; }

        public DbTableAttribute()
        {
        }

        public DbTableAttribute(string tableName)
        {
            this.TableName = tableName;
        }

        public DbTableAttribute(string schemaName, string tableName)
        {
            this.SchemaName = schemaName;
            this.TableName = tableName;
        }

        public DbTableAttribute(string schemaName, string tableName, string sequenceColumn, string sequenceName)
        {
            this.SchemaName = schemaName;
            this.TableName = tableName;
            this.SequenceName = sequenceName;
            this.SequenceColumn = sequenceColumn;
        }

    }
}
