using HtmlAgilityPack;
using System.IO;
using System.Net.Http;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System;

namespace Helper
{
    public class DownloderHelper
    {
        public static async Task Download(string[] urls, string cookies, Action<string> log)
        {
            if (urls.Length != 2)
            {
                log("参数urls长度不是2");
                return;
            }
            else
            {
                log("参数校验通过，开始下载");
                await PrivateDownloadAsync(urls, cookies, log);
            }
        }

        private static async Task PrivateDownloadAsync(string[] urls, string cookies, Action<string> log)
        {
            var dividePage = urls[0];
            var complatePage = urls[1];
            //下载简繁分卷
            await DownloadDivideVersionAsync(dividePage, cookies, log);
            //下载简繁全本
            await DownloadCompleteVersionAsync(complatePage, cookies, log);
        }

        public static async Task DownloadCompleteVersionAsync(string complatePage, string cookies, Action<string> log)
        {
            var handler = new HttpClientHandler { UseCookies = false };
            HttpClient h = new HttpClient(handler);
            h.DefaultRequestHeaders.Add("Cookie", cookies);

            var request = new HttpRequestMessage(HttpMethod.Get, complatePage);
            var content = (await h.SendAsync(request)).Content;
            var htmlStream = await content.ReadAsStreamAsync();
            string htmlStr = await htmlStream.ReadAsGB2312Async((int)content.Headers.ContentLength.Value);
            var doc = new HtmlDocument();
            doc.LoadHtml(htmlStr);

            //获取标题
            var dirTitle = doc.DocumentNode.SelectSingleNode("//*[@id='content']/table/caption/a").InnerText;
            if (!Directory.Exists($"data/{dirTitle}/全本/简体"))
            {
                Directory.CreateDirectory($"data/{dirTitle}/全本/简体");
            }
            if (!Directory.Exists($"data/{dirTitle}/全本/繁体"))
            {
                Directory.CreateDirectory($"data/{dirTitle}/全本/繁体");
            }
            var table = doc.DocumentNode.SelectSingleNode("//*[@id='content']/table");
            //获取简体的下载链接
            var lsSimple = table.SelectNodes("tr")
                .Skip(1)//跳过第一个
                .Select(m => m.SelectNodes("td").Last().SelectNodes("a").Skip(3).First().GetAttributeValue("href", "")).ToList();
            //下载简体
            if (lsSimple.Count == 1)
            {
                if (File.Exists($"data/{dirTitle}/全本/简体/{dirTitle}.txt"))
                {
                    log("简体全本已存在");
                }
                else
                {
                    var item = lsSimple[0];
                    var response = await h.GetAsync(item);
                    await (await response.Content.ReadAsStreamAsync()).SaveAsFile($"data/{dirTitle}/全本/简体/{dirTitle}.txt", (int)response.Content.Headers.ContentLength.Value);
                    log("简体全本下载成功");
                }
            }
            else
            {
                log("全本下载的数量不等于1");
            }
            //获取繁体的下载链接
            var lsTraditional = table.SelectNodes("tr")
                .Skip(1)//跳过第一个
                .Select(m => m.SelectNodes("td").Last().SelectNodes("a").Skip(5).First().GetAttributeValue("href", "")).ToList();
            if (lsTraditional.Count != 1)
            {
                if (File.Exists($"data/{dirTitle}/全本/繁体/{dirTitle}.txt"))
                {
                    log("繁体全本已存在");
                }
                else
                {
                    var item = lsTraditional[0];
                    var response = await h.GetAsync(item);
                    await (await response.Content.ReadAsStreamAsync()).SaveAsFile($"data/{dirTitle}/全本/繁体/{dirTitle}.txt", (int)response.Content.Headers.ContentLength.Value);
                    log("繁体全本下载成功");
                }
            }
            else
            {
                log("全本下载的数量不等于1");
            }
        }

        public static async Task DownloadDivideVersionAsync(string dividePage, string cookies, Action<string> log)
        {
            var handler = new HttpClientHandler { UseCookies = false };
            HttpClient h = new HttpClient(handler);
            h.DefaultRequestHeaders.Add("Cookie", cookies);

            var request = new HttpRequestMessage(HttpMethod.Get, dividePage);
            var content = (await h.SendAsync(request)).Content;
            var htmlStream = await content.ReadAsStreamAsync();
            string htmlStr = await htmlStream.ReadAsGB2312Async((int)content.Headers.ContentLength.Value);
            var doc = new HtmlDocument();
            doc.LoadHtml(htmlStr);

            //获取标题
            var dirTitle = doc.DocumentNode.SelectSingleNode("//*[@id='content']/table/caption/a").InnerText;
            if (!Directory.Exists($"data/{dirTitle}/分卷/简体"))
            {
                Directory.CreateDirectory($"data/{dirTitle}/分卷/简体");
            }
            if (!Directory.Exists($"data/{dirTitle}/分卷/繁体"))
            {
                Directory.CreateDirectory($"data/{dirTitle}/分卷/繁体");
            }
            var table = doc.DocumentNode.SelectSingleNode("//*[@id='content']/table");
            //获取简体的下载链接
            var lsSimple = table.SelectNodes("tr")
                .Skip(1)//跳过第一个
                .Select(m => m.SelectNodes("td").Last().SelectNodes("a").Skip(1).First().GetAttributeValue("href", "")).ToList();
            //下载简体
            for (int i = 0; i < lsSimple.Count(); i++)
            {
                if (File.Exists($"data/{dirTitle}/分卷/简体/第{i + 1}卷.txt"))
                {
                    log($"简体第{i + 1}卷已存在");
                    continue;
                }
                var item = lsSimple[i];
                var response = await h.GetAsync(item);
                await (await response.Content.ReadAsStreamAsync()).SaveAsFile($"data/{dirTitle}/分卷/简体/第{i + 1}卷.txt", (int)response.Content.Headers.ContentLength.Value);
                log($"简体第{i+1}卷下载成功");
            }
            //获取繁体的下载链接
            var lsTraditional = table.SelectNodes("tr")
                .Skip(1)//跳过第一个
                .Select(m => m.SelectNodes("td").Last().SelectNodes("a").Skip(2).First().GetAttributeValue("href", "")).ToList();
            for (int i = 0; i < lsTraditional.Count(); i++)
            {
                if (File.Exists($"data/{dirTitle}/分卷/繁体/第{i + 1}卷.txt"))
                {
                    log($"繁体第{i + 1}卷已存在");
                    continue;
                }
                var item = lsTraditional[i];
                var response = await h.GetAsync(item);
                await (await response.Content.ReadAsStreamAsync()).SaveAsFile($"data/{dirTitle}/分卷/繁体/第{i + 1}卷.txt", (int)response.Content.Headers.ContentLength.Value);
                log($"繁体第{i + 1}卷下载成功");
            }
        }
    }
}
