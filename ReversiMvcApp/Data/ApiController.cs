using Microsoft.AspNetCore.Mvc;

namespace ReversiMvcApp.Data
{
    public class ApiController : Controller
    {
        private string requestUri = "http://localhost:5001/";
        private string apiUri = "api/";
        private HttpClient client;

        public ApiController()
        {
            client = new HttpClient
            {
                BaseAddress = new Uri(requestUri)
            };
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<T?> GetAsync<T>(string path)
        {
            var response = await client.GetAsync(apiUri + path);

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException(await response.Content.ReadAsStringAsync());
            }

            return await response.Content.ReadFromJsonAsync<T>();
        }

        public async Task<IEnumerable<T>?> GetListAsync<T>(string path)
        {
            var response = await client.GetAsync(apiUri + path);

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException(await response.Content.ReadAsStringAsync());
            }

            return await response.Content.ReadFromJsonAsync<IEnumerable<T>>();
        }

        public async Task<T?> PutAsync<T>(string path, T body)
        {
            var response = await client.PutAsJsonAsync(apiUri + path, body);

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException(await response.Content.ReadAsStringAsync());
            }

            return await response.Content.ReadFromJsonAsync<T>();
        }

        public async Task<T?> PostAsync<T>(string path, T body)
        {
            var response = await client.PostAsJsonAsync(apiUri + path, body);

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException(await response.Content.ReadAsStringAsync());
            }

            return await response.Content.ReadFromJsonAsync<T>();
        }
    }
}