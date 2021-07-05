using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace HFM.Components
{
    public class SerializeHelper
    {
        public static string Serialize(Type type, object o)
        {
            string result = string.Empty;
            try
            {
                XmlSerializer xs = new XmlSerializer(type);
                MemoryStream ms = new MemoryStream();
                xs.Serialize(ms, o);
                ms.Seek(0, SeekOrigin.Begin);
                StreamReader sr = new StreamReader(ms);
                result = sr.ReadToEnd();
                ms.Close();
            }
            catch (Exception ex)
            {
                throw new Exception("对象序列化成xml时发生错误：" + ex.Message);
            }
            return result;
        }
        /// <summary>
        /// 序列化XML时不带默认的命名空间xmlns
        /// </summary>
        /// <param name="type"></param>
        /// <param name="o"></param>
        /// <returns></returns>
        public static string SerializeNoneNamespaces(Type type, object o)
        {
            string result = string.Empty;
            try
            {
                XmlSerializer xs = new XmlSerializer(type);
                MemoryStream ms = new MemoryStream();
                XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                ns.Add("", "");//Add an empty namespace and empty value
                xs.Serialize(ms, o, ns);
                ms.Seek(0, SeekOrigin.Begin);
                StreamReader sr = new StreamReader(ms);
                result = sr.ReadToEnd();
                ms.Close();
            }
            catch (Exception ex)
            {
                throw new Exception("对象序列化成xml时发生错误：" + ex.Message);
            }
            return result;
        }
        /// <summary>
        /// 序列化时不生成XML声明和XML命名空间
        /// </summary>
        /// <param name="type"></param>
        /// <param name="o"></param>
        /// <returns></returns>
        public static string SerializeNoNamespacesNoXmlDeclaration(Type type, object o)
        {
            string result = string.Empty;
            try
            {
                XmlSerializer xs = new XmlSerializer(type);
                MemoryStream ms = new MemoryStream();
                XmlSerializerNamespaces ns = new XmlSerializerNamespaces();

                XmlWriterSettings settings = new XmlWriterSettings();
                settings.OmitXmlDeclaration = true;//不编写XML声明
                XmlWriter xmlWriter = XmlWriter.Create(ms, settings);

                ns.Add("", "");//Add an empty namespace and empty value
                xs.Serialize(xmlWriter, o, ns);
                ms.Seek(0, SeekOrigin.Begin);
                StreamReader sr = new StreamReader(ms);
                result = sr.ReadToEnd();
                ms.Close();
            }
            catch (Exception ex)
            {
                throw new Exception("对象序列化成xml时发生错误：" + ex.Message);
            }
            return result;
        }
        public static object Deserialize(Type type, string xml)
        {
            object root = new object();
            try
            {
                XmlSerializer xs = new XmlSerializer(type);
                MemoryStream ms = new MemoryStream();
                StreamWriter sw = new StreamWriter(ms);
                sw.Write(xml);
                sw.Flush();
                ms.Seek(0, SeekOrigin.Begin);
                root = xs.Deserialize(ms);
                ms.Close();
            }
            catch (Exception ex)
            {
                throw new Exception("xml转换成对象时发生错误：" + ex.Message);
            }
            return root;
        }



    }
}
