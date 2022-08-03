using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eraEncoder
{
    class Program
    {
        static void Main(string[] args)
        {
            var folderPath = args[0];
            if (Directory.Exists(folderPath))
            {
                Encoding encoding;
                var outputEncoding = new UTF8Encoding(true);

                var extensions = new string[] { "*.erb", "*.csv" };
                foreach (var ext in extensions)
                {
                    foreach (var item in System.IO.Directory.GetFiles(folderPath, ext, SearchOption.AllDirectories))
                    {
                        var content = ReadTextFile(item, out encoding);
                        if (encoding.EncodingName != outputEncoding.EncodingName)
                            File.WriteAllText(item, content, outputEncoding);
                    }
                }
            }
        }

        /// <summary>
        /// 自動的にエンコード方式を判定してテキストファイルを読み込みます。
        /// Author https://rcie.hatenablog.com/entry/2018/12/31/024901
        /// </summary>
        /// <param name="path">ファイルパス</param>
        /// <param name="enc">エンコード方式</param>
        /// <returns>読み込んだ文字列</returns>
        static string ReadTextFile(string path, out Encoding enc)
        {
            var replacement = new DecoderReplacementFallback("�[FALLBACK]");
            Func<int, Encoding> CodePageをEncodingに = (cp) =>
            {
                var encoding = (Encoding)Encoding.GetEncoding(cp).Clone();
                encoding.DecoderFallback = replacement;
                return encoding;
            };
            int[] aryCP = { 51932, 932, 1200, 1201, 65001 };
            int minLength = int.MaxValue;
            string result = null;
            enc = null;
            byte[] bytes = File.ReadAllBytes(path);
            foreach (var codepage in aryCP)
            {
                var encoding = CodePageをEncodingに(codepage);
                string s = encoding.GetString(bytes);
                int length = Encoding.UTF8.GetByteCount(s);
                if (length < minLength)
                {
                    minLength = length;
                    result = s;
                    enc = encoding;
                }
            }
            return result;
        }
    }
}
