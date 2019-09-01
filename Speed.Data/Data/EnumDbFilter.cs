namespace Speed.Data
{

    public enum EnumDbFilter
    {
        /// <summary>
        /// Operador que faz um AND para todos os campos, e usa LIKE para campos string
        /// </summary>
        AndLike,
        /// <summary>
        /// Operador que faz um OR para todos os campos, e usa LIKE para campos string
        /// </summary>
        OrLike,
        /// <summary>
        /// Operador que faz um AND para todos os campos, e usa '=' para campos string
        /// </summary>
        AndEqual,
        /// <summary>
        /// Operador que faz um OR para todos os campos, e usa LIKE para campos string
        /// </summary>
        OrEqual
    }

    public enum EnumDbSqlOperation
    {
        Equal = 0,
        Different = 1,
        GreaterThan = 2,
        GreaterThanOrEqual = 3,
        LessThan = 4,
        LessThanOrEqual = 5,
        Like = 6,
    }

}
