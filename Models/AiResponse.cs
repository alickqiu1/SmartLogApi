namespace SmartLogApi.Models;

public class AiResponse
{
    public string Diagnosis { get; set; } = string.Empty;
    public string SuggestedFix { get; set; } = string.Empty;
}
