using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
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

            var webBrowserHelper = new WebBrowserHelper(browser);
            webBrowserHelper.NewWindow += WebBrowserOnNewWindow; 
        }

        private void WebBrowserOnNewWindow(object sender, CancelEventArgs e)
        {
            dynamic browser = sender;
            dynamic activeElement = browser.Document.activeElement;
            var link = activeElement.ToString();
            // 这儿是在新窗口中打开，如果要在内部打开，改变当前browser的Source就行了
            browser.Source = new Uri(link);
            e.Cancel = true;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void WebBrowser_Loaded(object sender, RoutedEventArgs e)
        {
            var b = sender as WebBrowser;
            if (b == null)
            {
                return;
            }
            SetWebBrowserSilent(b, true);
            b.Navigate("https://www.wenku8.net/index.php"); 
        }
         

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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var url = browser.Source;
        }
    }
}
