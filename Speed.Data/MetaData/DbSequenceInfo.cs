namespace Speed.Data.MetaData
{

    public class DbSequenceInfo
    {

        public string SequenceName { get; set; }
        public string TableName { get; set; }
        public string SchemaName { get; set; }
        public string ColumnName { get; set; }

        public string FullName
        {
            get
            {
                if (!string.IsNullOrEmpty(ColumnName))
                    return Conv.GetKey(SchemaName, TableName, ColumnName);
                else
                    return SequenceName;
            }
        }

        public DbSequenceInfo()
        {
        }

    }

}
