using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartLogApi.Data;
using SmartLogApi.Models;
using SmartLogApi.Services;

namespace SmartLogApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LogsController : ControllerBase
{
    private readonly LogContext _context;
    private readonly AiLogService _aiService; // Add this line

    public LogsController(LogContext context, AiLogService aiService) // Update this line
    {
        _context = context;
        _aiService = aiService; // Add this line
    }

    // POST: api/logs
    [HttpPost]
    public async Task<ActionResult<LogEntry>> SubmitLog([FromBody] LogEntry log)
    {
        // 1. Save the initial raw log entry first to generate an ID
        _context.Logs.Add(log);
        await _context.SaveChangesAsync();

        // 2. Call the free Groq AI backend to analyze the error logs
        try
        {
            var aiAnalysis = await _aiService.AnalyzeLogAsync(log.Message, log.StackTrace);
            if (aiAnalysis != null)
            {
                log.IsAiAnalyzed = true;
                log.AiDiagnosis = aiAnalysis.Diagnosis;
                log.AiSuggestedFix = aiAnalysis.SuggestedFix;

                // 3. Save the newly appended AI diagnosis insights back to SQLite
                await _context.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            // If the internet or AI service fails, the raw log still saves successfully
            //Console.WriteLine($"AI Analysis processing failed: {ex.Message}");
            Console.WriteLine($"\n[CRITICAL ERROR METRIC] AI Call Failed: {ex.Message}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"[INNER EXCEPTION DETAILS]: {ex.InnerException.Message}");
            }        
        }

        return CreatedAtAction(nameof(GetLogById), new { id = log.Id }, log);
    }

    // GET: api/logs
    [HttpGet]
    public async Task<ActionResult<IEnumerable<LogEntry>>> GetAllLogs()
    {
        return await _context.Logs.ToListAsync();
    }

    // GET: api/logs/5
    [HttpGet("{id}")]
    public async Task<ActionResult<LogEntry>> GetLogById(int id)
    {
        var log = await _context.Logs.FindAsync(id);
        if (log == null) return NotFound();

        return log;
    }
}
