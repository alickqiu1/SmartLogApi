using Xunit;
using Microsoft.EntityFrameworkCore;
using SmartLogApi.Data;
using SmartLogApi.Models;

namespace SmartLogApi.Tests;

public class DatabaseIntegrationTests
{
    [Fact]
    public async Task Database_ShouldSuccessfullySaveAndRetrieveLogEntry()
    {
        // 1. Arrange: Configure an isolated, temporary database in-memory for this specific test run
        var options = new DbContextOptionsBuilder<LogContext>()
            .UseInMemoryDatabase(databaseName: "TestLogDatabase_" + Guid.NewGuid().ToString())
            .Options;

        // 2. Act: Insert a test log entry into our database context
        using (var context = new LogContext(options))
        {
            var testLog = new LogEntry
            {
                ApplicationName = "AuthService",
                Message = "User authorization timeout.",
                IsAiAnalyzed = true,
                AiDiagnosis = "Token verification server is unreachable.",
                AiSuggestedFix = "Check network connectivity rules."
            };

            context.Logs.Add(testLog);
            await context.SaveChangesAsync();
        }

        // 3. Assert: Open a fresh connection channel to verify the record saved and matches perfectly
        using (var context = new LogContext(options))
        {
            var savedLogs = await context.Logs.ToListAsync();
            
            Assert.Single(savedLogs); // Verifies exactly 1 log exists in the database
            var record = savedLogs[0];
            
            Assert.Equal("AuthService", record.ApplicationName);
            Assert.Equal("User authorization timeout.", record.Message);
            Assert.True(record.IsAiAnalyzed);
            Assert.Equal("Check network connectivity rules.", record.AiSuggestedFix);
        }
    }

    [Fact]
    public async Task Database_QueryingNonExistentLogId_ShouldReturnNullGracefully()
    {
        // 1. Arrange: Setup an isolated temporary test database environment
        var options = new DbContextOptionsBuilder<LogContext>()
            .UseInMemoryDatabase(databaseName: "TestLogDatabase_Failure_" + Guid.NewGuid().ToString())
            .Options;

        // 2. Act: Attempt to look up an ID that we never inserted (ID: 999)
        LogEntry? missingRecord;
        using (var context = new LogContext(options))
        {
            missingRecord = await context.Logs.FindAsync(999);
        }

        // 3. Assert: Verify the framework processes this as a clean, safe null rather than an application crash
        Assert.Null(missingRecord);
    }

    [Fact]
    public async Task Database_ConcurrentIngestion_ShouldAssignUniqueIncrementalIds()
    {
        // Edge Case 3: High-concurrency throughput
        // Arrange
        var options = new DbContextOptionsBuilder<LogContext>()
            .UseInMemoryDatabase(databaseName: "TestLogDatabase_Concurrency_" + Guid.NewGuid().ToString())
            .Options;

        // Act: Fire off multiple database tasks simultaneously to simulate real-world traffic spikes
        using (var context = new LogContext(options))
        {
            var task1 = context.Logs.AddAsync(new LogEntry { ApplicationName = "App1", Message = "Err1" }).AsTask();
            var task2 = context.Logs.AddAsync(new LogEntry { ApplicationName = "App2", Message = "Err2" }).AsTask();
            var task3 = context.Logs.AddAsync(new LogEntry { ApplicationName = "App3", Message = "Err3" }).AsTask();

            await Task.WhenAll(task1, task2, task3);
            await context.SaveChangesAsync();
        }

        // Assert: Open a connection to verify all entries were saved uniquely without colliding IDs
        using (var context = new LogContext(options))
        {
            var totalRecords = await context.Logs.ToListAsync();
            Assert.Equal(3, totalRecords.Count);
            
            // Verify IDs are completely unique and not duplicated
            var uniqueIds = totalRecords.Select(l => l.Id).Distinct().Count();
            Assert.Equal(3, uniqueIds);
        }
    }
    
}
