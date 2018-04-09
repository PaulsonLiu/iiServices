using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace iiFramework.Util
{
    
    public static class XmlSerializeHelper
    {
        #region XmlSerializer
        //声明为静态只读的对象，可以在反序列化的时候节省很多时间
        //private static readonly XmlSerializer TestConfigureSerializer = new XmlSerializer(typeof(TestConfigureData));
        //private static readonly XmlSerializer SystemConfigureSerializer = new XmlSerializer(typeof(SystemConfigureData));

        //private static readonly List<XmlSerializer> SerializerList = new List<XmlSerializer>();

        private static readonly Dictionary<Type, XmlSerializer> publicTypeSerializerList = new Dictionary<Type, XmlSerializer>();
        private static readonly Dictionary<Type, DataContractSerializer> InternalTypeSerializerList = new Dictionary<Type, DataContractSerializer>();
        public static List<Type> RegistedType
        {
            get
            {
               return publicTypeSerializerList.Keys.ToList();
            }
        }
        #endregion

        #region RegisterSerializer
        public static void RegisterSerializer(Type TargetType, string DefaultNameSpace = "http://www.iierp.com", XmlRootAttribute RootAttribute = null)
        {
            if (TargetType == null)
            {
                throw new ArgumentNullException("TargetType");
            }

            XmlSerializer Serializer = RootAttribute == null? new XmlSerializer(TargetType,DefaultNameSpace) :new XmlSerializer(TargetType,DefaultNameSpace);

            RegisterPublicTypeSerializer(TargetType, new XmlSerializer(TargetType, DefaultNameSpace));

        }
        #endregion

        #region RegisterPublicTypeSerializer
        private static void RegisterPublicTypeSerializer(Type TargetType, XmlSerializer Serializer)
        {
            if (publicTypeSerializerList.ContainsKey(TargetType))
            {
                throw new Exception(string.Format("Serializer already registered! {0}", TargetType.Name));
            }

           
            publicTypeSerializerList.Add(TargetType, Serializer);
        }
        #endregion

        #region RegisterInternalTypeSerializer
        //private static void RegisterInternalTypeSerializer(Type TargetType, DataContractSerializer Serializer)
        //{
        //    if (InternalTypeSerializerList.ContainsKey(TargetType))
        //    {
        //        throw new Exception(string.Format("Serializer already registered! {0}", TargetType.Name));
        //    }

        //    InternalTypeSerializerList.Add(TargetType, Serializer);
        //}
        #endregion

        #region DeserializeFromXml
        //反序列化
        //接收2个参数:xmlFilePath(需要反序列化的XML文件的绝对路径),T(反序列化XML为哪种对象类型)
        public static T DeserializeFromXml<T>(string xmlFilePath)
        {
            if (xmlFilePath == null)
	        {
		         throw new ArgumentNullException("xmlFilePath");
	        }

            if (xmlFilePath.Length < 1)
            {
                throw new ArgumentNullException("xmlFilePath.Length");
            }

            if (File.Exists(xmlFilePath) != true)
            {
                throw new Exception(string.Format("系统找不到此文件：\r\n{0:s}", xmlFilePath));
            }

            Type TragetType = typeof(T);

            if (!publicTypeSerializerList.ContainsKey(TragetType)
                && !InternalTypeSerializerList.ContainsKey(TragetType))
            {
                throw new Exception(string.Format("Unregistered Serializer Type: {0}", TragetType.Name));
            }

            object result = null;

            using (XmlReader reader = XmlReader.Create(xmlFilePath))
            {
                //if (TragetType.IsNotPublic)
                //{
                //    result = InternalTypeSerializerList[TragetType].ReadObject(reader);
                //}
                //else
                //{
                    result = publicTypeSerializerList[TragetType].Deserialize(reader);
                //}
            }

            return (T)result;
        }
        #endregion

        #region DeserializeFromStream
        //反序列化
        //接收2个参数:xmlFilePath(需要反序列化的XML文件的绝对路径),T(反序列化XML为哪种对象类型)
        public static T DeserializeFromStream<T>(Stream XmlStream)
        {
            if (XmlStream == null)
            {
                throw new ArgumentNullException("XmlStream");
            }

            Type TragetType = typeof(T);

            if (!publicTypeSerializerList.ContainsKey(TragetType)
                && !InternalTypeSerializerList.ContainsKey(TragetType))
            {
                throw new Exception(string.Format("Unregistered Serializer Type: {0}", TragetType.Name));
            }

            object result = null;

            using (XmlReader reader = XmlReader.Create(XmlStream))
            {
                //if (TragetType.IsNotPublic)
                //{
                //    result = InternalTypeSerializerList[TragetType].ReadObject(reader);
                //}
                //else
                //{
                    result = publicTypeSerializerList[TragetType].Deserialize(reader);
                //}
            }

            return (T)result;
        }
        #endregion

        #region SerializeToXml
        //反序列化
        //接收2个参数:xmlFilePath(需要反序列化的XML文件的绝对路径),type(反序列化XML为哪种对象类型)
        public static void SerializeToXml(object Obj, string SaveFilePath)
        {
            if (Obj == null)
            {
                throw new ArgumentNullException("Obj");
            }

            if (SaveFilePath == null)
            {
                throw new ArgumentNullException("SaveFilePath");
            }


            Type TragetType = Obj.GetType();

            if (!publicTypeSerializerList.ContainsKey(TragetType)
                && !InternalTypeSerializerList.ContainsKey(TragetType))
            {
                throw new Exception(string.Format("Unregistered Serializer Type: {0}", TragetType.Name));
            }

            BackupConfigFile(SaveFilePath);

            //必须使用FileStream，否则生成的XML文件会丢失格式
            using (FileStream fs = new FileStream(SaveFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None))
            {
                if (TragetType.IsNotPublic)
                {
                    using (XmlWriter Writer = XmlWriter.Create(fs))
                    {
                        Writer.Settings.Encoding = Encoding.UTF8;
                        InternalTypeSerializerList[TragetType].WriteObject(Writer, Obj);
                    }
                }
                else
                {
                    
                    publicTypeSerializerList[TragetType].Serialize(fs, Obj);
                }
            }
        }
        #endregion

        #region Backup Config File
        private static void BackupConfigFile(string SourceFile)
        {
            if (SourceFile != null && File.Exists(SourceFile))
            {
                string[] fileinfo = new string[2];

                fileinfo[0] = Path.GetDirectoryName(SourceFile);
                fileinfo[1] = Path.GetFileNameWithoutExtension(SourceFile) + ".bak";
                string filename = Path.Combine(fileinfo);

                File.Copy(SourceFile, filename, true);
                File.Delete(SourceFile);
            }
        }
        #endregion

        #region 通过xsd验证xml格式是否正确，正确返回空字符串，错误返回提示
        /// <summary>
        /// 通过xsd验证xml格式是否正确，正确返回空字符串，错误返回提示
        /// </summary>
        /// <param name="xmlFile">xml文件</param>
        /// <param name="xsdFile">xsd文件</param>
        /// <param name="namespaceUrl">命名空间，无则默认为null</param>
        /// <returns></returns>
        public static string XmlValidationByXsd(string xmlFile, string xsdFile, string namespaceUrl)
        {
            StringBuilder sb = new StringBuilder();
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.ValidationType = ValidationType.Schema;
            settings.Schemas.Add(namespaceUrl, xsdFile);
            settings.ValidationEventHandler += (x, y) =>
            {
                sb.AppendFormat("{0}|", y.Message);
            };
            using (XmlReader reader = XmlReader.Create(xmlFile, settings))
            {
                try
                {
                    while (reader.Read()) ;
                }
                catch (XmlException ex)
                {
                    sb.AppendFormat("{0}|", ex.Message);
                }
            }
            return sb.ToString();
        }
        #endregion
    }
}
