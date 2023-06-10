using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;
using System.Collections;
using System.Text.RegularExpressions;
using System.Threading;

namespace MyCrawler
{
    public class Crawler
    {
        private Hashtable urls = new Hashtable();

        private int count = 0;

        public void Start()
        {
            Console.WriteLine("请指定要爬取的页面URL：");
            String startUrl = Console.ReadLine();

            Console.WriteLine("请指定要爬取的页面数量：");
            int MaxPage = int.Parse(Console.ReadLine());

            count = 0;
            urls.Clear();

            urls.Add(startUrl, false);

            Console.WriteLine("开始爬取……");

            while (true)
            {
                String current = null;
                foreach (string url in urls.Keys)
                {
                    if ((bool)(urls[url])) continue;
                    current = url;

                }
                if (current == null || count >= MaxPage) break;

                Console.WriteLine($"正在爬取{current}……");

                String pageFile = Download(current);

                urls[current] = true;
                count++;

                int pos = current.LastIndexOf('/');
                String currentDirectory = current.Substring(0, pos); // 当前目录

                Parse(pageFile, currentDirectory);
            }
            Console.WriteLine("爬取结束");
        }
        private string Download(String url)
        {
            try
            {
                WebClient webClient = new WebClient();
                webClient.Encoding = Encoding.UTF8;
                String context = webClient.DownloadString(url);
                //Console.WriteLine(context);

                String fileName = count.ToString();
                File.WriteAllText(fileName, context, Encoding.UTF8);
                return context;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return "";
            }
        }
        private String TransPath(String url, String baseUrl) // 手动进行路径转换
        {
            if (url.Contains("://")) // 包含完整协议头，说明路径是绝对路径
            {
                return url;
            }
            else if (url.StartsWith("/")) // 本目录
            {
                if (baseUrl.EndsWith("/"))
                {
                    url = url.Substring(1);
                }
                return baseUrl + url;
            }
            else if (url.StartsWith("./")) // 也是本目录，递归处理
            {
                return TransPath(url.Substring(1), baseUrl);
            }
            else if (url.StartsWith("../")) // 父目录
            {
                int pos = baseUrl.LastIndexOf('/');
                return TransPath(url.Substring(3), baseUrl.Substring(0, pos)); // 令base返回到父目录递归处理
            }
            else // 说明直接以文件/目录名开头，递归调用上述方法即可
            {
                return TransPath("/" + url, baseUrl);
            }
        }
        private void Parse(String context, String baseUrl)
        {
            // 用于识别超链接的正则表达式，并指定了特定网页类型
            string strRef = @"(href|HREF)[]*=[]*[""'][^""'#>]+/[^""'#>]+(\.)(htm|html|aspx|php|jsp)[""']";
            //string strRef = @"(href|HREF)[]*=[]*[""'][^""'#>]+[""']";
            MatchCollection matches = new Regex(strRef).Matches(context);

            foreach (Match match in matches)
            {
                strRef = match.Value.Substring(match.Value.IndexOf('=') + 1)
                  .Trim('"', '\"', '#', '>');
                if (strRef.Length == 0) continue;

                strRef = TransPath(strRef, baseUrl); // 相对路径转绝对路径

                if (urls[strRef] == null) urls[strRef] = false;
            }

        }
        public static void Main()
        {
            Crawler myCrawler = new Crawler();
            myCrawler.Start();


        }
    }

}