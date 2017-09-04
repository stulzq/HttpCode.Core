using System;
using System.IO;
using System.Text;

namespace HttpCode.Core
{
    /// <summary>
    /// 正则表达式静态类
    /// </summary>
    public class RegexString
    {
        /// <summary>
        /// 获取所有的A链接
        /// </summary>
        public static readonly string Alist = "<a[\\s\\S]+?href[=\"\']([\\s\\S]+?)[\"\'\\s+][\\s\\S]+?>([\\s\\S]+?)</a>";
        /// <summary>
        /// 获取所有的Img标签
        /// </summary>
        public static readonly string ImgList = "<img[\\s\\S]*?src=[\"\']([\\s\\S]*?)[\"\'][\\s\\S]*?>([\\s\\S]*?)>";
        /// <summary>
        /// 所有的Nscript
        /// </summary>
        public static readonly string Nscript = "<nscript[\\s\\S]*?>[\\s\\S]*?</nscript>";
        /// <summary>
        /// 所有的Style
        /// </summary>
        public static readonly string Style = "<style[\\s\\S]*?>[\\s\\S]*?</style>";
        /// <summary>
        /// 所有的Script
        /// </summary>
        public static readonly string Script = "<script[\\s\\S]*?>[\\s\\S]*?</script>";
        /// <summary>
        /// 所有的Html
        /// </summary>
        public static readonly string Html = "<[\\s\\S]*?>";
        /// <summary>
        /// 换行符号
        /// </summary>
        public static readonly string NewLine = Environment.NewLine;
        /// <summary>
        ///获取网页编码
        /// </summary>
        public static readonly string Enconding = "<meta[^<]*charset=([^<]*)[\"']";
        /// <summary>
        /// 所有Html
        /// </summary>
        public static readonly string AllHtml = "([\\s\\S]*?)";
        /// <summary>
        /// title
        /// </summary>
        public static readonly string HtmlTitle = "<title>([\\s\\S]*?)</title>";
    }
    /// <summary>
    /// 图片对象 
    /// </summary>
    public class ImgItem
    {
        /// <summary>
        /// 图片网址
        /// </summary>
        public string Src { get; set; }
        /// <summary>
        /// 图片标签Html
        /// </summary>
        public string Html { get; set; }
    }
    /// <summary>
    /// A链接对象   
    /// </summary>
    public class AItem
    {
        /// <summary>
        /// 链接地址
        /// </summary>
        public string Href { get; set; }
        /// <summary>
        /// 链接文本
        /// </summary>
        public string Text { get; set; }
        /// <summary>
        /// 链接的图片 
        /// </summary>
        public ImgItem Img { get; set; }
        /// <summary>
        /// 整个连接Html
        /// </summary>
        public string Html { get; set; }
        /// <summary>
        /// A链接的类型 文本/图像
        /// </summary>
        public AType Type { get; set; }
    }
    /// <summary>
    /// A 标签类型
    /// </summary>
    public enum AType
    {
        /// <summary>
        /// 文本链接(默认)
        /// </summary>
        Text,
        /// <summary>
        /// 图片链接
        /// </summary>
        Img
    }

