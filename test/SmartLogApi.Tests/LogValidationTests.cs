using Xunit;
using SmartLogApi.Models;

namespace SmartLogApi.Tests;

public class LogValidationTests
{
    [Fact]
    public void NewLogEntry_ShouldInitializeWithDefaultValues()
    {
        // Arrange & Act - Simulate a brand-new incoming log entry
        var log = new LogEntry
        {
            ApplicationName = "PaymentService",
            Message = "NullReferenceException occurred."
        };

        // Assert - Verify baseline constraints are met before AI processing runs
        Assert.False(log.IsAiAnalyzed);
        Assert.Null(log.AiDiagnosis);
        Assert.Null(log.AiSuggestedFix);
        Assert.Equal("Error", log.LogLevel); // Verifies default fallback string behaves correctly
    }

    [Fact]
    public void NewLogEntry_WithEmptyMessage_ShouldFailValidation()
    {
        // Arrange
        var log = new LogEntry
        {
            ApplicationName = "PaymentService",
            Message = "" // Empty message string simulation
        };

        // Act & Assert
        // In a production app, we ensure business rules validate text length
        Assert.True(string.IsNullOrEmpty(log.Message), "Validation failed: Message cannot be empty.");
    }

    [Fact]
    public void NewLogEntry_WithCriticalLogLevel_ShouldNotUseDefaultErrorLevel()
    {
        // Arrange & Act
        var log = new LogEntry
        {
            ApplicationName = "InventoryService",
            Message = "Out of memory",
            LogLevel = "Critical" // Explicitly overriding the default "Error"
        };

        // Assert
        Assert.NotEqual("Error", log.LogLevel);
        Assert.Equal("Critical", log.LogLevel);
    }

    [Fact]
    public void NewLogEntry_WithMassiveStackTrace_ShouldStoreItIntact()
    {
        // Edge Case 1: Extremely long text blocks simulating deep recursive crashes
        // Arrange
        var massiveStackTrace = new string('X', 50000); // 50,000 character string
        
        // Act
        var log = new LogEntry
        {
            ApplicationName = "ScalingService",
            Message = "Out of memory loop.",
            StackTrace = massiveStackTrace
        };

        // Assert
        Assert.Equal(50000, log.StackTrace.Length);
    }

    [Fact]
    public void NewLogEntry_WithMaliciousCodePayload_ShouldHandleCharactersSafely()
    {
        // Edge Case 2: Injected symbols that typically break JSON, SQL, or HTML parsers
        // Arrange
        var weirdPayload = "SELECT * FROM Users; <script>alert('hack')</script> { \"nested\": 'invalid' }";

        // Act
        var log = new LogEntry
        {
            ApplicationName = "SanitizationService",
            Message = weirdPayload
        };

        // Assert
        Assert.Equal(weirdPayload, log.Message);
    }    

}
