namespace Speed.Data
{

    /// <summary>
    /// Mode de salvar um registro
    /// </summary>
    public enum EnumSaveMode
    {
        /// <summary>
        /// Salva o registro e não atualiza as propriedades da classe. Este é o modo mais rápido
        /// </summary>
        None,
        // Salva o registro e recarrrega os valores da base de dados
        Requery
    }

    public enum EnumDbProviderType
    {
        SqlServer = 1,
        MySql = 2,
        Firebird = 3,
        Oracle = 4,
        SqlServerCe = 5,
        OleDb = 6,
        PostgreSQL = 7,
        Access = 8,
        SQLite = 9,
        MariaDB = 10
    }

    public enum EnumConstraintType
    {
        ForeignKey,
        PrimaryKey,
        Unique,
        Other
    }

    public enum EnumRoutineType
    {
        Undefined,
        Procedure,
        Function
    }

    public enum EnumFileChanged
    {
        Unmodified = 0,
        Modified = 1,
        New = 2
    }

    //public enum EnumParameterMode
    //{
    //    In, Out, InOut
    //}

}
