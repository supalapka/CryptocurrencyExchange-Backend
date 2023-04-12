using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;

namespace CryptocurrencyExchange.Controllers
{
    public class NewsController : Controller
    {

        public class News
        {
            public string Title { get; set; } = string.Empty;
            public string Body { get; set; } = string.Empty;
            public string Link { get; set; } = string.Empty;
            public string ImagePath { get; set; } = string.Empty;
            public DateTime Time { get; set; }

        }
        [HttpGet("/get-news")]
        public async Task<ActionResult<News>> GetNews()
        {
            var newsList = new List<News>();

            var url = "https://cryptonews.com/#news";

            var httpClient = new HttpClient();
            var html = await httpClient.GetStringAsync(url);

            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);

            var newsHeadlines = new List<string>();
            var newsHeadlineNodes = htmlDocument.DocumentNode.SelectNodes("//article[@class='mb-15 mb-sm-30 article-item']");

            foreach (var node in newsHeadlineNodes)
            {
                News news= new News();

                var imgNode = node.SelectSingleNode(".//div[@class='img-sized']/img");
                string image = imgNode.GetAttributeValue("src", "");

                var linkNode = node.SelectSingleNode(".//a[@class='article__title article__title--lg article__title--featured  mb-20']");
                string link = url + linkNode.GetAttributeValue("href", "");

                var bodyNode = node.SelectSingleNode(".//div[@class='mb-25 d-none d-md-block']");
                var body = bodyNode.InnerText;

                var timeNode = node.SelectSingleNode(".//div[@class='article__badge-date']");
                string time = timeNode.GetAttributeValue("data-utctime", "");
                DateTime dateTime = DateTime.Parse(time);

                string title = linkNode.InnerText;

                news.ImagePath = image;
                news.Link = link;
                news.Title = title;
                news.Body = body;
                news.Time = dateTime;
                newsHeadlines.Add(node.InnerHtml);
                newsList.Add(news);
            }

            return Ok(newsList);
        }
    }
}
