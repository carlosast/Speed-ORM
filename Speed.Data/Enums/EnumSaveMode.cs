using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

}
