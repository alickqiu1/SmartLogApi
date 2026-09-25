using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using SmartLogApi.Models;

namespace SmartLogApi.Services;

public class AiLogService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public AiLogService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        // This safely pulls your key from the local Secret Manager at runtime
        _apiKey = configuration["Groq:ApiKey"] ?? throw new ArgumentNullException("Groq API Key is missing!");
    }

    public async Task<AiResponse?> AnalyzeLogAsync(string errorMessage, string? stackTrace)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "https://api.groq.com/openai/v1/chat/completions");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

        // We use a system prompt to force the AI to return clean, error-free JSON code matching our model
        var payload = new
        {
            model = "openai/gpt-oss-20b", // Fast, free, highly accurate coding model on Groq
            response_format = new { type = "json_object" }, // Forces native Groq JSON Mode
            messages = new[]
            {
                new { role = "system", content = "You are an expert developer tool. Analyze the provided C# error and stack trace. You MUST return a valid JSON object matching this schema exactly: { \"Diagnosis\": \"Plain English explanation here\", \"SuggestedFix\": \"Clean C# code or structural fix instructions here\" }" },
                new { role = "user", content = $"Error Message: {errorMessage}\nStack Trace: {stackTrace ?? "No stack trace provided."}" }
            },
            temperature = 0.2 // Lower values keep the output consistent and factual
        };

        request.Content = JsonContent.Create(payload);
        var response = await _httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode) return null;

        var jsonString = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(jsonString);
        
        // Extract the content string inside Groq's standard message response structure
        var content = doc.RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString();

        if (string.IsNullOrEmpty(content)) return null;

        // Parse the raw JSON text directly into our strongly typed C# AiResponse model
        return JsonSerializer.Deserialize<AiResponse>(content, new JsonSerializerOptions 
        { 
            PropertyNameCaseInsensitive = true 
        });
    }
}
