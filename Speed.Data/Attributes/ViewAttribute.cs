using System;

namespace Speed.Data
{

    /// <summary>
    /// Classe de atributo de um view do banco de dados
    /// </summary>
#if !DEBUG
    [System.Diagnostics.DebuggerStepThrough]
#endif
    public class DbViewAttribute : Attribute
    {

        public string ViewName { get; set; }

        public DbViewAttribute()
        {
            // TODO: tratar quando não passar nada
        }

        public DbViewAttribute(string viewName)
        {
            this.ViewName = viewName;
        }

    }
}
