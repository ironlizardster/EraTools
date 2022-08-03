using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace EraCsvManager.Config
{
    public static class Configuration
    {
        private static 文字列変換設定 文字列変換設定Config = null;
        public static 文字列変換設定 Get文字列変換設定
        {
            get
            {
                if (文字列変換設定Config == null)
                    文字列変換設定Config = Configuration.ReadConfiguration<文字列変換設定>("文字列変換設定.xml");
                return 文字列変換設定Config;
            }
        }

        public static T ReadConfiguration<T>(string path) where T : class
        {
            T result = null;
            var serializer = new XmlSerializer(typeof(T));

            using (var reader = new StreamReader(path))
            {
                result = (T)serializer.Deserialize(reader);
            }
            return result;
        }

        public static void WriteConfiguration<T>(string path, T config)
        {
            var serializer = new XmlSerializer(typeof(T));

            using (var writer = new StreamWriter(path))
            {
                serializer.Serialize(writer, config);
            }
        }
    }

    // メモ: 生成されたコードは、少なくとも .NET Framework 4.5または .NET Core/Standard 2.0 が必要な可能性があります。
    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class 文字列変換設定
    {

        private 文字列変換設定ERB変換設定[] eRB変換設定Field;

        private 文字列変換設定ERB逆変換設定[] eRB逆変換設定Field;

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("ERB変換設定", IsNullable = false)]
        public 文字列変換設定ERB変換設定[] ERB変換設定
        {
            get
            {
                return this.eRB変換設定Field;
            }
            set
            {
                this.eRB変換設定Field = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("ERB逆変換設定", IsNullable = false)]
        public 文字列変換設定ERB逆変換設定[] ERB逆変換設定
        {
            get
            {
                return this.eRB逆変換設定Field;
            }
            set
            {
                this.eRB逆変換設定Field = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class 文字列変換設定ERB変換設定
    {

        private string 種類Field;

        private string 変換前パターンField;

        private string 変換後パターンField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string 種類
        {
            get
            {
                return this.種類Field;
            }
            set
            {
                this.種類Field = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string 変換前パターン
        {
            get
            {
                return this.変換前パターンField;
            }
            set
            {
                this.変換前パターンField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string 変換後パターン
        {
            get
            {
                return this.変換後パターンField;
            }
            set
            {
                this.変換後パターンField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class 文字列変換設定ERB逆変換設定
    {

        private string 種類Field;

        private string 変換前パターンField;

        private string 変換後パターンField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string 種類
        {
            get
            {
                return this.種類Field;
            }
            set
            {
                this.種類Field = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string 変換前パターン
        {
            get
            {
                return this.変換前パターンField;
            }
            set
            {
                this.変換前パターンField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string 変換後パターン
        {
            get
            {
                return this.変換後パターンField;
            }
            set
            {
                this.変換後パターンField = value;
            }
        }
    }


}
