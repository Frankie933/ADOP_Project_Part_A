//#define UseNewsApiSample  // Remove or undefine to use your own code to read live data

using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json; 
using System.Threading.Tasks;
using System.Collections.Generic;

using Assignment_A2_01.Models;
using Assignment_A2_01.ModelsSampleData;

namespace Assignment_A2_01.Services
{
    public class NewsService
    {
        HttpClient httpClient = new HttpClient();
 
        // Your API Key
        readonly string apiKey = "";

        public NewsService()
        {
            httpClient = new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate });
            httpClient.DefaultRequestHeaders.Add("user-agent", "News-API-csharp/0.1");
            httpClient.DefaultRequestHeaders.Add("x-api-key", apiKey);
        }

        public async Task<NewsApiData> GetNewsAsync()
        {

#if UseNewsApiSample
            NewsApiData nd = await NewsApiSampleData.GetNewsApiSampleAsync("sports");

#else
            var uri = $"https://newsapi.org/v2/top-headlines?country=se&category=sports"; 

            // make the http request
            var httpRequest = new HttpRequestMessage(HttpMethod.Get, uri);
            var response = await httpClient.SendAsync(httpRequest);
            response.EnsureSuccessStatusCode();

            //Convert Json to Object
            NewsApiData nd = await response.Content.ReadFromJsonAsync<NewsApiData>();



#endif            
            return nd;
        }
    }
}
