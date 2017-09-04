# HttpCode.Core
简单、易用、高效 一个有态度的开源.Net Http请求框架!

**HttpCode.Core**是**HttpCode**（https://github.com/msdn5/HttpCodeLib ） 的.net standard 2.0 实现，.net core、.net framework都可以使用，更改了异步方法的实现。

### 类库基本功能:
- 基础功能1:基于HttpWebRequest方式 同步/异步 提交数据 包含(Get/Post)
- 基础功能2:基于Wininet系统API方式提交数据 包含(Get/Post)
- 基础功能3:集合常用处理方法.处理页面结果/HTML数据更快捷

> HttpCode类库,让你感受一个简易到极致的HTTP编程. 
让编程更简易,代码更简洁...  
版权声明:  
本类库完全开源,只是基于微软官方提供的HttpWebRequest实现同步与异步的请求,Wininet是基于底层API进行封装; 
HttpCode Coding by **君临** 09-27/2014  
HttpCode.Core .net standard 2.0 implement by **stulzq**（xcmaster）09-01/2017

**API文档**: http://bbs.msdn5.com/forum.php?mod=forumdisplay&fid=37&filter=typeid&typeid=23  
> 这是HttpCode的文档，大部分和HttpCode.Core是一样的，可以参考，但是有一些地方有差异，主要是异步方法，然后移除了Wininet Api

### HttpCode.Core中的对象说明
- **HttpHelpers** 请求发起对象 请求过程与执行过程都在这个对象中.
- **HttpItems** 请求设置对象 请求时需要配置的参数都在这个对象中
- **HttpResults** 请求结果对象

#### 1. HttpHelpers 
```csharp
HttpHelpers http = new HttpHelpers(); //发起请求具体对象
http.AsyncGetHtml(); //异步模式获取请求
http.GetHtml(请求设置参数对象); //获取请求结果,参数返回HttpResults(请求结果对象)
http.GetImg(请求结果对象);//方法会根据请求结果返回一个Image对象
```

#### 2. HttpItems
```csharp
item.Accept;//请求标头值  如果没需要不用改动 默认为image/gif, image/x-xbitmap, image/jpeg, image/pjpeg, application/x-shockwave-flash, application/vnd.ms-excel, application/vnd.ms-powerpoint, application/msword, */* 

item.Allowautoredirect  //支持跳转页面，查询结果将是跳转后的页面 默认为true .如需获取跳转后的url请使用  HttpResults 中的属性: 

ResponseUrl  //获取响应结果的URL(可获取自动跳转后地址) ,如果获取跳转后地址失败,请使用RedirectUrl属性,并设置HttpItems对象的Allowautoredirect =false; 

RedirectUrl  //获取重定向的URL ;使用本属性时,请先关闭自动跳转属性;设置方法如下:设置HttpItems对象的Allowautoredirect =false; 

item.CerPath//证书绝对路径   如果模拟https时需要证书,则使用此属性附加证书. 详细使用 请参考 httpcode使用案例 如何使用httpcode访问https
//使用方法参考 http://bbs.msdn5.com/thread-555-1-1.html 

item.Connectionlimit //最大连接数  默认1024 无特殊需要不必修改 

item.Container //自动处理Cookie对象. 
//使用方法参考 http://bbs.msdn5.com/thread-552-1-1.html 

item.ContentType //请求返回类型默认 application/x-www-form-urlencoded  无特殊需要不必修改

item.Cookie //请求时的字符串类型Cookie
如果不使用自动维护cookie对象 每次使用cookie属性时请调用
new XJHTTP("上一次请求结果cookie","本次结果的cookie").UpdateCookie方法 更新cookie

item.CookieCollection //请求时的CookieCollection 对象,可以从请求结果(HttpResults)的CookieCollection 对象中得到.每次需要赋值.
 
item.Encoding  //返回数据编码默认为NUll,可以自动识别
 
item.Expect100Continue  //当该属性设置为 true 时，使用 POST 方法的客户端请求应该从服务器收到 100-Continue 响应，以指示客户端应该发送要发送的数据。此机制使客户端能够在服务器根据请求报头打算拒绝请求时，避免在网络上发送大量的数据 默认False

item.Header//请求时的header对象;
获取某一项头
item.Header["请求头名称"].toString(); 例如 item.Header["Location"].toString();  //获取跳转后的地址,如果location不存在则异常

//设置新的头部数据
item.Header.Add("请求头名称","请求头具体值"); // 例如 item.Header.Add("自定义头部数据","http://bbs.msdn5.com");

item.IsAjax //异步标志  默认为false 如果数据包头有 X-Requested-With:XMLHttpRequest 则设置IsAjax为true

item.IsToLower//是否设置为全文小写  默认false  无特殊需要不必修改

item.Method//请求方式 默认为GET方式 POST时需要修改为POST
//使用方法参考     http://bbs.msdn5.com/thread-551-1-1.html

item.Postdata//Post请求时要发送的字符串Post数据
//使用方法参考 http://bbs.msdn5.com/thread-551-1-1.html

item.PostdataByte //Post请求时要发送的Byte类型的Post数据  默认仅设置item.Postdata即可,需要上传包时设置本属性.
//使用方法参考 http://bbs.msdn5.com/thread-518-1-1.html 

item.PostDataType// Post的数据类型  默认为PostDataType.String;上传时需修改为Byte
//使用方法参考 http://bbs.msdn5.com/thread-518-1-1.html 

item.ProxyIp // 代理 服务IP

item.ProxyPwd //代理服务器密码

item.ProxyUserName //代理服务器用户名

item.ReadWriteTimeout//默认写入数据超时间 默认 30000 单位:毫秒  秒与毫秒的关系 1秒 = 1000毫秒

item.Referer //来源地址，上次访问地址  如非必要,不用填写

item.ResultType//设置返回类型String和Byte  默认String,请求图片或文件时选择Byte
//使用方法参考 http://bbs.msdn5.com/thread-553-1-1.html

item.Timeout //默认请求超时时间 默认3000 毫秒

item.URL //起请求的URL地址 必须填写

item.UserAgent //客户端访问信息默认Mozilla/5.0 (Windows NT 6.1; WOW64; rv:17.0) Gecko/20100101 Firefox/17.0 如非必要,不用修改
``` 

