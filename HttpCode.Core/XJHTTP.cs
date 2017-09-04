using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HttpCode.Core
{
    /// <summary>
    /// WinInet的方式请求数据
    /// </summary>
    public class Wininet
    {


        #region 字段/属性
        /// <summary>
        /// 默认UserAgent
        /// </summary>
        public string UserAgent = "Mozilla/4.0 (compatible; MSIE 9.0; Windows NT 6.1; 125LA; .NET CLR 2.0.50727; .NET CLR 3.0.04506.648; .NET CLR 3.5.21022)";

        private int _WininetTimeOut = 0;
        /// <summary>
        /// Wininet超时时间 默认0 不设置超时,由于是自行实现(微软没修复超时的bug) 所以如果设置后,每次请求都会暂停.
        /// </summary>
        public int WininetTimeOut
        {
            get { return _WininetTimeOut; }
            set { _WininetTimeOut = value; }
        }

        #endregion


        /// <summary>
        /// 自动解析编码
        /// </summary>
        /// <param name="ms">结果流</param>
        /// <returns>异常时返回null</returns>
        private string EncodingPack(MemoryStream ms)
        {
            Match meta = Regex.Match(Encoding.Default.GetString(ms.ToArray()), "<meta([^<]*)charset=([^<]*)[\"']", RegexOptions.IgnoreCase);
            string c = (meta.Groups.Count > 1) ? meta.Groups[2].Value.ToUpper().Trim() : string.Empty;
            if (c.IndexOf("\"") > 0)
            {
                c = c.Split('\"')[0];
            }
            if (c.Length > 2)
            {
                if (c.IndexOf("UTF-8") != -1)
                {
                    return Encoding.GetEncoding("UTF-8").GetString(ms.ToArray());
                }
            }
            return Encoding.GetEncoding("GBK").GetString(ms.ToArray());
        }
        /// <summary>
        /// 将内存流转换为字符串
        /// </summary>
        /// <param name="mstream">需要转换的流</param>
        /// <returns></returns>
        public string GetDataPro(MemoryStream mstream)
        {
            using (MemoryStream ms = mstream)
            {
                if (ms != null)
                {
                    //无视编码
                    return EncodingPack(ms);
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// 获取网页图片(Image)
        /// </summary>
        /// <param name="mstream">Stream流</param>
        /// <returns></returns>
        public Image GetImage(MemoryStream mstream)
        {
            using (MemoryStream ms = mstream)
            {
                if (ms == null)
                {
                    return null;
                }
                Image img = Image.FromStream(ms);
                return img;
            }
        }



        #region Cookie操作方法
        /// <summary>
        /// 遍历CookieContainer 转换为Cookie集合对象
        /// </summary>
        /// <param name="cc"></param>
        /// <returns>Cookie集合对象</returns>
        public List<Cookie> GetAllCookies(CookieContainer cc)
        {
            List<Cookie> lstCookies = new List<Cookie>();
            Hashtable table = (Hashtable)cc.GetType().InvokeMember("m_domainTable",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.GetField |
                System.Reflection.BindingFlags.Instance, null, cc, new object[] { });

            foreach (object pathList in table.Values)
            {
                SortedList lstCookieCol = (SortedList)pathList.GetType().InvokeMember("m_list",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.GetField
                    | System.Reflection.BindingFlags.Instance, null, pathList, new object[] { });
                foreach (CookieCollection colCookies in lstCookieCol.Values)
                    foreach (Cookie c in colCookies) lstCookies.Add(c);
            }
            return lstCookies;

        }
        /// <summary>
        /// 将String转CookieContainer
        /// </summary>
        /// <param name="Domain">Cookie对应的Domain</param>
        /// <param name="cookie">具体值</param>
        /// <returns>转换后的Container对象</returns>
        public CookieContainer StringToCookie(string Domain, string cookie)
        {
            string[] arrCookie = cookie.Split(';');
            CookieContainer cookie_container = new CookieContainer();    //加载Cookie
            foreach (string sCookie in arrCookie)
            {
                if (!string.IsNullOrEmpty(sCookie))
                {
                    Cookie ck = new Cookie();
                    ck.Name = sCookie.Split('=')[0].Trim();
                    ck.Value = sCookie.Split('=')[1].Trim();
                    ck.Domain = Domain;
                    try
                    {
                        cookie_container.Add(ck);
                    }
                    catch 
                    { 
                        continue;
                    }
                   
                }
            }
            return cookie_container;
        }
        /// <summary>
        /// 将CookieContainer转换为string类型
        /// </summary>
        /// <param name="cc">需要转换的Container对象</param>
        /// <returns>字符串结果</returns>
        public string CookieToString(CookieContainer cc)
        {
            System.Collections.Generic.List<Cookie> lstCookies = new System.Collections.Generic.List<Cookie>();
            Hashtable table = (Hashtable)cc.GetType().InvokeMember("m_domainTable",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.GetField |
                System.Reflection.BindingFlags.Instance, null, cc, new object[] { });
            StringBuilder sb = new StringBuilder();
            foreach (object pathList in table.Values)
            {
                SortedList lstCookieCol = (SortedList)pathList.GetType().InvokeMember("m_list",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.GetField
                    | System.Reflection.BindingFlags.Instance, null, pathList, new object[] { });
                foreach (CookieCollection colCookies in lstCookieCol.Values)
                    foreach (Cookie c in colCookies)
                    {
                        sb.Append(c.Name).Append("=").Append(c.Value).Append(";");
                    }
            }
            return sb.ToString();
        }
        #endregion
    }
    /// <summary>
    /// 系统时间结构体
    /// </summary>
    public struct SystemTime
    {
        /// <summary>
        /// 年
        /// </summary>
        public ushort wYear;
        /// <summary>
        /// 月
        /// </summary>
        public ushort wMonth;
        /// <summary>
        /// 周
        /// </summary>
        public ushort wDayOfWeek;
        /// <summary>
        /// 日
        /// </summary>
        public ushort wDay;
        /// <summary>
        /// 时
        /// </summary>
        public ushort wHour;
        /// <summary>
        /// 分
        /// </summary>
        public ushort wMinute;
        /// <summary>
        /// 秒
        /// </summary>
        public ushort wSecond;
        /// <summary>
        /// 毫秒
        /// </summary>
        public ushort wMiliseconds;
    }
    /// <summary>
    /// 玄机网一键HTTP类库
    /// 懒人库/快捷库
    /// </summary>
    public class XJHTTP
    {
        HttpItems item = new HttpItems();
        HttpHelpers http = new HttpHelpers();
        Wininet wnet = new Wininet();
        HttpResults hr;

        #region Json序列化方法 Framework 2.0下无效 ,默认注释.如需启用 请参考类库文首提示
        /*
        /// <summary>
        /// 将指定的Json字符串转为指定的T类型对象  
        /// </summary>
        /// <param name="jsonstr">字符串</param>
        /// <returns>转换后的对象，失败为Null</returns>
        public object JsonToObject<T>(string jsonstr)
        {
            try
            {
                JavaScriptSerializer jss = new JavaScriptSerializer();
                return jss.Deserialize<T>(jsonstr);
            }
            catch (Exception)
            {
                return null;
            }
        }
        /// <summary>
        /// 将指定的对象转为Json字符串
        /// </summary>
        /// <param name="obj">对象</param>
        /// <returns>转换后的字符串失败为空字符串</returns>
        public string ObjectToJson(object obj)
        {
            try
            {
                JavaScriptSerializer jss = new JavaScriptSerializer();
                return jss.Serialize(obj);
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }*/
        #endregion

        #region 设置/获取系统时间 / 获取当前/指定日期时间戳方法 / GMT时间与本地时间互转
        /// <summary>
        /// 时间戳转为C#格式时间
        /// </summary>
        /// <param name="timeStamp">Unix时间戳格式</param>
        /// <returns>C#格式时间</returns>
        public DateTime GetTime(string timeStamp)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime = long.Parse(timeStamp + "0000");
            TimeSpan toNow = new TimeSpan(lTime);
            return dtStart.Add(toNow);

        }
        /// <summary>
        /// 获取JS时间戳 13位(反射 性能较差如果在乎性能,请考虑GetTimeByCSharp13 方法)
        /// </summary>
        /// <returns></returns>
        public string GetTimeByJs()
        {
            Type obj = Type.GetTypeFromProgID("ScriptControl");
            if (obj == null) return null;
            object ScriptControl = Activator.CreateInstance(obj);
            obj.InvokeMember("Language", BindingFlags.SetProperty, null, ScriptControl, new object[] { "JScript" });
            string js = "function time(){return new Date().getTime()}";
            obj.InvokeMember("AddCode", BindingFlags.InvokeMethod, null, ScriptControl, new object[] { js });
            return obj.InvokeMember("Eval", BindingFlags.InvokeMethod, null, ScriptControl, new object[] { "time()" }).ToString();
        }
        /// <summary>
        /// 返回13位时间戳 非JS方式
        /// </summary>
        /// <param name="nAddSecond"></param>
        /// <returns></returns>
        public string GetTimeByCSharp13(int nAddSecond = 0)
        {
            return (DateTime.UtcNow.AddSeconds(nAddSecond) - DateTime.Parse("1970-01-01 0:0:0")).TotalMilliseconds.ToString("0");
        }

        /// <summary>  
        /// 获取时间戳 C# 10位 
        /// </summary>  
        /// <returns></returns>  
        public string GetTimeByCSharp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }
        /// <summary>
        /// 指定时间转换时间戳
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public string GetTimeToStamp(DateTime time)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            return ((time - startTime).TotalSeconds).ToString();
        }

        /// <summary>
        /// 获取服务器返回的时间,如果Header中没有Date则返回当前时间
        /// </summary>
        /// <param name="hrs">请求结果对象</param>
        /// <returns>返回本地时区Datatime数据</returns>
        public DateTime GetServerTime(HttpResults hrs)
        {
            try
            {
                return GetTime4Gmt(hrs.Header["Date"].ToString());
            }
            catch
            {

                return DateTime.Now;
            }

        }
        /// <summary>
        /// 本地时间转成GMT时间 (参数如果不传入则为当前时间)
        /// 本地时间为：2011-9-29 15:04:39
        /// 转换后的时间为：Thu, 29 Sep 2011 07:04:39 GMT
        /// </summary>
        /// <param name="dt">参数如果不传入则为当前时间 DateTime.Now</param>
        /// <returns></returns>
        public string GetTimeToGMTString(DateTime dt = default(DateTime))
        {
            if (dt == default(DateTime))
            {
                dt = DateTime.Now;
            }
            return dt.ToUniversalTime().ToString("r");
        }

        /// <summary>
        ///本地时间转成GMT格式的时间(参数如果不传入则为当前时间)
        ///本地时间为：2011-9-29 15:04:39
        ///转换后的时间为：Thu, 29 Sep 2011 15:04:39 GMT+0800
        /// </summary>
        /// <param name="dt">参数如果不传入则为当前时间 DateTime.Now</param>
        /// <returns></returns>
        public string GetTimeToGMTFormat(DateTime dt = default(DateTime))
        {

            if (dt == default(DateTime))
            {
                dt = DateTime.Now;
            }
            return dt.ToString("r") + dt.ToString("zzz").Replace(":", "");
        }

        /// <summary>  
        /// GMT时间转成本地时间  
        /// DateTime dt1 = GMT2Local("Thu, 29 Sep 2011 07:04:39 GMT");
        /// 转换后的dt1为：2011-9-29 15:04:39
        /// DateTime dt2 = GMT2Local("Thu, 29 Sep 2011 15:04:39 GMT+0800");
        /// 转换后的dt2为：2011-9-29 15:04:39
        /// </summary>  
        /// <param name="gmt">字符串形式的GMT时间</param>  
        /// <returns></returns>  
        public DateTime GetTime4Gmt(string gmt)
        {
            DateTime dt = DateTime.MinValue;
            try
            {
                string pattern = "";
                if (gmt.IndexOf("+0") != -1)
                {
                    gmt = gmt.Replace("GMT", "");
                    pattern = "ddd, dd MMM yyyy HH':'mm':'ss zzz";
                }
                if (gmt.ToUpper().IndexOf("GMT") != -1)
                {
                    pattern = "ddd, dd MMM yyyy HH':'mm':'ss 'GMT'";
                }
                if (pattern != "")
                {
                    dt = DateTime.ParseExact(gmt, pattern, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AdjustToUniversal);
                    dt = dt.ToLocalTime();
                }
                else
                {
                    dt = Convert.ToDateTime(gmt);
                }
            }
            catch
            {
            }
            return dt;
        }

        #endregion

        #region 字符串处理方法
        /// <summary>
        /// 取文本中间
        /// </summary>
        /// <param name="allStr">原字符</param>
        /// <param name="firstStr">前面的文本</param>
        /// <param name="lastStr">后面的文本</param>
        /// <returns>返回获取的值</returns>
        public string GetStringMid(string allStr, string firstStr, string lastStr)
        {
            //取出前面的位置
            int index1 = allStr.IndexOf(firstStr);
            //取出后面的位置
            int index2 = allStr.IndexOf(lastStr, index1 + firstStr.Length);

            if (index1 < 0 || index2 < 0)
            {
                return "";
            }
            //定位到前面的位置
            index1 = index1 + firstStr.Length;
            //判断要取的文本的长度
            index2 = index2 - index1;

            if (index1 < 0 || index2 < 0)
            {
                return "";
            }
            //取出文本
            return allStr.Substring(index1, index2);
        }
        /// <summary>
        /// 批量取文本中间
        /// </summary>
        /// <param name="allStr">原字符</param>
        /// <param name="firstStr">前面的文本</param>
        /// <param name="lastStr">后面的文本</param>
        /// <param name="regexCode">默认为万能表达式(.*?)</param>
        /// <returns>返回结果集合</returns>
        public List<string> GetStringMids(string allStr, string firstStr, string lastStr, string regexCode = "(.*?)")
        {
            List<string> list = new List<string>();
            string reString = string.Format("{0}{1}{2}", firstStr, regexCode, lastStr);
            Regex reg = new Regex(reString);
            MatchCollection mc = reg.Matches(allStr);
            for (int i = 0; i < mc.Count; i++)
            {
                GroupCollection gc = mc[i].Groups; //得到所有分组 
                for (int j = 1; j < gc.Count; j++) //多分组
                {
                    string temp = gc[j].Value;
                    if (!string.IsNullOrEmpty(temp))
                    {
                        list.Add(temp);
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// URL加密适用于淘宝中文编码算法
        /// </summary>
        /// <param name="str">明文</param>
        /// <returns>密文</returns>
        public string EnUrlMethod(string str)
        {
            byte[] buff = Encoding.Default.GetBytes(str);
            string s = "";
            for (int ix = 0; ix < buff.Length; ix++)
            {
                s += "%" + buff[ix].ToString("x2");
            }
            s = s.ToUpper(); //%62%62%73%2E%6D%73%64%6E%35%2E%63%6F%6D%D0%FE%BB%FA%C2%DB%CC%B3%B3%F6%C6%B7
            return s;
        }
        /// <summary>
        /// Url编码,encoding默认为utf8编码
        /// </summary>
        /// <param name="str">需要编码的字符串</param>
        /// <param name="encoding">指定编码类型</param>
        /// <returns>编码后的字符串</returns>
        public string UrlEncoding(string str, Encoding encoding = null)
        {
            if (encoding == null)
            {
                return System.Web.HttpUtility.UrlEncode(str, Encoding.UTF8);
            }
            else
            {
                return System.Web.HttpUtility.UrlEncode(str, encoding);
            }
        }
        /// <summary>
        /// URL解密适用于淘宝中文编码算法
        /// </summary>
        /// <param name="str">密文</param>
        /// <returns>明文</returns>
        public string DeUrlMethod(string str)
        {
            try
            {
                //改进后更适合的算法
                List<byte> li2 = new List<byte>();
                string[] strs = str.Split('%');
                for (int j = 0; j < strs.Length; j++)
                {
                    if (!string.IsNullOrEmpty(strs[j]))
                    {
                        li2.Add(Convert.ToByte(strs[j], 16));
                    }
                }
                string res = Encoding.Default.GetString(li2.ToArray());//bbs.msdn5.com玄机论坛出品
                return res;
            }
            catch
            {
                return "Error";
            }

        }
        /// <summary>
        /// Url解码,encoding默认为utf8编码
        /// </summary>
        /// <param name="str">需要解码的字符串</param>
        /// <param name="encoding">指定解码类型</param>
        /// <returns>解码后的字符串</returns>
        public string UrlDecoding(string str, Encoding encoding = null)
        {
            if (encoding == null)
            {
                return System.Web.HttpUtility.UrlDecode(str, Encoding.UTF8);
            }
            else
            {
                return System.Web.HttpUtility.UrlDecode(str, encoding);
            }
        }

        /// <summary>
        /// Html解码
        /// </summary>
        /// <param name="str">需要解码的字符</param>
        /// <returns></returns>
        public string HtmlDecode(string str)
        {
            string[] strsx = str.Split('△');
            if (strsx.Length > 1)
            {
                return FromUnicodeString(strsx[0], strsx[1].Trim());//Decode2Html(strsx[0], strsx[1].Trim());
            }
            else
            {
                return Decode2Html(str);
            }
        }
        /// <summary>
        /// 解析任意符号开头的编码后续数据符合Hex编码
        /// </summary>
        /// <param name="param"></param>
        /// <param name="sp"></param>
        /// <returns></returns>
        private string Decode2Html(string param, string sp = "&#")
        {

            string[] paramstr = param.Replace(sp, sp + " ").Replace(sp, "").Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            string str = string.Empty;
            foreach (string item in paramstr)
            {
                try
                {

                    str += (char)int.Parse(item, System.Globalization.NumberStyles.HexNumber);
                }
                catch
                {
                    str += item;
                    continue;
                }
            }
            // 如果失败请尝试这种办法 可解决变异HTML 开头非&# :->  StringWriter myWriter = new StringWriter(); System.Web.HttpUtility.HtmlDecode(param,myWriter); return myWriter.ToString(); 
            return str;
        }

        /// <summary>
        /// Html编码 
        /// </summary>
        /// <param name="param">需要编码的字符</param>
        /// <returns>返回编码后数据</returns>
        public string HtmlEncode(string param)
        {
            string str = string.Empty;
            foreach (char item in param.ToCharArray())
            {
                try
                {
                    str += "&#" + Convert.ToInt32(item).ToString("x4") + " ";
                }
                catch
                {
                    str += "ToHtml Error";
                }
            }

            return str;
        }

        /// <summary>
        /// 取文本右边 
        /// 默认取出右边所有文本,如果需要取固定长度请设置 length参数
        /// 异常则返回空字符串
        /// </summary>
        /// <param name="str">原始字符串</param>
        /// <param name="right">需要确认位置的字符串</param>
        /// <param name="length">默认0,如果设置按照设置的值取出数据</param>
        /// <returns>返回结果</returns>
        public string Right(string str, string right, int length = 0)
        {
            int pos = str.IndexOf(right, StringComparison.Ordinal);
            if (pos < 0) return "";
            int len = str.Length;
            if (len - pos - right.Length <= 0) return "";
            string result = "";
            if (length == 0)
            {
                result = str.Substring(pos + right.Length, len - (pos + right.Length));
            }
            else
            {
                result = str.Substring(pos + right.Length, length);
            }
            return result;
        }
        /// <summary>
        ///  取文本左边
        ///  默认取出左边所有文本,如果需要取固定长度请设置 length参数
        /// 异常则返回空字符串
        /// </summary>
        /// <param name="str">原始字符串</param>
        /// <param name="left">需要确认位置的字符串</param>
        /// <param name="length">默认0,如果设置按照设置的值取出数据</param>
        /// <returns>返回结果</returns>
        public string Left(string str, string left, int length = 0)
        {
            var pos = str.IndexOf(left, StringComparison.Ordinal);
            if (pos < 0) return "";
            string result = "";
            if (length == 0)
            {
                result = str.Substring(0, pos);
            }
            else
            {
                result = str.Substring(length, pos);
            }
            return result;
        }
        /// <summary>
        /// 取文本中间 正则方式
        /// </summary>
        /// <param name="html">原始Html</param>
        /// <param name="s">开始字符串</param>
        /// <param name="e">结束字符串</param>
        /// <returns>返回获取结果</returns>
        public string GetMidHtml(string html, string s, string e)
        {
            string rx = string.Format("{0}{1}{2}", s, RegexString.AllHtml, e);
            if (Regex.IsMatch(html, rx, RegexOptions.IgnoreCase))
            {
                Match match = Regex.Match(html, rx, RegexOptions.IgnoreCase);
                if (match != null && match.Groups.Count > 0)
                {
                    return match.Groups[1].Value.Trim();
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// Unicode字符转汉字 允许自定义分隔字符
        /// </summary>
        /// <param name="str">需要转换的字符串</param>
        /// <param name="SplitString">分隔字符</param>
        /// <param name="TrimStr">如果有尾部数据则填写尾部</param>
        /// <returns>处理后结果</returns>
        public string FromUnicodeString(string str, string SplitString = "u", string TrimStr = ";")
        {
            string regexCode = SplitString == "u" ? "\\\\u(\\w{1,4})" : SplitString + "(\\w{1,4})";
            string reString = str;
            System.Text.RegularExpressions.Regex reg = new System.Text.RegularExpressions.Regex(regexCode);
            System.Text.RegularExpressions.MatchCollection mc = reg.Matches(reString);
            for (int i = 0; i < mc.Count; i++)
            {
                try
                {
                    var outs = (char)int.Parse(mc[i].Groups[1].Value, System.Globalization.NumberStyles.HexNumber);
                    if (str.IndexOf(mc[i].Groups[0].Value + TrimStr) > 0)
                    {
                        //如果出现(封号);结尾则连带符号替换
                        str = str.Replace(mc[i].Groups[0].Value + TrimStr, outs.ToString());
                    }
                    else
                    {
                        str = str.Replace(mc[i].Groups[0].Value, outs.ToString());
                    }
                }
                catch
                {
                    continue;
                }
            }
            return str;
        }
        /// <summary>
        /// 汉字转Unicode字符 默认\u1234 
        /// </summary>
        /// <param name="param">需要转换的字符</param>
        /// <param name="SplitString">分隔结果</param>
        /// <returns>转换后结果</returns>
        public string GetUnicodeString(string param, string SplitString = "u")
        {
            string outStr = "";
            for (int i = 0; i < param.Length; i++)
            {
                try
                {
                    outStr += "\\" + SplitString + ((int)param[i]).ToString("x4");
                }
                catch
                {
                    outStr += param[i];
                    continue;
                }

            }

            return outStr;
        }
        /// <summary>
        /// 将字符串转换为base64格式 默认UTF8编码
        /// </summary>
        /// <param name="str">原始字符串</param>
        /// <param name="encoding">编码</param>
        /// <returns>结果</returns>
        public string GetString2Base64(string str, Encoding encoding = null)
        {
            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }
            return Convert.ToBase64String(encoding.GetBytes(str));
        }
        /// <summary>
        /// base64字符串转换为普通格式 默认UTF8编码
        /// </summary>
        /// <param name="str">原始字符串</param>
        /// <param name="encoding">编码</param>
        /// <returns>结果</returns>
        public string GetStringbyBase64(string str, Encoding encoding = null)
        {
            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }
            byte[] buffer = Convert.FromBase64String(str);
            return encoding.GetString(buffer);
        }
        /// <summary>
        /// 将byte数组转换为AscII字符
        /// </summary>
        /// <param name="b">需要操作的数组</param>
        /// <returns>结果</returns>
        public string GetAscii2string(byte[] b)
        {
            string str = "";
            for (int i = 7; i < 19; i++)
            {
                str += (char)b[i];
            }
            return str;
        }

        /// <summary>
        /// 将字节数组转化为十六进制字符串，每字节表示为两位
        /// </summary>
        /// <param name="bytes">需要操作的数组</param>
        /// <param name="start">起始位置</param>
        /// <param name="len">长度</param>
        /// <returns>字符串结果</returns>
        public string Bytes2HexString(byte[] bytes, int start, int len)
        {
            string tmpStr = "";

            for (int i = start; i < (start + len); i++)
            {
                tmpStr = tmpStr + bytes[i].ToString("X2");
            }

            return tmpStr;
        }
        /// <summary>
        /// 字符串转16进制
        /// </summary>
        /// <param name="mHex">需要转换的字符串</param>
        /// <returns>返回十六进制代表的字符串</returns>
        public string HexToStr(string mHex) // 返回十六进制代表的字符串 
        {
            byte[] bTemp = System.Text.Encoding.Default.GetBytes(mHex);
            string strTemp = "";
            for (int i = 0; i < bTemp.Length; i++)
            {
                strTemp += bTemp[i].ToString("X");
            }
            return strTemp;


        }
        /// <summary>
        /// 将十六进制字符串转化为字节数组
        /// </summary>
        /// <param name="src">需要转换的字符串</param>
        /// <returns>结果数据</returns>
        public byte[] HexString2Bytes(string src)
        {
            byte[] retBytes = new byte[src.Length / 2];

            for (int i = 0; i < src.Length / 2; i++)
            {
                retBytes[i] = byte.Parse(src.Substring(i * 2, 2), System.Globalization.NumberStyles.AllowHexSpecifier);
            }

            return retBytes;
        }
        #endregion

        #region Cookie维护处理方法
        /// <summary>
        /// 合并Cookie，将cookie2与cookie1合并更新 返回字符串类型Cookie
        /// </summary>
        /// <param name="cookie1">旧cookie</param>
        /// <param name="cookie2">新cookie</param>
        /// <returns></returns>
        public string UpdateCookie(string cookie1, string cookie2)
        {
            StringBuilder sb = new StringBuilder();

            Dictionary<string, string> dicCookie = new Dictionary<string, string>();
            //遍历cookie1
            if (!string.IsNullOrEmpty(cookie1))
            {
                foreach (string cookie in cookie1.Replace(',', ';').Split(';'))
                {
                    if (!string.IsNullOrEmpty(cookie) && cookie.IndexOf('=') > 0)
                    {
                        string key = cookie.Split('=')[0].Trim();
                        string value = cookie.Substring(key.Length + 1).Trim();
                        if (dicCookie.ContainsKey(key))
                        {
                            dicCookie[key] = cookie;
                        }
                        else
                        {
                            dicCookie.Add(key, cookie);
                        }
                    }
                }
            }

            if (!string.IsNullOrEmpty(cookie2))
            {
                //遍历cookie2
                foreach (string cookie in cookie2.Replace(',', ';').Split(';'))
                {
                    if (!string.IsNullOrEmpty(cookie) && cookie.IndexOf('=') > 0)
                    {
                        string key = cookie.Split('=')[0].Trim();
                        string value = cookie.Substring(key.Length + 1).Trim();
                        if (dicCookie.ContainsKey(key))
                        {
                            dicCookie[key] = cookie;
                        }
                        else
                        {
                            dicCookie.Add(key, cookie);
                        }
                    }
                }
            }
            int i = 0;
            foreach (var item in dicCookie)
            {
                i++;
                if (i < dicCookie.Count)
                {
                    sb.Append(item.Value + ";");
                }
                else
                {
                    sb.Append(item.Value);
                }
            }
            return sb.ToString();

        }
        /// <summary>
        /// 清理string类型Cookie.剔除无用项返回结果为null时遇见错误.
        /// </summary>
        /// <param name="Cookies"></param>
        /// <returns></returns>
        public string ClearCookie(string Cookies)
        {
            try
            {
                string rStr = string.Empty;
                Cookies = Cookies.Replace(";", "; ");
                 string clStr = "(?<cookie>[^ ]+=(?!deleted;)[^;]+);";
                Again: 
                Regex r = new Regex(clStr);//"(?<=,)(?<cookie>[^ ]+=(?!deleted;)[^;]+);");
                Match m = r.Match(Cookies);
                while (m.Success)
                {
                    rStr += m.Groups["cookie"].Value + ";";
                    m = m.NextMatch();
                }
                if (rStr.Contains("path"))
                { 
                    clStr = "(?<=,)(?<cookie>[^ ]+=(?!deleted;)[^;]+);";
                    rStr = rStr.Split(new string[] { "path" }, StringSplitOptions.RemoveEmptyEntries)[0];
                    
                }
                return rStr;
            }
            catch
            {
                return string.Empty;
            }

        }

        /// <summary>
        /// 获取当前请求所有Cookie
        /// </summary>
        /// <param name="items"></param>
        /// <returns>Cookie集合</returns>
        public List<Cookie> GetAllCookieByHttpItems(HttpItems items)
        {
            return wnet.GetAllCookies(items.Container);
        }

        /// <summary>
        /// 获取CookieContainer 中的所有对象
        /// </summary>
        /// <param name="cc"></param>
        /// <returns></returns>
        public List<Cookie> GetAllCookie(CookieContainer cc)
        {
            return wnet.GetAllCookies(cc);
        }
        /// <summary>
        /// 将 CookieContainer 对象转换为字符串类型
        /// </summary>
        /// <param name="cc"></param>
        /// <returns></returns>
        public string CookieTostring(CookieContainer cc)
        {
            return wnet.CookieToString(cc);
        }
        /// <summary>
        /// 将文字Cookie转换为CookieContainer 对象
        /// </summary>
        /// <param name="url"></param>
        /// <param name="cookie"></param>
        /// <returns></returns>
        public CookieContainer StringToCookie(string url, string cookie)
        {

            return wnet.StringToCookie(url, cookie);
        }
        /// <summary>
        /// 【不推荐使用】 推荐使用字符串方式维护Cookie
        /// 将StringCookie 添加到 CookieContainer对象中
        /// 当CookieContainer出现问题时请调用本方法修复 CookieContainer对象
        /// 更新失败时,返回原来的CookieContainer对象
        /// 
        /// </summary>
        /// <param name="cookie_container">需要更新的cookie_container</param>
        /// <param name="cookie">字符串cookie</param>
        /// <param name="domain">domain默认为空</param>
        /// <returns></returns>
        public CookieContainer AddCookieToContainer(CookieContainer cookie_container, string cookie, string domain = "")
        {
            if (cookie_container == null)
            {
                cookie_container = new CookieContainer();
            }
            Regex reg = new Regex("domain=(.*?);");
            string rdomain = string.Empty;
            if (reg.IsMatch(cookie))
            {
                rdomain = reg.Match(cookie).Groups[1].Value;
            }
            else
            {
                // 有指定域名就用指定的
                if (domain.Length > 0)
                {
                    rdomain = domain;
                }//匹配失败 如果cookie_container中的数据为0那么直接返回,本次更新失败
                else if (cookie_container.Count > 0)
                {
                    try
                    {
                        rdomain = new XJHTTP().GetAllCookie(cookie_container)[0].Domain; //得到原cookie_container中的domain.  如果失败,则返回原先对象
                    }
                    catch
                    {
                        return cookie_container;
                    }

                }
            }
            string[] arrCookie = cookie.Split(';');
            foreach (string sCookie in arrCookie)
            {

                string vsCookie = sCookie.Replace("HttpOnly,", "").Trim(); //剔除httponly
                if (vsCookie.Contains("expires")) //去除过滤时间
                {
                    continue;
                }
                if (vsCookie.Split(',').Length > 1) //去掉,分隔的cookie
                {
                    vsCookie = vsCookie.Split(',')[1];
                }
                try
                {
                    if (!string.IsNullOrEmpty(vsCookie))
                    {
                        Cookie ck = new Cookie();
                        ck.Name = vsCookie.Split(new Char[] { '=' }, 2)[0].Trim();
                        ck.Value = vsCookie.Split(new Char[] { '=' }, 2)[1].Trim();
                        ck.Domain = rdomain;
                        cookie_container.Add(ck);
                    }
                }
                catch
                {
                    continue;
                }

            }
            return cookie_container;
        }

        #endregion

        #region 设置/删除IE(Webbrowser) Cookie 方法

        ///
        /// 设置cookie
        ///
        [DllImport("wininet.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool InternetSetCookie(string lpszUrlName, string lbszCookieName, string lpszCookieData);
        ///
        /// 获取cookie
        ///
        [DllImport("wininet.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool InternetGetCookie(
        string url, string name, StringBuilder data, ref int dataSize);
        /// <summary>
        /// 设置IE cookie
        /// </summary>
        /// <param name="GetUrl">URL</param>
        /// <param name="NewCookie">Cookie</param>
        public void SetIeCookie(string GetUrl, string NewCookie)
        {
            //获取旧的
            StringBuilder cookie = new StringBuilder(new String(' ', 2048));
            int datasize = cookie.Length;
            bool b = InternetGetCookie(GetUrl, null, cookie, ref datasize);
            //删除旧的
            foreach (string fileName in System.IO.Directory.GetFiles(System.Environment.GetFolderPath(Environment.SpecialFolder.Cookies)))
            {
                if (fileName.ToLower().IndexOf(GetUrl) > 0)
                {
                    System.IO.File.Delete(GetUrl);
                }
            }
            //生成新的
            foreach (string c in NewCookie.Split(';'))
            {
                string[] item = c.Split('=');
                string name = item[0];
                string value = item[1] + ";expires=Sun,22-Feb-2099 00:00:00 GMT";
                InternetSetCookie(GetUrl, name, value);
                InternetSetCookie(GetUrl, name, value);
                InternetSetCookie(GetUrl, name, value);
            }
        }
        /// <summary>
        /// 删除IE COOKIE
        /// </summary>
        public void ClearIECookie()
        {
            CleanAll();
            InternetSetOption(IntPtr.Zero, INTERNET_OPTION_END_BROWSER_SESSION, IntPtr.Zero, 0);
        }

        #region 删除IE COOKIE具体代码
        /// <summary>
        /// 默认常量
        /// </summary>
        public const int INTERNET_OPTION_END_BROWSER_SESSION = 42;
        #region Web清理
        /// <summary>
        /// 设置IE
        /// </summary>
        /// <param name="hInternet">hInternet</param>
        /// <param name="dwOption">dwOption</param>
        /// <param name="lpBuffer">lpBuffer</param>
        /// <param name="lpdwBufferLength">lpdwBufferLength</param>
        /// <returns>处理结果</returns>
        [DllImport("wininet.dll", SetLastError = true)]
        public static extern bool InternetSetOption(IntPtr hInternet, int dwOption, IntPtr lpBuffer, int lpdwBufferLength);
        /*
         * 7 个静态函数
         * 私有函数
         * private bool FileDelete()    : 删除文件
         * private void FolderClear()   : 清除文件夹内的所有文件
         * private void RunCmd(): 运行内部命令
         *
         * 公有函数
         * public void CleanCookie()    : 删除Cookie
         * public void CleanHistory()   : 删除历史记录
         * public void CleanTempFiles() : 删除临时文件
         * public void CleanAll()       : 删除所有
         *
         *
         *
         * */
        //private
        /// <summary>
        /// 删除一个文件，System.IO.File.Delete()函数不可以删除只读文件，这个函数可以强行把只读文件删除。
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <returns>成功为true</returns>
        public bool FileDelete(string path)
        {
            //first set the File\'s ReadOnly to 0
            //if EXP, restore its Attributes
            System.IO.FileInfo file = new System.IO.FileInfo(path);
            System.IO.FileAttributes att = 0;
            bool attModified = false;
            try
            {
                //### ATT_GETnSET
                att = file.Attributes;
                file.Attributes &= (~System.IO.FileAttributes.ReadOnly);
                attModified = true;
                file.Delete();

            }
            catch
            {
                if (attModified)
                    file.Attributes = att;
                return false;

            }
            return true;
        }
        /// <summary>
        /// 删除文件夹
        /// </summary>
        /// <param name="path">文件夹路径</param>
        public void FolderClear(string path)
        {
            System.IO.DirectoryInfo diPath = new System.IO.DirectoryInfo(path);
            foreach (System.IO.FileInfo fiCurrFile in diPath.GetFiles())
            {
                FileDelete(fiCurrFile.FullName);

            }
            foreach (System.IO.DirectoryInfo diSubFolder in diPath.GetDirectories())
            {
                FolderClear(diSubFolder.FullName); // Call recursively for all subfolders

            }

        }
        /// <summary>
        /// 删除历史记录  Win7+ 需要管理员权限
        /// </summary>
        public void CleanHistory()
        {
            string[] theFiles = System.IO.Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.History), "*", System.IO.SearchOption.AllDirectories);
            foreach (string s in theFiles)
                FileDelete(s);
            RunCmd("RunDll32.exe InetCpl.cpl,ClearMyTracksByProcess 1");
        }
        /// <summary>
        ///  删除临时文件  Win7+ 需要管理员权限
        /// </summary>
        public void CleanTempFiles()
        {
            FolderClear(Environment.GetFolderPath(Environment.SpecialFolder.InternetCache));
            RunCmd("RunDll32.exe InetCpl.cpl,ClearMyTracksByProcess 8");
        }
        /// <summary>
        /// 删除Cookie文件 Win7+ 需要管理员权限
        /// </summary>
        public void CleanCookie()
        {
            string[] theFiles = System.IO.Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.Cookies), "*", System.IO.SearchOption.AllDirectories);
            foreach (string s in theFiles)
                FileDelete(s);
            RunCmd("RunDll32.exe InetCpl.cpl,ClearMyTracksByProcess 2");
        }
        ///
        /// 删除全部 历史记录,Cookie,临时文件
        ///
        public void CleanAll()
        {
            CleanHistory();
            CleanCookie();
            CleanTempFiles();
        }

        #endregion
        /// <summary>
        /// 调用CMD执行命令
        /// </summary>
        /// <param name="cmd"></param>
        public void RunCmd(string cmd)
        {
            System.Diagnostics.Process p = new System.Diagnostics.Process();
            p.StartInfo.FileName = "cmd.exe";
            // 关闭Shell的使用
            p.StartInfo.UseShellExecute = false;
            // 重定向标准输入
            p.StartInfo.RedirectStandardInput = true;
            // 重定向标准输出
            p.StartInfo.RedirectStandardOutput = true;
            //重定向错误输出
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.CreateNoWindow = true;
            p.Start();
            p.StandardInput.WriteLine(cmd);
            p.StandardInput.WriteLine("exit");
        }

        #endregion

        #endregion

        #region 反射执行JS/调用IE/默认浏览器打开URL/MD5加密 等

        /// <summary>
        /// 执行js代码(JS代码,参数,调用方法名,方法名[默认Eval 可选Run])
        /// </summary>
        /// <param name="reString">JS代码</param>
        /// <param name="para">参数</param>
        /// <param name="MethodName">调用方法名</param>
        /// <param name="Method">方法名:默认Eval 可选Run</param>
        /// <returns></returns>
        public string RunJsMethod(string reString, string para, string MethodName, string Method = "Eval")
        {
            try
            {
                Type obj = Type.GetTypeFromProgID("ScriptControl");
                if (obj == null) return string.Empty;
                object ScriptControl = Activator.CreateInstance(obj);
                obj.InvokeMember("Language", BindingFlags.SetProperty, null, ScriptControl, new object[] { "JScript" });
                obj.InvokeMember("AddCode", BindingFlags.InvokeMethod, null, ScriptControl, new object[] { reString });
                object objx = obj.InvokeMember(Method, BindingFlags.InvokeMethod, null, ScriptControl, new object[] { string.Format("{0}({1})", MethodName, para) }).ToString();//执行结果
                if (objx == null)
                {
                    return string.Empty;
                }
                return objx.ToString();
            }
            catch (Exception ex)
            {
                string ErrorInfo = string.Format("执行JS出现错误:   \r\n 错误描述: {0} \r\n 错误原因: {1} \r\n 错误来源:{2}", ex.Message, ex.InnerException.Message, ex.InnerException.Source);//异常信息
                return ErrorInfo;
            }
        }
        /// <summary>
        /// 打开指定URL openType:0使用IE打开,!=0 使用默认浏览器打开
        /// </summary>
        /// <param name="url">需要打开的地址</param>
        /// <param name="openType">0使用IE,其他使用默认</param>
        public void OpenUrl(string url, int openType = 0)
        {
            // 调用ie打开网页
            if (openType == 0)
            {
                System.Diagnostics.Process.Start("IEXPLORE.EXE", url);
            }
            else
            {
                System.Diagnostics.Process.Start(url);
            }
        }
        /// <summary>
        /// 字符串MD5加密
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public string EncryptMD5String(string str)
        {
            using (MD5 md5String = MD5.Create())
            {
                StringBuilder sb = new StringBuilder();
                byte[] bytes = Encoding.UTF8.GetBytes(str);
                byte[] md5Encrypt = md5String.ComputeHash(bytes);
                for (int i = 0; i < md5Encrypt.Length; i++)
                {
                    sb.Append(md5Encrypt[i].ToString("x2"));
                }
                return sb.ToString();
            }
        }
        #endregion

        #region Html处理方法

        /// <summary>
        /// 获取所有的A标签
        /// </summary>
        /// <param name="html">要分析的Html代码</param>
        /// <returns>返回一个List存储所有的A标签</returns>
        public List<AItem> GetAList(string html)
        {
            List<AItem> list = null;
            string ra = RegexString.Alist;
            if (Regex.IsMatch(html, ra, RegexOptions.IgnoreCase))
            {
                list = new List<AItem>();
                foreach (Match item in Regex.Matches(html, ra, RegexOptions.IgnoreCase))
                {
                    AItem a = null;
                    try
                    {
                        a = new AItem()
                        {
                            Href = item.Groups[1].Value,
                            Text = item.Groups[2].Value,
                            Html = item.Value,
                            Type = AType.Text
                        };
                        List<ImgItem> imglist = GetImgList(a.Text);
                        if (imglist != null && imglist.Count > 0)
                        {
                            a.Type = AType.Img;
                            a.Img = imglist[0];
                        }
                    }
                    catch { continue; }
                    if (a != null)
                    {
                        list.Add(a);
                    }
                }
            }
            return list;
        }
        /// <summary>
        /// 获取所有的Img标签
        /// </summary>
        /// <param name="html">要分析的Html代码</param>
        /// <returns>返回一个List存储所有的Img标签</returns>
        public List<ImgItem> GetImgList(string html)
        {
            List<ImgItem> list = null;
            string ra = RegexString.ImgList;
            if (Regex.IsMatch(html, ra, RegexOptions.IgnoreCase))
            {
                list = new List<ImgItem>();
                foreach (Match item in Regex.Matches(html, ra, RegexOptions.IgnoreCase))
                {
                    ImgItem a = null;
                    try
                    {
                        a = new ImgItem()
                        {
                            Src = item.Groups[1].Value,
                            Html = item.Value
                        };
                    }
                    catch { continue; }
                    if (a != null)
                    {
                        list.Add(a);
                    }
                }
            }
            return list;
        }
        /// <summary>
        /// 过滤html标签
        /// </summary>
        /// <param name="html">html的内容</param>
        /// <returns>处理后的文本</returns>
        public string StripHTML(string html)
        {
            html = Regex.Replace(html, RegexString.Nscript, string.Empty, RegexOptions.IgnoreCase | RegexOptions.Compiled);
            html = Regex.Replace(html, RegexString.Style, string.Empty, RegexOptions.IgnoreCase | RegexOptions.Compiled);
            html = Regex.Replace(html, RegexString.Script, string.Empty, RegexOptions.IgnoreCase | RegexOptions.Compiled);
            html = Regex.Replace(html, RegexString.Html, string.Empty, RegexOptions.IgnoreCase | RegexOptions.Compiled);
            return html;
        }
        /// <summary>
        /// 过滤html中所有的换行符号
        /// </summary>
        /// <param name="html">html的内容</param>
        /// <returns>处理后的文本</returns>
        public string ReplaceNewLine(string html)
        {
            return Regex.Replace(html, RegexString.NewLine, string.Empty, RegexOptions.IgnoreCase | RegexOptions.Compiled);
        }
        /// <summary>
        /// 提取网页Title
        /// </summary>
        /// <param name="html">Html</param>
        /// <returns>返回Title</returns>
        public string GetHtmlTitle(string html)
        {
            if (Regex.IsMatch(html, RegexString.HtmlTitle))
            {
                return Regex.Match(html, RegexString.HtmlTitle).Groups[1].Value.Trim();
            }
            else
            {
                return string.Empty;
            }
        }
        #endregion

        #region 懒人方法

        /// <summary>
        /// 文件下载[如果连接不是绝对路径存在跳转默认会自动跳转]
        /// 会自动分析协议头中的filename
        /// 如果分析失败则直接存储为默认名[默认为:.zip格式].
        /// 成功返回true;
        /// </summary>
        /// <param name="url">下载地址</param>
        /// <param name="paths">保存绝对路径 如:c://download//</param>
        /// <param name="cc">Cookie</param>
        /// <param name="defaultName">默认后缀</param>
        /// <returns></returns>
        public bool DonwnLoad(string url, string paths, CookieContainer cc, string defaultName = ".zip")
        {
            try
            {
                item.Url = url;
                item.Allowautoredirect = true;
                item.ResultType = ResultType.Byte;
                item.Container = cc;
                //item.Container = wnet.StringToCookie(url, wnet.GetCookies(url));
                hr = http.GetHtml(item);
                string strname = hr.Header["Content-Disposition"].Split(new string[] { "filename=" }, StringSplitOptions.RemoveEmptyEntries)[1];
                byte[] buffer = hr.ResultByte;
                try
                {
                    File.WriteAllBytes(paths + strname, buffer);
                }
                catch
                {
                    File.WriteAllBytes(paths + defaultName, buffer);
                }
                return true;

            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// 文件下载[如果连接不是绝对路径存在跳转默认会自动跳转]
        /// 会自动分析协议头中的filename
        /// 如果分析失败则直接存储为默认名[默认为:.zip格式]
        /// 成功返回true;
        /// </summary>
        /// <param name="Url">请求地址</param>
        /// <param name="paths">保存位置</param>
        /// <param name="Referer">referer</param>
        /// <param name="cc">cc</param>
        /// <param name="Encoder">编码</param>
        /// <param name="defaultName">默认后缀</param>
        /// <returns></returns>
        public bool WebClientDonwnLoad(string Url, string paths, string Referer, ref CookieContainer cc, Encoding Encoder = null, string defaultName = ".zip")
        {
            try
            {
                if (Encoder == null)
                {
                    Encoder = Encoding.UTF8;
                }
                WebClient myClient = new WebClient();
                myClient.Headers.Add("Accept: */*");
                myClient.Headers.Add("User-Agent: Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1; Trident/4.0; .NET4.0E; .NET4.0C; InfoPath.2; .NET CLR 2.0.50727; .NET CLR 3.0.04506.648; .NET CLR 3.5.21022; .NET CLR 3.0.4506.2152; .NET CLR 3.5.30729; SE 2.X MetaSr 1.0)");
                myClient.Headers.Add("Accept-Language: zh-cn");
                myClient.Headers.Add("Content-Type: multipart/form-data");
                myClient.Headers.Add("Accept-Encoding: gzip, deflate");
                myClient.Headers.Add("Cache-Control: no-cache");
                myClient.Headers.Add(CookieTostring(cc));
                myClient.Encoding = Encoder;
                byte[] buffer = myClient.DownloadData(Url);
                string strname = myClient.Headers["Content-Disposition"].Split(new string[] { "filename=" }, StringSplitOptions.RemoveEmptyEntries)[1];
                //处理cookie
                cc = StringToCookie(Url, myClient.ResponseHeaders["Set-Cookie"].ToString());
                try
                {
                    File.WriteAllBytes(paths + strname, buffer);
                }
                catch
                {
                    File.WriteAllBytes(paths + defaultName, buffer);
                }
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }
        /// <summary>
        /// WebClient Post 上传
        /// 用于上传类型为multipart/form-data 
        /// 如果上传失败,请检查协议头是否有自定义协议头.如Ajax头
        /// </summary>
        /// <param name="Url">上传地址</param>
        /// <param name="Referer">referer</param>
        /// <param name="PostData"></param>
        /// <param name="cc">Cookie</param>
        /// <param name="Encoder">编码默认utf8</param> 
        /// <returns></returns>
        public string UploadPost(string Url, string Referer, string PostData, ref CookieContainer cc, Encoding Encoder = null)
        {
            if (Encoder == null)
            {
                Encoder = Encoding.UTF8;
            }
            string result = "";
            WebClient myClient = new WebClient();
            myClient.Headers.Add("Accept: */*");
            myClient.Headers.Add("User-Agent: Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1; Trident/4.0; .NET4.0E; .NET4.0C; InfoPath.2; .NET CLR 2.0.50727; .NET CLR 3.0.04506.648; .NET CLR 3.5.21022; .NET CLR 3.0.4506.2152; .NET CLR 3.5.30729; SE 2.X MetaSr 1.0)");
            myClient.Headers.Add("Accept-Language: zh-cn");
            myClient.Headers.Add("Content-Type: multipart/form-data");
            myClient.Headers.Add("Accept-Encoding: gzip, deflate");
            myClient.Headers.Add("Cache-Control: no-cache");
            myClient.Headers.Add(CookieTostring(cc));
            myClient.Encoding = Encoder;
            result = myClient.UploadString(Url, PostData);
            //处理cookie
            cc = StringToCookie(Url, myClient.ResponseHeaders["Set-Cookie"].ToString());
            return result;

        }
        /// <summary>
        /// 普通请求.直接返回标准结果
        /// </summary>
        /// <param name="url">请求的URL</param>
        /// <returns>返回结果</returns>
        public HttpResults GetHtml(string url)
        {
            item.Url = url;
            return http.GetHtml(item);
        }
        /// <summary>
        /// 普通请求.直接返回标准结果
        /// </summary>
        /// <param name="url">请求的URL</param>
        /// <param name="ipProxy">代理IP地址</param>
        /// <param name="TimeOut">超时时间</param>
        /// <returns>结果对象</returns>
        public HttpResults GetHtml(string url, string ipProxy, int TimeOut = 15000)
        {
            item.Url = url;
            item.ProxyIp = ipProxy;
            item.Timeout = TimeOut;
            return http.GetHtml(item);
        }
        /// <summary>
        /// 普通请求.直接返回标准结果
        /// </summary>
        /// <param name="url">请求的URL</param>
        /// <param name="cc">当前Cookie</param>
        /// <param name="TimeOut">超时时间</param>
        /// <returns>结果对象</returns>
        public HttpResults GetHtml(string url, CookieContainer cc, int TimeOut = 15000)
        {
            item.Url = url;
            item.Container = cc;
            item.Timeout = TimeOut;
            return http.GetHtml(item);
        }
        /// <summary>
        /// 普通请求.直接返回标准结果
        /// </summary>
        /// <param name="url">请求的URL</param>
        /// <param name="cc">当前Cookie</param>
        ///<param name="ipProxy">代理IP地址</param>
        /// <param name="TimeOut">超时时间</param>
        /// <returns>结果对象</returns>
        public HttpResults GetHtml(string url, CookieContainer cc, string ipProxy, int TimeOut = 15000)
        {
            item.Url = url;
            item.ProxyIp = ipProxy;
            item.Container = cc;
            item.Timeout = TimeOut;
            return http.GetHtml(item);
        }
        /// <summary>
        ///  普通请求.直接返回标准结果
        /// </summary>
        /// <param name="picurl">图片请求地址</param>
        /// <param name="referer">上一次请求地址</param>
        /// <param name="cc">当前Cookie</param>
        /// <param name="TimeOut">超时时间</param>
        /// <returns></returns>
        public HttpResults GetImage(string picurl, string referer, CookieContainer cc, int TimeOut = 15000)
        {
            item.Url = picurl;
            item.Referer = referer;
            item.Container = cc;
            item.ResultType = ResultType.Byte;
            item.Timeout = TimeOut;
            return http.GetHtml(item);
        }
        /// <summary>
        /// 普通请求.直接返回Image格式图像
        /// </summary>
        /// <param name="picurl">图片请求地址</param>
        /// <param name="referer">上一次请求地址</param>
        /// <param name="cc">当前Cookie</param>
        /// <returns></returns>
        public Image GetImageByImage(string picurl, string referer, CookieContainer cc)
        {
            item.Url = picurl;
            item.Referer = referer;
            item.Container = cc;
            item.ResultType = ResultType.Byte;
            return http.GetImg(http.GetHtml(item));
        }
        /// <summary>
        /// 针对于Base64形式的图片/验证码
        /// </summary>
        /// <param name="StrBaser64">信息</param>
        /// <returns>图像结果</returns>
        public Image GetBase64Image(string StrBaser64)
        {
            if (StrBaser64.Contains("base64,"))
            {
                StrBaser64 = StrBaser64.Substring(StrBaser64.IndexOf(',') + 1);
            }
            byte[] buffer = Convert.FromBase64String(StrBaser64);
            return http.byteArrayToImage(buffer);
        }
        /// <summary>
        /// 普通请求.直接返回标准结果
        /// </summary>
        /// <param name="posturl">post地址</param>
        /// <param name="referer">上一次请求地址</param>
        /// <param name="postdata">请求数据</param>
        /// <param name="IsAjax">是否需要异步标识</param>
        /// <param name="cc">当前Cookie</param>
        /// <param name="TimeOut">超时时间</param>
        /// <returns>返回数据结果</returns>
        public HttpResults PostHtml(string posturl, string referer, string postdata, bool IsAjax, CookieContainer cc, int TimeOut = 15000)
        {
            item.Url = posturl;
            item.Referer = referer;
            item.Method = "Post";
            item.IsAjax = IsAjax;
            item.ResultType = ResultType.String;
            item.Postdata = postdata;
            item.Container = cc;
            item.Timeout = TimeOut;
            return http.GetHtml(item);
        }


        /// <summary>
        /// 异步POST请求 通过回调返回结果
        /// </summary>
        /// <param name="objHttpItems">请求项</param>
        public async Task<HttpResults> PostHtmlAsync(HttpItems objHttpItems)
        {
            return await http.GetHtmlAsync(objHttpItems);
        }
        /// <summary>
        /// 异步GET请求 通过回调返回结果
        /// </summary>
        /// <param name="objHttpItems">请求项</param>
        public async Task<HttpResults> GetHtmlAsync(HttpItems objHttpItems)
        {
			return await http.GetHtmlAsync(objHttpItems);
		}


        #endregion
    }
	
}
