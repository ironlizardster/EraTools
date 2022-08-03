using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace eraRenamer.Config
{
    public static class Configuration
    {
        private static CSV設定 CSV設定Config = null;
        public static CSV設定 GetCSV設定
        {
            get
            {
                if (CSV設定Config == null)
                    CSV設定Config = Configuration.ReadConfiguration<CSV設定>("CSV設定.xml");
                return CSV設定Config;
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
    public partial class CSV設定
    {

        private CSV設定CSV種類[] 基本機能変換Field;

        private CSV設定CSV種類1[] 文字列の変換Field;

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("CSV種類", IsNullable = false)]
        public CSV設定CSV種類[] 基本機能変換
        {
            get
            {
                return this.基本機能変換Field;
            }
            set
            {
                this.基本機能変換Field = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("CSV種類", IsNullable = false)]
        public CSV設定CSV種類1[] 文字列の変換
        {
            get
            {
                return this.文字列の変換Field;
            }
            set
            {
                this.文字列の変換Field = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class CSV設定CSV種類
    {

        private CSV設定CSV種類追加変換トークン[] 追加変換トークンField;

        private string 種類Field;

        private string ファイル名Field;

        private string トークンField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("追加変換トークン")]
        public CSV設定CSV種類追加変換トークン[] 追加変換トークン
        {
            get
            {
                return this.追加変換トークンField;
            }
            set
            {
                this.追加変換トークンField = value;
            }
        }

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
        public string ファイル名
        {
            get
            {
                return this.ファイル名Field;
            }
            set
            {
                this.ファイル名Field = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string トークン
        {
            get
            {
                return this.トークンField;
            }
            set
            {
                this.トークンField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class CSV設定CSV種類追加変換トークン
    {

        private string トークンField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string トークン
        {
            get
            {
                return this.トークンField;
            }
            set
            {
                this.トークンField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class CSV設定CSV種類1
    {

        private string 種類Field;

        private string ファイル名パターンField;

        private string 変換前パラField;

        private string 変換後パラField;

        private string 変換後前置Field;

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
        public string ファイル名パターン
        {
            get
            {
                return this.ファイル名パターンField;
            }
            set
            {
                this.ファイル名パターンField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string 変換前パラ
        {
            get
            {
                return this.変換前パラField;
            }
            set
            {
                this.変換前パラField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string 変換後パラ
        {
            get
            {
                return this.変換後パラField;
            }
            set
            {
                this.変換後パラField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string 変換後前置
        {
            get
            {
                return this.変換後前置Field;
            }
            set
            {
                this.変換後前置Field = value;
            }
        }
    }

}
