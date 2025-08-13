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
                resp = await _http.GetAsync(url);
            }
            catch (Exception ex)
            {
                throw new HttpRequestException("Network error while requesting token: " + ex.Message, ex);
            }

            if (!resp.IsSuccessStatusCode)
            {
                var err = await resp.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Token endpoint returned {(int)resp.StatusCode}: {resp.ReasonPhrase}. {err}");
            }

            var content = await resp.Content.ReadAsStringAsync();
            return TryParseToken(content) ?? throw new Exception("Token could not be retrieved from the API.");
        }

        private string? TryParseToken(string content)
        {
            content = content.Trim().Trim('"');

            try
            {
                using var doc = JsonDocument.Parse(content);
                if (doc.RootElement.TryGetProperty("AccessToken", out var t))
                    return t.GetString();
                if (doc.RootElement.TryGetProperty("token", out var t2))
                    return t2.GetString();
            }
            catch
            {

            }

            return string.IsNullOrWhiteSpace(content) ? null : content;
        }

        public async Task<List<Suburb>> GetSuburbsAsync(string token, string search)
        {
            if (string.IsNullOrEmpty(token))
                throw new ArgumentException("Token is required");

            var url = $"{_baseUrl}GetRestSuburb?suburb={Uri.EscapeDataString(search)}";
            var req = new HttpRequestMessage(HttpMethod.Get, url);
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            HttpResponseMessage resp;
            try
            {
                resp = await _http.SendAsync(req);
            }
            catch (Exception ex)
            {
                throw new HttpRequestException("Network error while requesting suburbs: " + ex.Message, ex);
            }

            if (!resp.IsSuccessStatusCode)
            {
                var err = await resp.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Suburb endpoint returned {(int)resp.StatusCode}: {resp.ReasonPhrase}. {err}");
            }

            var content = await resp.Content.ReadAsStringAsync();
            try
            {
                var suburbs = JsonSerializer.Deserialize<List<Suburb>>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                return suburbs ?? new List<Suburb>();
            }
            catch
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

