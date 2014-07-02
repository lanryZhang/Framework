using System;
using System.Collections.Generic;
using System.Web;
using System.Net;
using System.IO;
using System.Text;

namespace Ifeng.Utility.Helper
{
    /// <summary>
    ///ServiceRequest 的摘要说明
    /// </summary>
    public class WebRequestHelper
    {
        private HttpWebRequest request = null;
        private List<KeyValuePair<string, object>> param = null;
        private int timeOut = 10000;
        private string url = string.Empty;
        private Uri uri = null;


        public int TimeOut
        {
            get { return timeOut; }
            set { timeOut = value; }
        }

        public CookieContainer Cookie
        {
            get { return this.request.CookieContainer; }
            set { this.request.CookieContainer = value; }
        }

        public HttpWebRequest RequestInstance
        {
            get { return request; }
            private set { request = value; }
        }

        public WebRequestHelper(string url)
        {
            this.url = url;
            this.uri = new Uri(url);
        }

        public WebRequestHelper(string url, List<KeyValuePair<string, object>> param)
        {
            this.url = url;
            this.uri = new Uri(url);
            this.param = param;
        }

        public string Request(string method, Action<HttpStatusCode, string> onComplete)
        {
            var res = string.Empty;

            switch (method.ToLower())
            {
                case "post":
                    res = Post(onComplete);
                    break;
                case "get":
                    res = Get(onComplete);
                    break;
            }
            return res;
        }

        /// <summary>
        /// 初始化参数
        /// </summary>
        /// <returns></returns>
        private string InitParam()
        {
            var strBuilder = new StringBuilder();

            if (param != null && param.Count > 0)
            {
                foreach (var item in param)
                {
                    strBuilder.AppendFormat("{0}={1}&", item.Key, item.Value);
                }
            }

            return strBuilder.ToString().TrimEnd('&');
        }

        /// <summary>
        /// 构造Post请求
        /// </summary>
        /// <param name="onComplete">请求结束后执行的方法</param>
        /// <returns></returns>
        private string Post(Action<HttpStatusCode, string> onComplete)
        {
            var paramStr = InitParam();
            byte[] data = Encoding.UTF8.GetBytes(paramStr);
            
            this.request = (HttpWebRequest)HttpWebRequest.Create(uri);
            this.request.ContentLength = data.Length;

            this.request.Timeout = timeOut;
            this.request.KeepAlive = true;
            this.request.Method = "POST";
            this.request.ContentType = "application/x-www-form-urlencoded";

            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            return OnRequest(onComplete);
        }

        /// <summary>
        /// 构造Get请求
        /// </summary>
        /// <param name="onComplete">请求结束后执行的方法</param>
        /// <returns></returns>
        private string Get(Action<HttpStatusCode, string> onComplete)
        {
            var paramStr = InitParam();
            if (!string.IsNullOrEmpty(paramStr))
            {
                this.uri = new Uri(this.url + "?" + paramStr);
            }
            this.request = (HttpWebRequest)HttpWebRequest.Create(uri);

            this.request.Timeout = timeOut;
            this.request.KeepAlive = true;
            this.request.Method = "GET";

            return OnRequest(onComplete);
        }

        /// <summary>
        /// 发起请求
        /// </summary>
        /// <param name="method">请求方式</param>
        /// <param name="onComplete">请求结束后执行的方法</param>
        /// <returns></returns>
        private string OnRequest(Action<HttpStatusCode, string> onComplete)
        {
            if (uri == null)
            {
                return string.Empty;
            }


            HttpWebResponse response = null;
            string res = string.Empty;
            try
            {
                response = request.GetResponse() as HttpWebResponse;
            }
            catch (WebException err)
            {
                throw err;
            }

            if (response == null && onComplete != null)
            {
                onComplete(HttpStatusCode.NotFound, "请求失败.");
            }

            if (response != null)
            {
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    res = reader.ReadToEnd();
                }
                if (onComplete != null)
                {
                    onComplete(response.StatusCode, res);
                }
            }
            return res;
        }
    }
}
