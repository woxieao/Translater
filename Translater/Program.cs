using System;
using System.Linq;
using System.Net.Http;
using System.Web;

namespace Translater
{
    class Program
    {
        private const string ApiUrl = "http://fanyi.baidu.com";
        private const string GetLanguageApi = "langdetect";
        private const string TranslateApi = "v2transapi";
        private static readonly HttpClient Client = new HttpClient
        {
            BaseAddress = new Uri(ApiUrl),
        };
        static void Main(string[] args)
        {
            string query;
            if (!args.Any())
            {
                query = Console.ReadLine() ?? "";
            }
            else
            {
                query = args.Aggregate(string.Empty, (current, arg) => current + $"{arg} ");
            }
            while (query.ToLower() != "e")
            {
                try
                {

                    var queryEncode = HttpUtility.UrlEncode(query);
                    var checkLanguage = GetLanguage(queryEncode);
                    if (checkLanguage.error != 0)
                    {
                        Console.WriteLine(checkLanguage.msg);
                    }
                    else
                    {
                        var result = Translate(queryEncode, checkLanguage.lan);
                        var meanResultList = result.trans_result.data;
                        foreach (var meanResult in meanResultList)
                        {
                            Console.WriteLine($"{meanResult.dst}");
                        }
                        try
                        {
                            var exampleResultList = result.dict_result.synthesize_means.symbols;

                            foreach (var exampleResult in exampleResultList)
                            {
                                foreach (var meanList in exampleResult.cys)
                                {
                                    foreach (var exampleList in meanList.means)
                                    {
                                        foreach (var example in exampleList.ljs)
                                        {

                                            Console.WriteLine($"{example.ls}\n{example.ly}");
                                        }
                                    }
                                }
                            }
                        }
                        catch
                        {
                            //do nothing
                        }
                        Console.WriteLine(checkLanguage.lan == "zh"
                            ? "翻译成功!(输入E以退出或继续输入以翻译)"
                            : "Translate Success!(Input E to exit Or type to translate)");
                    }
                }
                catch
                {
                    Console.WriteLine("Error");
                }
                finally
                {
                    query = Console.ReadLine() ?? "";
                }
            }
        }

        private static dynamic GetLanguage(string query)
        {
            var result = Client.GetAsync($"{GetLanguageApi}?query={query}").Result;
            return result.Content.ReadAsAsync<dynamic>().Result;
        }

        private static dynamic Translate(string query, dynamic from)
        {
            var to = from == "en" ? "zh" : "en";
            var result = Client.GetAsync($"{TranslateApi}?from={from}&to={to}&query={query}").Result;
            return result.Content.ReadAsAsync<dynamic>().Result;
        }
    }
}