    /// <summary>
    /// 从流识别编码
    /// </summary>
    public class StreamEncoder
    {
        /// <summary>   
        /// 取得一个文本文件的编码方式。如果无法在文件头部找到有效的前导符，Encoding.Default将被返回。   
        /// </summary>   
        /// <param name="fileName">文件名。</param>   
        /// <returns></returns>   
        public static Encoding GetEncoding(string fileName)
        {
            return GetEncoding(fileName, Encoding.Default);
        }
        /// <summary>   
        /// 取得一个文本文件流的编码方式。   
        /// </summary>   
        /// <param name="stream">文本文件流。</param>   
        /// <returns></returns>   
        public static Encoding GetEncoding(FileStream stream)
        {
            return GetEncoding(stream, Encoding.Default);
        }
        /// <summary>   
        /// 取得一个文本文件的编码方式。   
        /// </summary>   
        /// <param name="fileName">文件名。</param>   
        /// <param name="defaultEncoding">默认编码方式。当该方法无法从文件的头部取得有效的前导符时，将返回该编码方式。</param>   
        /// <returns></returns>   
        public static Encoding GetEncoding(string fileName, Encoding defaultEncoding)
        {
            FileStream fs = new FileStream(fileName, FileMode.Open);
            Encoding targetEncoding = GetEncoding(fs, defaultEncoding);
            fs.Close();
            return targetEncoding;
        }
        /// <summary>   
        /// 取得一个文本文件流的编码方式。   
        /// </summary>   
        /// <param name="stream">文本文件流。</param>   
        /// <param name="defaultEncoding">默认编码方式。当该方法无法从文件的头部取得有效的前导符时，将返回该编码方式。</param>   
        /// <returns></returns>   
        public static Encoding GetEncoding(FileStream stream, Encoding defaultEncoding)
        {
            Encoding targetEncoding = defaultEncoding;
            if (stream != null && stream.Length >= 2)
            {
                //保存文件流的前4个字节   
                byte byte1 = 0;
                byte byte2 = 0;
                byte byte3 = 0;
                byte byte4 = 0;
                //保存当前Seek位置   
                long origPos = stream.Seek(0, SeekOrigin.Begin);
                stream.Seek(0, SeekOrigin.Begin);

                int nByte = stream.ReadByte();
                byte1 = Convert.ToByte(nByte);
                byte2 = Convert.ToByte(stream.ReadByte());
                if (stream.Length >= 3)
                {
                    byte3 = Convert.ToByte(stream.ReadByte());
                }
                if (stream.Length >= 4)
                {
                    byte4 = Convert.ToByte(stream.ReadByte());
                }
                //根据文件流的前4个字节判断Encoding   
                //Unicode {0xFF, 0xFE};   
                //BE-Unicode {0xFE, 0xFF};   
                //UTF8 = {0xEF, 0xBB, 0xBF};   
                if (byte1 == 0xFE && byte2 == 0xFF)//UnicodeBe   
                {
                    targetEncoding = Encoding.BigEndianUnicode;
                }
                if (byte1 == 0xFF && byte2 == 0xFE && byte3 != 0xFF)//Unicode   
                {
                    targetEncoding = Encoding.Unicode;
                }
                if (byte1 == 0xEF && byte2 == 0xBB && byte3 == 0xBF)//UTF8   
                {
                    targetEncoding = Encoding.UTF8;
                }
                //恢复Seek位置         
                stream.Seek(origPos, SeekOrigin.Begin);
            }
            return targetEncoding;
        }
        /// <summary>
        /// 从字节数组中返回字符编码
        /// </summary>
        /// <param name="sen">字节数组</param>
        /// <returns>编码结果</returns>
        public static System.Text.Encoding GetEncodingFromBytes(byte[] sen)
        {
            if (sen.Length == 0)
            {
                return null;
            }
            else
            {
                Stream stream = new MemoryStream(sen);
                return GetEncoding(stream);
            }
        }

        // 新增加一个方法，解决了不带BOM的 UTF8 编码问题    
        /// <summary>   
        /// 通过给定的文件流，判断文件的编码类型   
        /// </summary>   
        /// <param name="fs">文件流</param>   
        /// <returns>文件的编码类型</returns>   
        public static System.Text.Encoding GetEncoding(Stream fs)
        {
            byte[] Unicode = new byte[] { 0xFF, 0xFE, 0x41 };
            byte[] UnicodeBIG = new byte[] { 0xFE, 0xFF, 0x00 };
            byte[] UTF8 = new byte[] { 0xEF, 0xBB, 0xBF }; //带BOM   
            Encoding reVal = Encoding.Default;
            using (BinaryReader r = new BinaryReader(fs, System.Text.Encoding.Default))
            {
                byte[] ss = r.ReadBytes(4);
                if (ss[0] == 0xFE && ss[1] == 0xFF && ss[2] == 0x00)
                {
                    reVal = Encoding.BigEndianUnicode;
                }
                else if (ss[0] == 0xFF && ss[1] == 0xFE && ss[2] == 0x41)
                {
                    reVal = Encoding.Unicode;
                }
                else
                {
                    if (ss[0] == 0xEF && ss[1] == 0xBB && ss[2] == 0xBF)
                    {
                        reVal = Encoding.UTF8;
                    }
                    else
                    {
                        int i;
                        int.TryParse(fs.Length.ToString(), out i);
                        ss = r.ReadBytes(i);

                        if (IsUTF8Bytes(ss))
                            reVal = Encoding.UTF8;
                    }
                }
                r.Close();
                return reVal;
            }
        }

        /// <summary>   
        /// 判断是否是不带 BOM 的 UTF8 格式   
        /// </summary>   
        /// <param name="data"></param>   
        /// <returns></returns>   
        private static bool IsUTF8Bytes(byte[] data)
        {
            int charByteCounter = 1;　 //计算当前正分析的字符应还有的字节数   
            byte curByte; //当前分析的字节.   
            for (int i = 0; i < data.Length; i++)
            {
                curByte = data[i];
                if (charByteCounter == 1)
                {
                    if (curByte >= 0x80)
                    {
                        //判断当前   
                        while (((curByte <<= 1) & 0x80) != 0)
                        {
                            charByteCounter++;
                        }
                        //标记位首位若为非0 则至少以2个1开始 如:110XXXXX...........1111110X　   
                        if (charByteCounter == 1 || charByteCounter > 6)
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    //若是UTF-8 此时第一位必须为1   
                    if ((curByte & 0xC0) != 0x80)
                    {
                        return false;
                    }
                    charByteCounter--;
                }
            }
            if (charByteCounter > 1)
            {
                return false;
            }
            return true;
        }
    }
}
