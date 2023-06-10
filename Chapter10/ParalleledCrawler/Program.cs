using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;
using System.Collections;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace ParalleledCrawler
{
    public class Pair<T1, T2> // First用于存放网页文件，Second代表其所在目录，该类用于下载-解析的信息传递
    {
        public T1 First { get; set; }
        public T2 Second { get; set; }

        public Pair(T1 first, T2 second)
        {
            First = first;
            Second = second;
        }
    }
    public class ParalleledCrawler // 并行处理：url的下载和解析分为两个线程，遵循生产者-消费者模式
    {
        public string startUrl;

        public int count;

        public int maxPage;

        private BlockingCollection<string> urls =
            new BlockingCollection<string>(); // 待爬取的url队列
        private BlockingCollection<Pair<string, string>> pageFiles =
            new BlockingCollection<Pair<string, string>>(); // 待解析的网页文件队列

        private Hashtable vis = new Hashtable();

        private void ConsumeUrls() // 负责下载
        {
            // 当urls中无元素时，该foreach循环被阻塞，添加新元素后将自启动
            foreach (var url in urls.GetConsumingEnumerable()) 
            {
                if (!(vis[url] == null)) continue;
                vis[url] = true;

                Console.WriteLine($"正在爬取{url}……");

                String pageFile = Download(url);

                count++;

                if (count > maxPage)
                {
                    Console.WriteLine("爬取结束");
                    return;
                }  

                int pos = url.LastIndexOf('/');
                String currentDirectory = url.Substring(0, pos); // 当前目录

                pageFiles.Add(new Pair<string, string>(pageFile, currentDirectory));
                    
            }

        }
        private void ProduceUrls() // 负责解析
        {
            // 同理，新的文件被下载后将自启动
            foreach (var pagePair in pageFiles.GetConsumingEnumerable())
            {
                //Console.WriteLine("###");
                String context = pagePair.First;
                String baseUrl = pagePair.Second;

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
                    urls.Add(strRef); 
                }
            }
        }
        public void Start()
        {
            count = 0;
            
            urls.Add(startUrl);

            Console.WriteLine("开始爬取……");

            Task consumeTask = Task.Run(() => ConsumeUrls());
            Task produceTask = Task.Run(() => ProduceUrls());

            Task.WaitAll(consumeTask); // 等待Url Consumer收集足够的超链接后返回

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
        
        public static void Main()
        {
            ParalleledCrawler myCrawler = new ParalleledCrawler();

            Console.WriteLine("请指定要爬取的页面URL：");
            myCrawler.startUrl = Console.ReadLine();

            Console.WriteLine("请指定要爬取的页面数量：");
            myCrawler.maxPage = int.Parse(Console.ReadLine());

            myCrawler.Start();

        }
    }

}