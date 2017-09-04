using System;
using System.Net;
using System.Text;

namespace HttpCode.Core
{
	/// <summary>
	///     Http请求参考类
	/// </summary>
	[Serializable]
	public class HttpItems
	{
		private string _accept =
				"image/gif, image/x-xbitmap, image/jpeg, image/pjpeg, application/x-shockwave-flash, application/vnd.ms-excel, application/vnd.ms-powerpoint, application/msword, */*"
			;

		private bool _autoRedirectMax;
		private string _cerPass;
		private string _cerPath = string.Empty;
		private CookieContainer _container;
		private string _contentType = "application/x-www-form-urlencoded";
		private string _cookie = string.Empty;

		private string _encoding = string.Empty;
		private string _method = "GET";
		private string _postdata;
		private byte[] _postdataByte;
		private PostDataType _postDataType = PostDataType.String;

		private Encoding _postEncoding = Encoding.Default;
		private int _readWriteTimeout = 15000;
		private string _referer = string.Empty;
		private int _timeout = 15000;
		private string _url;

		private string _userAgent =
			"Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/55.0.2883.9 Safari/537.36";

		private bool _useStringCookie = true;

		private bool _allowautoredirect = true;
		private int _connectionlimit = 1024;
		private CookieCollection _cookiecollection;

		/// <summary>
		///     默认的编码类型,如果不初始化,则为null,每次自动识别
		/// </summary>
		public Encoding Encoding = null;

		/// <summary>
		///     当该属性设置为 true 时，使用 POST 方法的客户端请求应该从服务器收到 100-Continue 响应，以指示客户端应该发送要发送的数据。此机制使客户端能够在服务器根据请求报头打算拒绝请求时，避免在网络上发送大量的数据
		/// </summary>
		private bool _expect100Continue;

		private WebHeaderCollection _header = new WebHeaderCollection();
		private bool _isAjax;
		private bool _isToLower;
		private string _proxyip = string.Empty;
		private string _proxypwd = string.Empty;
		private string _proxyusername = string.Empty;
		private ResultType _resulttype = ResultType.String;


		/// <summary>
		///     请求URL必须填写
		/// </summary>
		public string Url
		{
			get => _url;
			set => _url = value;
		}

		/// <summary>
		///     请求方式默认为GET方式
		/// </summary>
		public string Method
		{
			get => _method;
			set => _method = value;
		}

		/// <summary>
		///     默认请求超时时间
		/// </summary>
		public int Timeout
		{
			get => _timeout;
			set => _timeout = value;
		}

		/// <summary>
		///     默认写入Post数据超时间
		/// </summary>
		public int ReadWriteTimeout
		{
			get => _readWriteTimeout;
			set => _readWriteTimeout = value;
		}

		/// <summary>
		///     请求标头值 默认为image/gif, image/x-xbitmap, image/jpeg, image/pjpeg, application/x-shockwave-flash,
		///     application/vnd.ms-excel, application/vnd.ms-powerpoint, application/msword, */*
		/// </summary>
		public string Accept
		{
			get => _accept;
			set => _accept = value;
		}

		/// <summary>
		///     请求返回类型默认 application/x-www-form-urlencoded
		/// </summary>
		public string ContentType
		{
			get => _contentType;
			set => _contentType = value;
		}

		/// <summary>
		///     客户端访问信息默认Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/55.0.2883.9
		///     Safari/537.36
		/// </summary>
		public string UserAgent
		{
			get => _userAgent;
			set => _userAgent = value;
		}

		/// <summary>
		///     Post数据时的编码 默认为 Default,如需必要 请勿修改
		/// </summary>
		public Encoding PostEncoding
		{
			get => _postEncoding;
			set => _postEncoding = value;
		}

		/// <summary>
		///     返回数据编码默认为NUll,可以自动识别
		/// </summary>
		public string EncodingStr
		{
			get => _encoding;
			set => _encoding = value;
		}

		/// <summary>
		///     Post的数据类型
		/// </summary>
		public PostDataType PostDataType
		{
			get => _postDataType;
			set => _postDataType = value;
		}

		/// <summary>
		///     Post请求时要发送的字符串Post数据
		/// </summary>
		public string Postdata
		{
			get => _postdata;
			set => _postdata = value;
		}

		/// <summary>
		///     Post请求时要发送的Byte类型的Post数据
		/// </summary>
		public byte[] PostdataByte
		{
			get => _postdataByte;
			set => _postdataByte = value;
		}

		/// <summary>
		///     Cookie对象集合
		/// </summary>
		public CookieCollection CookieCollection
		{
			get => _cookiecollection;
			set => _cookiecollection = value;
		}

		/// <summary>
		///     自动处理cookie
		/// </summary>
		public CookieContainer Container
		{
			get => _container;
			set => _container = value;
		}

		/// <summary>
		///     请求时的Cookie
		/// </summary>
		public string Cookie
		{
			get => _cookie;
			set => _cookie = value;
		}

		/// <summary>
		///     来源地址，上次访问地址
		/// </summary>
		public string Referer
		{
			get => _referer;
			set => _referer = value;
		}

		/// <summary>
		///     证书绝对路径
		/// </summary>
		public string CerPath
		{
			get => _cerPath;
			set => _cerPath = value;
		}

		/// <summary>
		///     证书密码
		/// </summary>
		public string CerPass
		{
			get => _cerPass;
			set => _cerPass = value;
		}

