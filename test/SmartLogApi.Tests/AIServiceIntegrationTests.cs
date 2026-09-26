using Xunit;
using Microsoft.Extensions.Configuration;
using SmartLogApi.Services;

namespace SmartLogApi.Tests;

public class AiServiceIntegrationTests
{
    [Fact(Skip = "Manual verification test only. Remove this attribute to test live cloud connectivity.")]
    public async Task AnalyzeLogAsync_WithValidCredentials_ShouldReturnPopulatedAiResponse()
    {
        // 1. Arrange: Build a live local configuration provider to safely pull your real user secret key
        var configuration = new ConfigurationBuilder()
            .AddUserSecrets<AiServiceIntegrationTests>() // Securely reads your stored local Groq key
            .Build();

        var httpClient = new HttpClient();
        var service = new AiLogService(httpClient, configuration);

        var testMessage = "NullReferenceException in processing pipeline.";
        var testStackTrace = "at SmartLogApi.Controllers.LogsController.SubmitLog";

        // 2. Act: Physically call the Groq cloud servers
        var result = await service.AnalyzeLogAsync(testMessage, testStackTrace);

        // 3. Assert: Verify the live cloud model understands the error and formats the response correctly
        Assert.NotNull(result);
        Assert.False(string.IsNullOrWhiteSpace(result.Diagnosis));
        Assert.False(string.IsNullOrWhiteSpace(result.SuggestedFix));
    }
}
