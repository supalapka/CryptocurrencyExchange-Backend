namespace Crawler.Contracts
{
    public record StartCrawl(string Filter);
    public record UrlMatched(string Title, string Url);
}
