using Bhav.Application.IRepositories;
using Bhav.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace Bhav.Infrastructure.Repositories
{
    public class BhavCopyRepository : IBhavCopyRepository
    {
        private readonly BhavDbContext _context;
        private readonly IHttpClientFactory _httpClientFactory;

        public BhavCopyRepository(BhavDbContext context, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
        }


        public async Task<List<string>> GetSymbolsByDateAsync(DateOnly date, CancellationToken cancellationToken = default)
        {
            return await _context.BhavCopies
                .Where(x => x.TradeDate == date)
                .Select(x => x.Symbol!)
                .ToListAsync(cancellationToken);
        }


        public async Task AddRangeAsync(List<BhavCopy> data)
        {
            await _context.BhavCopies.AddRangeAsync(data);
            await _context.SaveChangesAsync();
        }


        public async Task<List<BhavCopy>> GetByDateAsync(DateOnly date)
        {
            return await _context.BhavCopies
                .Where(x => x.TradeDate == date)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<byte[]?> DownloadAsync(string url)
        {
            var cookieContainer = new CookieContainer();

            using var handler = new HttpClientHandler
            {
                CookieContainer = cookieContainer,
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
                UseCookies = true,
                AllowAutoRedirect = true
            };

            using var client = new HttpClient(handler)
            {
                Timeout = TimeSpan.FromSeconds(30)
            };

            client.DefaultRequestHeaders.Add("User-Agent",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/124.0.0.0 Safari/537.36");
            client.DefaultRequestHeaders.Add("Accept",
                "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
            client.DefaultRequestHeaders.Add("Accept-Language", "en-US,en;q=0.9");
            client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");
            client.DefaultRequestHeaders.Add("Connection", "keep-alive");
            client.DefaultRequestHeaders.Add("Cache-Control", "no-cache");

            Console.WriteLine($"Downloading URL: {url}");

            try
            {
                // NSE requires session cookies + referer
                if (url.Contains("nseindia.com"))
                {
                    var homeResponse = await client.GetAsync("https://www.nseindia.com/");
                    Console.WriteLine($"NSE home status: {homeResponse.StatusCode}");

                    await Task.Delay(1000);

                    client.DefaultRequestHeaders.Referrer =
                        new Uri("https://www.nseindia.com/all-reports");
                }

               
                if (url.Contains("bseindia.com"))
                {
                    client.DefaultRequestHeaders.Referrer =
                        new Uri("https://www.bseindia.com/markets/MarketInfo/BhavCopy.aspx");
                }

                var response = await client.GetAsync(url);

                Console.WriteLine($"Response: {response.StatusCode} for {url}");

              
                if (response.StatusCode == HttpStatusCode.NotFound ||
                    response.StatusCode == HttpStatusCode.Forbidden ||
                    response.StatusCode == HttpStatusCode.Unauthorized ||
                    response.StatusCode == (HttpStatusCode)429)
                {
                    Console.WriteLine($"Skipping ({response.StatusCode}): {url}");
                    return null;
                }

                response.EnsureSuccessStatusCode();

                return await response.Content.ReadAsByteArrayAsync();
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"HTTP error for {url}: {ex.Message}");
                return null;
            }
            catch (TaskCanceledException)
            {
                Console.WriteLine($"Timeout for {url}");
                return null;
            }
        }
    }
}