#### 3. HttpResults
```csharp
HttpResults hr = new HttpResults();  //具体对象请调用httphelper.gethtml方法获取的请求后的结果对象
 
hr.Container //自动处理cookie对象 无需自行维护
 
hr.Cookie//请求返回的字符串cookie  下次请求时需要自行维护
 
hr.CookieCollection //请求返回的CookieCollection   下次请求时需要自行维护
 
hr.Header // 请求后的header对象,使用方法同httpitems
 
hr.Html //请求后的结果  
 
hr.RedirectUrl //请求跳转后的地址 获取本属性时,请确认httpitems.Allowautoredirect 为false
 
hr.ResponseUrl//自动跳转后的url地址 获取本属性时,请确认httpitems.Allowautoredirect 为true 
 
 
hr.ResultByte //返回的Byte数组 只有ResultType.Byte时才返回数据，其它情况为空
//使用方法参考 http://bbs.msdn5.com/thread-553-1-1.html]http://bbs.msdn5.com/thread-553-1-1.html
 
hr.StatusCode //Http请求后的状态码
 
hr.StatusDescription //请求后的状态信息          
```

#### 使用HttpCode.Core发起Get请求
1. 方法一 使用HttpHelpers类获取结果
```csharp
string url = "bbs.msdn5.com";//请求地址
 
string res = string.Empty;//请求结果,请求类型不是图片时有效
 
System.Net.CookieContainer cc = new System.Net.CookieContainer();//自动处理Cookie对象
 
HttpHelpers helper = new HttpHelpers();//发起请求对象
 
HttpItems items = new HttpItems();//请求设置对象
 
HttpResults hr = new HttpResults();//请求结果
 
items.URL = url; //设置请求地址
 
items.Container = cc;//自动处理Cookie时,每次提交时对cc赋值即可
 
hr = helper.GetHtml(items);//发起请求
 
res = hr.Html;//得到请求结果
```
2. 方法二 使用XJHTTP类获取结果
```csharp
XJHTTP xjhttp = new XJHTTP();
 
CookieContainer cc = new CookieContainer();//自动处理cookie
 
string referer = "www.msdn5.com"; //referer头数据,如果没有请留空
 
string postdata = "这里是提交的数据"; //post提交数据
 
HttpResults hr = xjhttp.PostHtml(PostUrl, referer, postdata, false, cc);
 
参数说明:
xjhttp.PostHtml("提交的url", "Referer数据头","提交的数据", "Ajax标志不需要时请填写false", "自动处理Cookie对象");
```

