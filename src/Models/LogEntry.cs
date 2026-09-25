namespace SmartLogApi.Models;

public class LogEntry
{
    public int Id { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string ApplicationName { get; set; } = string.Empty;
    public string LogLevel { get; set; } = "Error"; // e.g., Warning, Error, Critical
    public string Message { get; set; } = string.Empty;
    public string? StackTrace { get; set; }
    
    // AI Enrichment Fields (We will populate these on Day 3)
    public bool IsAiAnalyzed { get; set; } = false;
    public string? AiDiagnosis { get; set; }      
    public string? AiSuggestedFix { get; set; }  
}