		/// <summary>
		///     是否设置为全文小写
		/// </summary>
		public bool IsToLower
		{
			get => _isToLower;
			set => _isToLower = value;
		}

		/// <summary>
		///     是否增加异步请求头
		///     对应协议 : x-requested-with: XMLHttpRequest
		/// </summary>
		public bool IsAjax
		{
			get => _isAjax;
			set => _isAjax = value;
		}

		/// <summary>
		///     支持跳转页面，查询结果将是跳转后的页面
		/// </summary>
		public bool Allowautoredirect
		{
			get => _allowautoredirect;
			set => _allowautoredirect = value;
		}

		/// <summary>
		///     如果返回内容为 : 尝试自动重定向的次数太多 请设置本属性为true
		///     同时应注意设置超时时间(默认15s)
		/// </summary>
		public bool AutoRedirectMax
		{
			get => _autoRedirectMax;
			set => _autoRedirectMax = value;
		}

		/// <summary>
		///     当该属性设置为 true 时，使用 POST 方法的客户端请求应该从服务器收到 100-Continue 响应，以指示客户端应该发送要发送的数据。此机制使客户端能够在服务器根据请求报头打算拒绝请求时，避免在网络上发送大量的数据
		///     默认False
		/// </summary>
		public bool Expect100Continue
		{
			get => _expect100Continue;
			set => _expect100Continue = value;
		}

		/// <summary>
		///     最大连接数
		/// </summary>
		public int Connectionlimit
		{
			get => _connectionlimit;
			set => _connectionlimit = value;
		}

		/// <summary>
		///     代理Proxy 服务器用户名
		/// </summary>
		public string ProxyUserName
		{
			get => _proxyusername;
			set => _proxyusername = value;
		}

		/// <summary>
		///     代理 服务器密码
		/// </summary>
		public string ProxyPwd
		{
			get => _proxypwd;
			set => _proxypwd = value;
		}

		/// <summary>
		///     代理 服务IP
		/// </summary>
		public string ProxyIp
		{
			get => _proxyip;
			set => _proxyip = value;
		}

		/// <summary>
		///     设置返回类型String和Byte
		/// </summary>
		public ResultType ResultType
		{
			get => _resulttype;
			set => _resulttype = value;
		}

		/// <summary>
		///     头数据
		/// </summary>
		public WebHeaderCollection Header
		{
			get => _header;
			set => _header = value;
		}

		/// <summary>
		///     字符串头
		/// </summary>
		public string HeaderStr { get; set; }

		/// <summary>
		///     如果提示以下错误,请设置为true
		///     服务器提交了协议冲突. Section=ResponseHeader Detail=CR 后面必须是 LF
		/// </summary>
		public bool UseUnsafe { get; set; }

		/// <summary>
		///     如果提示以下错误,请设置为true
		///     从传输流收到意外的 EOF 或 0 个字节
		/// </summary>
		public bool IsEofError { get; set; }

		/// <summary>
		///     是否为So模式
		/// </summary>
		public bool IsSo { get; set; }

		/// <summary>
		///     文件上传路径
		/// </summary>
		public string UpLoadPath { get; set; }

		/// <summary>
		///     是否请求图片
		/// </summary>
		public bool IsGetImage { get; set; }

		/// <summary>
		///     是否使用字符串Cookie
		/// </summary>
		public bool UseStringCookie
		{
			get => _useStringCookie;
			set => _useStringCookie = value;
		}

		/// <summary>
		///     Https时的传输协议.
		///     替代原始SecurityProtocolType;
		///     包含四个版本(Ssl3 Tls Tls1.1 Tls1.2)
		/// </summary>
		public SecurityProtocolTypeEx SecProtocolTypeEx { get; set; }

		/// <summary>
		///     是否设置SecurityProtocolType 默认为false
		///     如果需要设置具体属性,请使用SecProtocolTypeEx枚举
		/// </summary>
		public bool IsSetSecurityProtocolType { get; set; }
	}

	/// <summary>
	///     Post的数据格式默认为string
	/// </summary>
	public enum PostDataType
	{
		/// <summary>
		///     字符串
		/// </summary>
		String, //字符串

		/// <summary>
		///     字节流
		/// </summary>
		Byte, //字符串和字节流

		/// <summary>
		///     文件路径
		/// </summary>
		FilePath //表示传入的是文件
	}

	/// <summary>
	///     返回类型
	/// </summary>
	public enum ResultType
	{
		/// <summary>
		///     表示只返回字符串
		/// </summary>
		String,

		/// <summary>
		///     表示只返回字节流
		/// </summary>
		Byte,

		/// <summary>
		///     急速请求,仅返回数据头
		/// </summary>
		So
	}

	/// <summary>
	///     Https时的传输协议.
	/// </summary>
	public enum SecurityProtocolTypeEx
	{
		// 摘要: 
		//     指定安全套接字层 (SSL) 3.0 安全协议。
		Ssl3 = 48,

		//
		// 摘要: 
		//     指定传输层安全 (TLS) 1.0 安全协议。
		Tls = 192,

		/// <summary>
		///     指定传输层安全 (TLS) 1.1 安全协议。
		/// </summary>
		Tls11 = 768,

		/// <summary>
		///     指定传输层安全 (TLS) 1.2 安全协议。
		/// </summary>
		Tls12 = 3072
	}
}