using System;
using System.Net;

namespace HttpCode.Core
{

    /// <summary>
    /// 请求结果类
    /// 请求结果均包含在此类中
    /// </summary>
    public class HttpResults
    {

        /// <summary>
        /// 响应结果的URL(可获取自动跳转后地址)   
        /// 如果获取跳转后地址失败,请使用RedirectUrl属性,
        /// 并设置HttpItems对象的Allowautoredirect =false;
        /// </summary>
        public string ResponseUrl { get; set; }
        /// <summary>
        /// 获取重定向的URL 
        /// 使用本属性时,请先关闭自动跳转属性  
        /// 设置方法如下:
        /// 设置HttpItems对象的Allowautoredirect =false;
        /// </summary>
        public string RedirectUrl
        {
            get
            {

                if (Header != null && Header.Count > 0)
                {
                    if (!string.IsNullOrEmpty(Header["location"]))
                    {
                        return Header["location"].ToString();
                    }
                    return string.Empty;
                }

                return string.Empty;
            }
        }

        CookieContainer _Container;
        /// <summary>
        /// 自动处理Cookie集合对象
        /// </summary>
        public CookieContainer Container
        {
            get { return _Container; }
            set { _Container = value; }
        }
        string _Cookie = string.Empty;
        /// <summary>
        /// Http请求返回的Cookie
        /// </summary>
        public string Cookie
        {
            get { return _Cookie; }
            set { _Cookie = value; }
        }
        string _RawCookie;
        /// <summary>
        /// 未清理/未合并时的 原始Cookie
        /// </summary>
        public string RawCookie
        {
            get { return _RawCookie; }
            set { _RawCookie = value; }
        }
        CookieCollection cookiecollection = new CookieCollection();
        /// <summary>
        /// Cookie对象集合
        /// </summary>
        public CookieCollection CookieCollection
        {
            get { return cookiecollection; }
            set { cookiecollection = value; }
        }
        private string html = string.Empty;
        /// <summary>
        /// 返回的String类型数据 只有ResultType.String时才返回数据，其它情况为空
        /// </summary>
        public string Html
        {
            get { return html; }
            set { html = value; }
        }
        private byte[] resultbyte = null;
        /// <summary>
        /// 返回的Byte数组 只有ResultType.Byte时才返回数据，其它情况为空
        /// </summary>
        public byte[] ResultByte
        {
            get { return resultbyte; }
            set { resultbyte = value; }
        }
        private WebHeaderCollection header = new WebHeaderCollection();
        /// <summary>
        /// 返回请求的头数据
        /// </summary>
        public WebHeaderCollection Header
        {
            get { return header; }
            set { header = value; }
        }
        /// <summary>
        /// Http请求后的状态码(详细描述)类型为枚举类型
        /// 如果需要使用数字型描述,请使用 StatusCodeNum
        /// </summary>
        public HttpStatusCode StatusCode;
        /// <summary>
        /// 状态码的数字形式
        /// </summary>
        public int StatusCodeNum
        {
            get { return Convert.ToInt32(StatusCode); }
        }
        private string _StatusDescription;
        /// <summary>
        /// 详细状态描述
        /// </summary>
        public string StatusDescription
        {
            get { return _StatusDescription; }
            set { _StatusDescription = value; }
        }
    }
}
