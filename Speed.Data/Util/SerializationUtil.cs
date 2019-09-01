using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Xml;
using Speed.Data;
using System.Reflection;
using Speed.Common;

// Carlos Alberto Stefani

namespace Speed
{

    public static class SerializationUtil
    {

        public static string Serialize<T>(object value, bool indent = true)
        {
            DataContractSerializer dcs = new DataContractSerializer(typeof(T), GetTypes<T>());
            using (StringWriter sw = new StringWriter())
            {
                //using (XmlWriter xw = XmlWriter.Create(sw, new XmlWriterSettings { Indent = indent }))
                using (XmlWriter xw = XmlWriter.Create(sw, new XmlWriterSettings
                {
                    CheckCharacters = true,
                    CloseOutput = false,
                    ConformanceLevel = ConformanceLevel.Auto,
                    //Encoding = System.Text.UTF8Encoding.UTF8,
                    Indent = indent,
                    IndentChars = "\t",
                    NamespaceHandling = NamespaceHandling.OmitDuplicates,
                    NewLineChars = "\r\n",
                    NewLineHandling = NewLineHandling.Entitize,
                    NewLineOnAttributes = false,
                    OmitXmlDeclaration = false
                }))

                    dcs.WriteObject(xw, value);
                return sw.ToString();
            }
        }

        public static T Deserialize<T>(string xml)
        {
            DataContractSerializer dcs = new DataContractSerializer(typeof(T), GetTypes<T>());
            using (StringReader sr = new StringReader(xml))
                return (T)dcs.ReadObject(XmlReader.Create(sr));
        }

        public static string SerializeToJson<T>(object value)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                DataContractJsonSerializer serializer =
                        new DataContractJsonSerializer(typeof(T), GetTypes<T>());
                serializer.WriteObject(ms, value);
                ms.Position = 0;

                using (StreamReader reader = new StreamReader(ms))
                {
                    var ret = reader.ReadToEnd();
                    return ret;
                }
            }
        }

        public static T DeserializeFromJson<T>(string value)
        {
            using (MemoryStream ms = new MemoryStream(System.Text.Encoding.Unicode.GetBytes(value)))
            {
                DataContractJsonSerializer serializer =
                        new DataContractJsonSerializer(typeof(T), GetTypes<T>());

                return (T)serializer.ReadObject(ms);
            }
        }

        public static Type[] GetTypes<T>()
        {
            return new Type[] { typeof(T), typeof(List<T>), typeof(Record), typeof(List<Record>) };
        }

        public static void SaveToFile<T>(T obj, string fileName)
        {
            File.WriteAllText(fileName, Serialize<T>(obj));
        }

        public static T LoadFromFile<T>(string fileName)
        {
            return Deserialize<T>(File.ReadAllText(fileName));
        }

        /*
        /// <summary>
        /// Converte um objeto na sua representação Xml
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="text"></param>
        /// <returns></returns>
        public static T StringXmlToObject<T>(string text)
        {
            T obj;
            using (var stream = new MemoryStream())
            {
                var serializer = new DataContractSerializer(typeof(T));

                obj = (T)serializer.ReadObject(stream);
                return obj;
            }
        }
        */

        /// Converte a representação Xml de um objeto para uma instância do objeto
        public static string ObjectToStringXml(object obj)
        {
            String xml;
            using (var stream = new MemoryStream())
            {
                var serializer = new DataContractSerializer(obj.GetType());
                serializer.WriteObject(stream, obj);
                xml = Encoding.UTF8.GetString(stream.ToArray());
                return xml;
            }
        }

        /// <summary>
        /// Salva um objeto ao disco, no formato Xml
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="obj"></param>
        public static void WriteToXml<T>(string fileName, T obj)
        {
            File.WriteAllText(fileName, Serialize<T>(obj));
            //File.WriteAllText(fileName, ObjectToStringXml(obj));
        }

        /// <summary>
        /// Lê do disco um arquivo no formato Xml e converte numa instância do objeto
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static T ReadFromXml<T>(string fileName)
        {
            return Deserialize<T>(File.ReadAllText(fileName));
        }

        /*
        public static void WriteToCsv<T>(string fileName, IEnumerable<T> list)
        {
            using (var fs = new System.IO.FileStream(fileName, FileMode.Create))
            {
                using (var w = new StreamWriter(fs))
                {
                    var props = typeof(T).GetRuntimeProperties().ToDictionary(p => p.Name);


                    int index = 0;
                    foreach (var prop in props)
                    {
                        if (index < props.Count - 1)
                            w.Write(string.Format("\"{0}\";", getCsvValue(prop.Value.Name)));
                        else
                            w.Write(string.Format("\"{0}\"", getCsvValue(prop.Value.Name)));
                        index++;
                    }

                    foreach (var item in list)
                    {
                        index = 0;
                        foreach (var prop in props)
                        {
                            if (index < props.Count - 1)
                                w.Write(string.Format("\"{0}\";", getCsvValue(prop.Value.GetValue(item, null))));
                            else
                                w.Write(string.Format("\"{0}\"", getCsvValue(prop.Value.GetValue(item, null))));
                            index++;
                        }
                        w.WriteLine();
                    }
                }
            }
        }
        */
        static string getCsvValue(object value)
        {
            if (value == null)
                return null;
            var val = Conv.ToString(value);
            if (val.IndexOf(';') > -1)
                val = val.Replace(';', ',');
            return val;

        }

    }

}
