using System.Data;

namespace Speed.Data
{

    public partial class Parameter
    {
        public string Name { get; set; }
        public object Value { get; set; }
        public DbType DbType { get; set; }
        public int? Size { get; set; }
        public ParameterDirection Direction { get; set; }
        public IDbDataParameter DbParameter { get; set; }

        public Parameter()
        {
        }

        public Parameter(string name, object value)
        {
            this.Name = name;
            this.Value = value;
        }

        public Parameter(string name, DbType type, object value)
        {
            this.Name = name;
            this.DbType = type;
            this.Value = value;
        }

    }

}
