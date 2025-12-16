using Newtonsoft.Json;
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

        /// <summary>
        /// Perform client logon, similar to how the AuthServer handles logon.
        /// </summary>
        /// <param name="accountName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static async Task<string> Login(string accountName, string password)
        {
            // todo: hash password according to wow2d standards.

            var content = new StringContent(JsonConvert.SerializeObject(
                new Dictionary<string, string>()
                {
                    { "accountName", $"{accountName}" },
                    { "hashedPw", $"{password}" }
                }, Formatting.Indented), Encoding.UTF8, "application/json");

            using var response = await _webClient.PostAsync($"{_apiRoot}/auth", content);
            return await response.Content.ReadAsStringAsync();
        }
    }
}
