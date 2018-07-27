using System;
using System.Collections.Generic;
using System.Linq;

namespace Speed
{

#if !DEBUG
    [System.Diagnostics.DebuggerStepThrough]
#endif
    public static class CollectionUtil
    {

        /// <summary>
        /// Checa se uma Key existe no Dictionary. Se existir, retorna. Senão retorna default(TSource)
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static TSource GetValue<TKey, TSource>(this Dictionary<TKey, TSource> source, TKey key)
        {
            TSource ret;
            if (source.TryGetValue(key, out ret))
                return ret;
            else
                return default(TSource);
        }

        public static Dictionary<TKey, List<TSource>> GroupByToDictionary<TKey, TSource>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer = null)
        {
            var dic = comparer != null ? new Dictionary<TKey, List<TSource>>(comparer) : new Dictionary<TKey, List<TSource>>();
            foreach (var pair in source.GroupBy(keySelector))
                dic.Add(pair.Key, pair.ToList());
            return dic;
        }

        public static Dictionary<TKey, TSource> GroupByToDictionaryDistinct<TKey, TSource>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer = null)
        {
            var dic = comparer != null ? new Dictionary<TKey, TSource>(comparer) : new Dictionary<TKey, TSource>();
            foreach (var pair in source.GroupBy(keySelector))
                dic.Add(pair.Key, pair.First());
            return dic;
        }

        /// <summary>
        /// Divide um IEnumerable em partes
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="blockSize">Número de partes á ser dividida</param>
        /// <returns></returns>
        public static IEnumerable<IEnumerable<T>> Split<T>(this IEnumerable<T> list, int blockSize)
        {
            List<List<T>> ret = new List<List<T>>();
            var list2 = list.ToList();

            if (blockSize == 0)
                blockSize = 1;

            while (list2.Count > 0)
            {
                List<T> block = list2.Take(Math.Min(list2.Count(), blockSize)).ToList();
                list2.RemoveRange(0, block.Count);
                ret.Add(block);
            }
            return ret;

            //int parts = list.Count() / blockSize;
            //if (parts == 0)
            //    parts = 1;

            //return list.Select((item, index) => new { index, item })
            //           .GroupBy(x => x.index % parts)
            //           .Select(x => x.Select(y => y.item));
        }

        /// <summary>
        /// Converte para um List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static List<T> ToList<T>(this System.Collections.IEnumerable collection)
        {
            List<T> list = new List<T>();
            foreach (T item in collection)
                list.Add(item);
            return list;
        }

        /// <summary>
        /// Ordena um IEnumerable pelo nome de uma Propriedade
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="propertyName"></param>
        /// <param name="descending"></param>
        /// <returns></returns>
        public static IEnumerable<T> Sort<T>(IEnumerable<T> list, string propertyName, bool descending = false)
        {
            if (list == null)
                return list;
            if (list.Count() == 0)
                return list;

            var prop = list.First().GetType().GetProperty(propertyName);
            if (prop == null)
                throw new Exception("Property not found");

            List<KeyValuePair<object, T>> values = new List<KeyValuePair<object, T>>();

            foreach (T value in list)
                values.Add(new KeyValuePair<object, T>(prop.GetValue(value, null), value));

            List<T> result = new List<T>();
            if (!descending)
                result = (from c in values orderby c.Key select c.Value).ToList();
            else
                result = (from c in values orderby c.Key descending select c.Value).ToList();

            return result;
        }

        /// <summary>
        /// Faz um ToDictionary, mas só para valores distintos. Se tiver valores com chaves duplicadas serão desprezadas
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="keySelector"></param>
        /// <returns></returns>
        public static Dictionary<TKey, TSource> ToDictionaryDistinct<TKey, TSource>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            Dictionary<TKey, TSource> dict = new Dictionary<TKey, TSource>();

            foreach (var pair in source.GroupBy(keySelector))
            {
                dict.Add(pair.Key, pair.First());
            }
            return dict;
        }

        /// <summary>
        /// Faz um ToDictionary, mas só para valores distintos. Se tiver valores com chaves duplicadas serão desprezadas
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="keySelector"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public static Dictionary<TKey, TSource> ToDictionaryDistinct<TKey, TSource>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
        {
            Dictionary<TKey, TSource> dict = new Dictionary<TKey, TSource>(comparer);

            foreach (var pair in source.GroupBy(keySelector))
            {
                dict.Add(pair.Key, pair.First());
            }
            return dict;
        }

        public static IEnumerable<TSource> Distinct<TKey, TSource>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            List<TSource> list = new List<TSource>();

            foreach (var pair in source.GroupBy(keySelector))
            {
                list.Add(pair.First());
            }
            return list;
        }

        // http://stackoverflow.com/questions/5215469/use-linq-to-break-up-listt-into-lots-of-listt-of-n-length/5221464#5221464
        public static List<List<T>> Chunk<T>(this List<T> theList, int chunkSize)
        {
            if (!theList.Any())
            {
                return new List<List<T>>();
            }

            List<List<T>> result = new List<List<T>>();
            List<T> currentList = new List<T>();
            result.Add(currentList);

            int i = 0;
            foreach (T item in theList)
            {
                if (i >= chunkSize)
                {
                    i = 0;
                    currentList = new List<T>();
                    result.Add(currentList);
                }
                i += 1;
                currentList.Add(item);
            }
            return result;
        }

        /// <summary>
        /// Embaralha a ordem dos elementos
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        public static void Shuffle<T>(this IList<T> list)
        {
            Random rng = new Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

    }

}
