using Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using Taobaoke;

namespace WpfApp
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window, IMainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            _timer = new Timer(CallbackAsync, null, 1000, Timeout.Infinite);
        }


        private List<string> _urls = new List<string>();
        private string _cookie = "";
        private SynchronizationContext _context = SynchronizationContext.Current;
        private Timer _timer;

        private async void CallbackAsync(object obj)
        {
            if (_urls.Count != 0)
            {
                Log("开始任务");
                var urls = _urls[_urls.Count - 1];
                _urls.RemoveAt(_urls.Count - 1);
                if (urls.IndexOf('|') < 0)
                {
                    Log("_urls中的数据格式不正确，不包含字符：|");
                }
                else
                {
                    await DownloderHelper.Download(urls.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries), _cookie, Log);
                    Log("下载完成");
                }
                _timer = new Timer(CallbackAsync, null, 1000, Timeout.Infinite);
            }
            else
            {
                Log("任务url为空");
                _timer = new Timer(CallbackAsync, null, 10000, Timeout.Infinite);
            }
        }

        /// <summary>
        /// 设置首页
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WebBrowser_Loaded(object sender, RoutedEventArgs e)
        {
            if (!(sender is WebBrowser b))
            {
                return;
            }
            //设置以_blank打开url时候，不弹出浏览器
            var webBrowserHelper = new WebBrowserHelper(b);
            webBrowserHelper.NewWindow += WebBrowserOnNewWindow;

            b.ObjectForScripting = new OprateBasic(this);

            SetWebBrowserSilent(b, true);

            b.Navigate("https://www.wenku8.net/login.php?jumpurl=http%3A%2F%2Fwww.wenku8.net%2Findex.php");
        }

        /// <summary>
        /// 设置以_blank打开url时候，不弹出浏览器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WebBrowserOnNewWindow(object sender, CancelEventArgs e)
        {
            dynamic browser = sender;
            dynamic activeElement = browser.Document.activeElement;
            try
            {
                var link = activeElement.ToString();
                // 这儿是在新窗口中打开，如果要在内部打开，改变当前browser的Source就行了
                browser.Source = new Uri(link);
                e.Cancel = true;
            }
            catch (Exception ex)
            {
                if (activeElement.value== "轻小说搜索")
                {
                    e.Cancel = true;
                    string keywords = activeElement.parentElement.parentElement.children[1].children[0].value.ToString();
                    this.browser.Navigate($"https://www.wenku8.net/modules/article/search.php?searchtype=articlename&searchkey={keywords}");
                }
                else
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// 页面报错的时候不提示弹窗
        /// </summary>
        /// <param name="webBrowser"></param>
        /// <param name="silent"></param>
        private void SetWebBrowserSilent(WebBrowser webBrowser, bool silent)
        {
            FieldInfo fi = typeof(WebBrowser).GetField("_axIWebBrowser2", BindingFlags.Instance | BindingFlags.NonPublic);
            if (fi != null)
            {
                object browser = fi.GetValue(webBrowser);
                if (browser != null)
                    browser.GetType().InvokeMember("Silent", BindingFlags.SetProperty, null, browser, new object[] { silent });
            }
        }

        /// <summary>
        /// 下载逻辑
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            BeginGetDownloadUrls();
        }

        private void BeginGetDownloadUrls()
        {
            var script = ";window.external.SetCookiesVal(document.cookie);";
            browser.InvokeScript("eval", script);

            script = ";var str='';var als=document.getElementsByTagName('a');for(var i=0;i<als.length;i++){var item=als[i];if(item.innerText.indexOf('TXT简繁')>=0){str+=('|'+item.href);}}if(str.length!=0){ window.external.DownloadForTheWpf(str); } else { alert('未找到下载连接'); }; ";
            browser.InvokeScript("eval", script);
        }

        public void SetUrls(string us)
        {
            _urls.Add(us);
            Log($"获取url成功:{us}");
        }

        public void SetCookie(string cookie)
        {
            _cookie = cookie;
            Log("设置cookie成功");
        }
        public void Log(string msg)
        {
            _context.Post(p =>
            {
                txtLog.AppendText($"{DateTime.Now}:{msg}\r\n");
                txtLog.ScrollToEnd();
            }, null);
        }

        #region 帮助类


        [System.Runtime.InteropServices.ComVisible(true)] // 将该类设置为com可访问
        public class OprateBasic
        {
            private readonly IMainWindow _setUrls;

            public OprateBasic(IMainWindow setUrls)
            {
                this._setUrls = setUrls;
            }

            public void DownloadForTheWpf(string data)
            {
                _setUrls.SetUrls(data);
            }

            public void SetCookiesVal(string cookies)
            {
                _setUrls.SetCookie(cookies);
            }
        }

        #endregion

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            browser.GoBack();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            browser.GoForward();
        }
    }
}
