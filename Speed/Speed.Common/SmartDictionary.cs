using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Speed
{

    /// <summary>
    /// Classe de ajuda, principalmente usada dentro de loops, com lógica que evitar que uma mesma busca seja realizada na base de dados.
    /// Esta classe herda do Dictionary e implementa apenas um método, o Find. Todo restante funciona como um Dictionary.
    /// Esta classe não acessa a base de dados, mas serve como um cache local
    /// Possui internamente uma lógica que, usando o método Find, caso um elemento não exista, executa a função funcValue. Se existir, retornar o valor já  existente.
    /// </summary>
    /// <typeparam name="TKey">Tipo da chave do dictionary</typeparam>
    /// <typeparam name="TValue">Tipo dos valores do dictionary</typeparam>
#if !DEBUG
    [System.Diagnostics.DebuggerStepThrough]
#endif
    public class SmartDictionary<TKey, TValue> : Dictionary<TKey, TValue>
    {

        /// <summary>
        /// Busca um elemento no dictioary. Caso exista, retorna o valor encontrado.
        /// Caso não exista, executa funcFindValue e adicona um elemento com chave "key" e o valor retornado por "funcFindValue"
        /// </summary>
        /// <param name="key">Chave do elemento</param>
        /// <param name="funcFindValue">Function que será executada, caso ainda não exista um elemento com a chave "key"</param>
        /// <returns></returns>
        public TValue Find(TKey key, Func<TValue> funcFindValue)
        {
            TValue value;
            if (!TryGetValue(key, out value))
            {
                value = funcFindValue();
                this.Add(key, value);
            }
            return value;
        }


    }

    /// <summary>
    /// Classe de ajuda, principalmente usada dentro de loops, com lógica que evitar que uma mesma busca seja realizada na base de dados.
    /// Esta classe herda do Dictionary e implementa apenas um método, o Find. Todo restante funciona como um Dictionary.
    /// Esta classe não acessa a base de dados, mas serve como um cache local
    /// Possui internamente uma lógica que, usando o método Find, caso um elemento não exista, executa a função funcValue. Se existir, retornar o valor já  existente.
    /// Difere de SmartDictionary, pq os elementos são um List de TValue
    /// </summary>
    /// <typeparam name="TKey">Tipo da chave do dictionary</typeparam>
    /// <typeparam name="TValue">Tipo dos valores do dictionary</typeparam>
    public class SmartDictionaryList<TKey, TValue> : Dictionary<TKey, List<TValue>>
    {

        /// <summary>
        /// Busca um elemento no dictionary. Caso exista, retorna o valor encontrado.
        /// Caso não exista, executa funcFindValue e adicona um elemento com chave "key" e o valor retornado por "funcFindValue"
        /// </summary>
        /// <param name="key">Chave do elemento</param>
        /// <param name="funcFindValue">Function que será executada, caso ainda não exista um elemento com a chave "key"</param>
        /// <returns></returns>
        public List<TValue> Find(TKey key, Func<IEnumerable<TValue>> funcFindValue)
        {
            List<TValue> values;
            if (!TryGetValue(key, out values))
            {
                values = funcFindValue().ToList();
                this.Add(key, values);
            }
            return values;
        }

    }

    /// <summary>
    /// Classe de ajuda, para definir uma classe com valores, mas que serão populados apenas quando a propriedade value for chamada.
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    public class SmartValue<TValue>
    {

        TValue value;
        Func<TValue> funcValue;

        /// <summary>
        /// Cria uma nova instância de SmartValue;
        /// </summary>
        /// <param name="funcValue">Função que será executada quando a propriedade Value for chamada pela priemira vez</param>
        public SmartValue(Func<TValue> funcValue)
        {
            this.funcValue = funcValue;
        }

        /// <summary>
        /// Valor
        /// </summary>
        public TValue Value
        {
            get
            {
                if (value == null)
                    value = funcValue();
                return value;
            }

        }

    }


}
