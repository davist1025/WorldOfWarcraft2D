using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;

namespace WoW.Launcher
{
    public class HttpService
    {
        private static HttpClient _webClient = new HttpClient();

        private static string _apiRoot = "https://localhost:7125/api";

        public static async Task<string> GetVersionAsync()
        {
            using var response = await _webClient.GetAsync($"{_apiRoot}/version/");

            return await response.Content.ReadAsStringAsync();
        }
    }
}