#### 如何为httpcode 设置字符串Cookie与自动维护字符串Cookie
>自动维护就是指根据每次请求的Response返回的Cookie，来对本地的Cookie进行更新 

1. 手动维护Cookie
```csharp
HttpItems items = new HttpItems();//请求设置对象

HttpHelpers helper = new HttpHelpers();//发起请求对象

HttpResults hr = new HttpResults();//请求结果对象

string cookie = "";//字符串方式处理

items = new HttpItems(); //初始化请求对象,使其保持最新状态

items.Cookie = cookie;  //使用字符串方式

items.Url = "http://www.163.com/opt.php?do=login"; //请求URL地址

items.Referer = "http://mrtx.163.com/"; //请求的Referer

items.Method = "Post";//请求方式  Post

items.Postdata = "username=ceshiyixia1&password=111111";//提交参数

hr = helper.GetHtml(items); //正常模式得到请求结果

//手动处理字符串Cookie方式   每次都需要剔除返回的hr.Cookie,并且需要与原先的cookie合并为新的Cookie

string CookieStr = new XJHTTP().ClearCookie(hr.Cookie); //首先,剔除 服务器返回cookie中的无用项 

cookie = new XJHTTP().UpdateCookie(CookieStr, hr.Cookie);//接着手动合并两个Cookie ,组成新的cookie给字符串对象使用

//第二次请求
items = new HttpItems();

items.Url = "http://mrtx.u193.com/";

items.Referer = "http://mrtx.u193.com/";

items.Cookie = cookie;

hr = helper.GetHtml(items);
```
2. 自动维护字符串Cookie
```csharp
HttpItems items = new HttpItems();//请求设置对象
 
HttpHelpers helper = new HttpHelpers();//发起请求对象
 
HttpResults hr = new HttpResults();//请求结果对象
 
string cookie = "";//字符串方式处理
 
items = new HttpItems(); //初始化请求对象,使其保持最新状态
 
items.Cookie = cookie;  //使用字符串方式
 
items.URL = "http://www.163.com/opt.php?do=login"; //请求URL地址
 
items.Referer = "http://mrtx.163.com/"; //请求的Referer
 
items.Method = "Post";//请求方式  Post
 
items.Postdata = "username=ceshiyixia1&password=111111";//提交参数
 
#region 自动处理字符串Cookie方式(新)  
 
//使用ref将cookie传递进  GetHtml  重载方法    每次会   自动合并   上次  与  本次返回的cookie
 
hr = helper.GetHtml(items,ref cookie); 
 
#endregion
//多次请求
items = new HttpItems();
 
items.URL = "http://mrtx.u193.com/";
 
items.Referer = "http://mrtx.u193.com/";
 
items.Cookie = cookie;
 
//注意 如果有其他请求 请使用以下调用方式,保持cookie每次被自动合并
hr = helper.GetHtml(items,ref cookie);
```
3. 手动处理Cookie的几种常用方式：
```csharp
new XJHTTP().UpdateCookie("旧Cookie", "请求后的Cookie");//合并Cookie，将cookie2与cookie1合并更新 返回字符串类型Cookie
 
new XJHTTP().StringToCookie("网站Domain", "字符串Cookie内容");//将文字Cookie转换为CookieContainer 对象
 
new XJHTTP().CookieTostring("CookieContainer 对象");//将 CookieContainer 对象转换为字符串类型
 
new XJHTTP().GetAllCookie("CookieContainer 对象");//得到CookieContainer中的所有Cookie对象集合,返回List<Cookie>
 
new XJHTTP().GetCookieByWininet("网站Url");//从Wininet 中获取字符串Cookie 可获取IE与Webbrowser控件中的Cookie
 
new XJHTTP().GetAllCookieByHttpItems("请求设置对象");//从请求设置对象中获取Cookie集合,返回List<Cookie>
 
new XJHTTP().ClearCookie("需要处理的字符串Cookie");//清理string类型Cookie.剔除无用项返回结果为null时遇见错误.
 
new XJHTTP().SetIeCookie("设置Cookie的URL", "需要设置的Cookie");//可设置IE/Webbrowser控件Cookie
 
new XJHTTP().CleanAll();//清除IE/Webbrowser所有内容 (注意,调用本方法需要管理员权限运行) CleanHistory();清空历史记录  CleanCookie(); 清空Cookie CleanTempFiles(); 清空临时文件
```

更多介绍与示例，请看在线文档：http://bbs.msdn5.com/forum.php?mod=forumdisplay&fid=37&page=1&filter=typeid&typeid=23


