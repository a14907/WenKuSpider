using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using Taobaoke;

namespace WpfApp
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 设置首页
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WebBrowser_Loaded(object sender, RoutedEventArgs e)
        {
            var b = sender as WebBrowser;
            if (b == null)
            {
                return;
            }
            //设置以_blank打开url时候，不弹出浏览器
            var webBrowserHelper = new WebBrowserHelper(b);
            webBrowserHelper.NewWindow += WebBrowserOnNewWindow;

            b.ObjectForScripting = new OprateBasic();

            SetWebBrowserSilent(b, true);

            b.Navigate("https://www.wenku8.net/book/2254.htm");
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
            var link = activeElement.ToString();
            // 这儿是在新窗口中打开，如果要在内部打开，改变当前browser的Source就行了
            browser.Source = new Uri(link);
            e.Cancel = true;
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
            var urls = GetDownloadUrls();
        }

        private IEnumerable<string> GetDownloadUrls()
        {
            var script = ";var str='';var als=document.getElementsByTagName('a');for(var i=0;i<als.length;i++){var item=als[i];if(item.innerText.indexOf('TXT简繁')>=0){str+=('|'+item.href);}}if(str.length!=0){ window.external.DownloadForTheWpf(str); } else { alert('未找到下载连接'); }; ";
            browser.InvokeScript("eval", script);
            return null;
        }

        #region 帮助类


        [System.Runtime.InteropServices.ComVisible(true)] // 将该类设置为com可访问
        public class OprateBasic
        {
            public void DownloadForTheWpf(string data)
            {
                var arr = data.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            }
        }

        #endregion
    }

}
