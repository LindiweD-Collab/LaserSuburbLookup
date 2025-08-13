using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using LaserSuburbLookup.Models;

namespace LaserSuburbLookup.Services
{
    public class ApiClient : IDisposable
    {
        private readonly HttpClient _http;
        private readonly string _baseUrl;
        private bool _disposed;

        public ApiClient(string baseUrl)
        {
            _baseUrl = baseUrl.EndsWith("/") ? baseUrl : baseUrl + "/";
            _http = new HttpClient();
        }

        public async Task<string> GetTokenAsync(string username, string password)
        {
            var url = $"{_baseUrl}LaserToken?username={Uri.EscapeDataString(username)}&password={Uri.EscapeDataString(password)}";
            HttpResponseMessage resp;
            try
            {
                resp = await _http.GetAsync(url).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw new HttpRequestException("Network error while requesting token: " + ex.Message, ex);
            }

            if (!resp.IsSuccessStatusCode)
            {
                var err = await resp.Content.ReadAsStringAsync().ConfigureAwait(false);
                throw new HttpRequestException($"Token endpoint returned {(int)resp.StatusCode}: {resp.ReasonPhrase}. {err}");
            }

            var content = await resp.Content.ReadAsStringAsync().ConfigureAwait(false);

            try
            {
                using var doc = JsonDocument.Parse(content);
                if (doc.RootElement.TryGetProperty("token", out var t1))
                    return t1.GetString();
                if (doc.RootElement.TryGetProperty("Token", out var t2))
                    return t2.GetString();
            }
            catch
            {
             
            }

            return content.Trim();
        }

        public async Task<List<Suburb>> GetSuburbsAsync(string token, string search)
        {
            var url = $"{_baseUrl}GetRestSuburb?suburb={Uri.EscapeDataString(search)}";
            var req = new HttpRequestMessage(HttpMethod.Get, url);

            if (!string.IsNullOrEmpty(token))
            {
                req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            HttpResponseMessage resp;
            try
            {
                resp = await _http.SendAsync(req).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw new HttpRequestException("Network error while requesting suburbs: " + ex.Message, ex);
            }

            if (!resp.IsSuccessStatusCode)
            {
                var err = await resp.Content.ReadAsStringAsync().ConfigureAwait(false);
                throw new HttpRequestException($"Suburb endpoint returned {(int)resp.StatusCode}: {resp.ReasonPhrase}. {err}");
            }

            var content = await resp.Content.ReadAsStringAsync().ConfigureAwait(false);
            try
            {
                var suburbs = JsonSerializer.Deserialize<List<Suburb>>(content, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return suburbs ?? new List<Suburb>();
            }
            catch (Exception)
            {
                throw new Exception("Failed to parse suburb response.");
            }
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _http.Dispose();
                _disposed = true;
            }
        }
    }
}
