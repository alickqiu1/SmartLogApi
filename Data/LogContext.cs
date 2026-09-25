using Microsoft.EntityFrameworkCore;
using SmartLogApi.Models;

namespace SmartLogApi.Data;

public class LogContext : DbContext
{
    public LogContext(DbContextOptions<LogContext> options) : base(options) { }

    public DbSet<LogEntry> Logs => Set<LogEntry>();
}