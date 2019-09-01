using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using Speed.Data;

namespace Speed.Windows
{

    /// <summary>
    /// Simples Cache de dados
    /// Evita se sejam refeitas chamadas desnecessárias ao banco de dados
    /// </summary>
    public class DataCache
    {

        private static Dictionary<string, DataCacheItemBase> items;
        static Timer timer = new Timer();

        static DataCache()
        {
            items = new Dictionary<string, DataCacheItemBase>();
            timer = new Timer();
            timer.Interval = 5000;
            timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
            timer.Start();
        }

        static void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Clear();
        }

        public static void Clear()
        {
            lock (items)
            {
                foreach (var pair in items.ToList())
                {
                    var item = pair.Value;
                    if (item.IsExpired)
                    {
                        items.Remove(pair.Key);
                        item.Dispose();
                    }
                }
            }
        }

        /// <summary>
        /// Adicona um objeto ao cache
        /// </summary>
        /// <param name="value"></param>
        /// <param name="secondsInCache">Tempo em segundos. O default é 5 segundos</param>
        public static void Add(string key, object value, double secondsInCache = 5)
        {
            items.Add(key, new DataCacheItem<object>(value, secondsInCache));
        }

        public static void Add<T>(string key, double secondsInCache = 5, Func<Database, T> funcLoad = null)
        {
            items.Add(key, new DataCacheItem<T>(null, secondsInCache, funcLoad));
        }

        /// <summary>
        /// Retorna item do cache com chave key. se não existir retorna null
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static object GetValue(string key)
        {
            lock (items)
            {
                if (items.ContainsKey(key))
                    return items[key];
            }
            return null;
        }

        /// <summary>
        /// Retorna item do cache com chave key. Se não existir, se for passada a function funcLoad adiciona-o, senão retorna null
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="secondsInCache"></param>
        /// <param name="funcLoad"></param>
        /// <returns></returns>
        public static T GetValue<T>(string key, double secondsInCache = 5, Func<Database, T> funcLoad = null)
        {
            lock (items)
            {
                if (items.ContainsKey(key))
                {
                    var item = (DataCacheItem<T>)items[key];
                    return (T)item.Value;
                }
                else
                {
                    Add<T>(key, secondsInCache, funcLoad);
                    var item = (DataCacheItem<T>)items[key];
                    return (T)item.Value;
                }
            }
        }

        #region DataCacheItemBase

        internal class DataCacheItemBase : IDisposable
        {

            protected object value;
            DateTime dateCreated;
            DateTime? DateFinish;

            public DataCacheItemBase(object value, double secondsInCache)
            {
                dateCreated = DateTime.Now;
                if (secondsInCache != 0)
                    DateFinish = DateTime.Now.AddSeconds(secondsInCache);
                this.value = value;
            }

            public void Dispose()
            {
                if (value is IDisposable)
                    ((IDisposable)value).Dispose();
                value = null;
            }

            public virtual object Value
            {
                get
                {
                    return this.value;
                }
                set { this.value = value; }
            }

            /// <summary>
            /// Checa se o cache expirou
            /// </summary>
            public bool IsExpired
            {
                get
                {
                    return DateFinish != null && DateFinish.HasValue && DateFinish.Value > DateTime.Now;
                }
            }

        }

        #endregion DataCacheItemBase

        #region DataCacheItem

        internal class DataCacheItem<T> : DataCacheItemBase
        {

            Func<Database, T> funcLoad;

            public DataCacheItem(object value, double secondsInCache, Func<Database, T> funcLoad = null)
                : base(value, secondsInCache)
            {
                this.funcLoad = funcLoad;
            }

            public new object Value
            {
                get
                {
                    if (value == null && funcLoad != null)
                        value = ProgramBase.RunInDb<T>(funcLoad);
                    return this.value;
                }
                set { this.value = value; }
            }

        }

        #endregion DataCacheItem

    }

}
