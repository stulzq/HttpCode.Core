using System;
using Xunit;

namespace HttpCode.Core.Test
{
    public class TestRequest
    {
        [Fact]
        public async void TestGetRequestAsync()
        {
	        string url = "bbs.msdn5.com";//请求地址

	        string res = string.Empty;//请求结果,请求类型不是图片时有效

	        System.Net.CookieContainer cc = new System.Net.CookieContainer();//自动处理Cookie对象

	        HttpHelpers helper = new HttpHelpers();//发起请求对象

	        HttpItems items = new HttpItems();//请求设置对象

	        HttpResults hr = new HttpResults();//请求结果

	        items.Url = url; //设置请求地址

	        items.Container = cc;//自动处理Cookie时,每次提交时对cc赋值即可

	        hr = await helper.GetHtmlAsync(items);//发起异步请求

	        res = hr.Html;//得到请求结果
		}
    }
}
