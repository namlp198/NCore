using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Npc.Foundation.Util
{
    public static class JsonUtil
    {
        /// <summary>
        /// Model To Json String
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ModelToJsonString(object obj, bool formatting = false)
        {
            if (obj == null) { return null; }

            return JsonConvert.SerializeObject(obj, (formatting) ? Formatting.Indented : Formatting.None);
        }

        /// <summary>
        /// Model To Json File
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="filePath"></param>
        public static void ModelToJsonFile(object obj, string filePath, bool formatting = false)
        {
            using (StreamWriter file = File.CreateText(filePath))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Formatting = (formatting) ? Formatting.Indented : Formatting.None;
                serializer.Serialize(file, obj);
            }
        }

        /// <summary>
        /// Json String To Model
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public static T JsonStringToModel<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        /// <summary>
        /// Json File To Model
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static T JsonFileToModel<T>(string filePath)
        {
            T obj = default(T);

            using (StreamReader file = File.OpenText(filePath))
            {
                JsonSerializer serializer = new JsonSerializer();
                obj = (T)serializer.Deserialize(file, typeof(T));
            }

            return obj;
        }

        /// <summary>
        /// Json File To Dictionary List
        /// [NCS-3182] : 언어 파일 export, import 시, 언어별로 Export 필요
        /// </summary>
        /// <param name="keyPropertyName"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static List<Dictionary<string, object>> JsonFileToDictionaryList(string keyPropertyName, string filePath)
        {
            List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();

            using (StreamReader stringReader = File.OpenText(filePath))
            {
                using (var jsonReader = new JsonTextReader(stringReader))
                {
                    Dictionary<string, object> obj = new Dictionary<string, object>();
                    while (jsonReader.Read())
                    {
                        if (jsonReader.TokenType == JsonToken.PropertyName)
                        {
                            string pName = (string)jsonReader.Value;
                            if (pName == keyPropertyName)
                            {
                                obj = new Dictionary<string, object>();
                                list.Add(obj);
                            }

                            obj.Add(pName, "");

                            jsonReader.Read();
                            obj[pName] = jsonReader.Value;
                        }
                    }
                }
            }

            return list;
        }

        /// <summary>
        /// Json Serialize를 사용한 오브젝트 복제(deep Copy)
        /// </summary>
        /// <typeparam name="T">변환 타입</typeparam>
        /// <param name="source">오브젝트</param>
        /// <returns>복제 오브젝트</returns>
        public static T CloneJson<T>(T source) where T : class
        {
            // Don't serialize a null object, simply return the default for that object
            if (ReferenceEquals(source, null)) return null;

            try
            {
                var serializaString = JsonConvert.SerializeObject(source
                , new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All,
                    Converters = new List<JsonConverter> { new PathGeometryConverter() }
                });

                return JsonConvert.DeserializeObject<T>(serializaString
                    , new JsonSerializerSettings
                    {
                        ObjectCreationHandling = ObjectCreationHandling.Replace,
                        TypeNameHandling = TypeNameHandling.All,
                        Converters = new List<JsonConverter> { new PathGeometryConverter() }
                    });
            }
            catch (JsonSerializationException e)
            {
                Console.WriteLine($"JsonSerializationException: {e}");
                return null;
            }
        }

        /// <summary>
        /// PathGeometry 변환기.
        /// 적용하지 않는 경우, StreamGeometry -> PathGeometry 캐스팅 시도로, Exceiption 발생
        /// </summary>
        private class PathGeometryConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(PathGeometry);
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                if (value is PathGeometry)
                {
                    var pathGeometry = (PathGeometry)value;
                    writer.WriteValue(pathGeometry != PathGeometry.Empty ? pathGeometry.ToString() : string.Empty);
                }
                else
                {
                    throw new Exception("Expected ObjectId value.");
                }
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                if (reader.TokenType != JsonToken.String)
                    throw new Exception($"Unexpected token parsing PathGeometry. Expected String, got {reader.TokenType}.");

                return PathGeometry.CreateFromGeometry(PathGeometry.Parse((string)reader.Value));
            }
        }
    }
}
