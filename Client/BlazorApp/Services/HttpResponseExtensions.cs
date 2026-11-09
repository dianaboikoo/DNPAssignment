using System.Text.Json;

namespace BlazorApp.Services;

public static class HttpResponseExtensions
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public static async Task<T> ReadAsAsync<T>(this HttpResponseMessage response)
    {
        string content = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception(ErrorMessage(response, content));
        }

        return JsonSerializer.Deserialize<T>(content, JsonOptions)!;
    }

    public static async Task EnsureSuccessAsync(this HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
        {
            string content = await response.Content.ReadAsStringAsync();
            throw new Exception(ErrorMessage(response, content));
        }
    }

    private static string ErrorMessage(HttpResponseMessage response, string content)
    {
        return string.IsNullOrWhiteSpace(content)
            ? $"Request failed with status {(int)response.StatusCode}"
            : content;
    }
}