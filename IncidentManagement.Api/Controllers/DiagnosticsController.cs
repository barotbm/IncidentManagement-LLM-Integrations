using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace IncidentManagement.Api.Controllers;

/// <summary>
/// Diagnostics controller to expose thread pool metrics and prove async behavior.
/// 
/// STAFF-LEVEL VALIDATION:
/// "I validate through documentation, load testing, and thread pool metrics.
/// Some async APIs still block internally, so I confirm behavior under concurrency."
/// 
/// This controller provides empirical evidence that:
/// 1. Async/await actually frees threads
/// 2. Synchronous blocking holds threads
/// 3. Thread pool behavior under load
/// </summary>
[ApiController]
[Route("diagnostics")]
public class DiagnosticsController : ControllerBase
{
    private readonly ILogger<DiagnosticsController> _logger;

    public DiagnosticsController(ILogger<DiagnosticsController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Returns current thread pool statistics.
    /// Use this to monitor thread availability during load tests.
    /// </summary>
    /// <remarks>
    /// PROOF TECHNIQUE:
    /// - Run load test against /slow-sync endpoint
    /// - Watch available worker threads drop to near zero
    /// - Run load test against /slow-async endpoint
    /// - Watch available worker threads remain high
    /// </remarks>
    [HttpGet("threadpool")]
    public IActionResult GetThreadPoolStats()
    {
        ThreadPool.GetAvailableThreads(out int availableWorkerThreads, out int availableCompletionPortThreads);
        ThreadPool.GetMaxThreads(out int maxWorkerThreads, out int maxCompletionPortThreads);
        ThreadPool.GetMinThreads(out int minWorkerThreads, out int minCompletionPortThreads);

        var stats = new
        {
            Timestamp = DateTime.UtcNow,
            WorkerThreads = new
            {
                Available = availableWorkerThreads,
                InUse = maxWorkerThreads - availableWorkerThreads,
                Max = maxWorkerThreads,
                Min = minWorkerThreads,
                UtilizationPercent = Math.Round(((double)(maxWorkerThreads - availableWorkerThreads) / maxWorkerThreads) * 100, 2)
            },
            CompletionPortThreads = new
            {
                Available = availableCompletionPortThreads,
                InUse = maxCompletionPortThreads - availableCompletionPortThreads,
                Max = maxCompletionPortThreads,
                Min = minCompletionPortThreads
            },
            ProcessInfo = new
            {
                ProcessId = Environment.ProcessId,
                ThreadCount = Process.GetCurrentProcess().Threads.Count,
                WorkingSetMB = Math.Round(Process.GetCurrentProcess().WorkingSet64 / 1024.0 / 1024.0, 2)
            }
        };

        _logger.LogInformation(
            "Thread pool stats: {Available}/{Max} worker threads available ({Utilization}% utilized)",
            availableWorkerThreads,
            maxWorkerThreads,
            stats.WorkerThreads.UtilizationPercent);

        return Ok(stats);
    }

    /// <summary>
    /// SYNCHRONOUS (BLOCKING) endpoint - Proves threads are NOT freed.
    /// This holds a thread for the entire duration of the delay.
    /// </summary>
    /// <remarks>
    /// ANTI-PATTERN DEMONSTRATION:
    /// Thread.Sleep() blocks the thread - it cannot be reused.
    /// Under load, this will exhaust the thread pool.
    /// 
    /// TEST:
    /// curl https://localhost:5001/diagnostics/slow-sync?delayMs=5000
    /// 
    /// LOAD TEST (PowerShell):
    /// 1..50 | ForEach-Object -Parallel { 
    ///     Invoke-RestMethod "https://localhost:5001/diagnostics/slow-sync?delayMs=5000" -SkipCertificateCheck
    /// }
    /// 
    /// Then check: GET /diagnostics/threadpool
    /// You'll see available threads drop significantly.
    /// </remarks>
    [HttpGet("slow-sync")]
    public IActionResult SlowSynchronous([FromQuery] int delayMs = 1000)
    {
        var threadId = Environment.CurrentManagedThreadId;
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString() ?? "N/A";

        _logger.LogWarning(
            "SYNC (BLOCKING) request started. CorrelationId: {CorrelationId}, ThreadId: {ThreadId}, Delay: {DelayMs}ms",
            correlationId, threadId, delayMs);

        var sw = Stopwatch.StartNew();

        // ❌ BAD: Thread.Sleep BLOCKS the thread
        // This thread cannot handle other requests during this time
        Thread.Sleep(delayMs);

        sw.Stop();

        _logger.LogWarning(
            "SYNC (BLOCKING) request completed. CorrelationId: {CorrelationId}, ThreadId: {ThreadId}, Duration: {Duration}ms",
            correlationId, threadId, sw.ElapsedMilliseconds);

        return Ok(new
        {
            Method = "Synchronous (Thread.Sleep)",
            ThreadId = threadId,
            RequestedDelayMs = delayMs,
            ActualDurationMs = sw.ElapsedMilliseconds,
            Warning = "This thread was BLOCKED and unavailable for other requests",
            CorrelationId = correlationId
        });
    }

    /// <summary>
    /// ASYNCHRONOUS (NON-BLOCKING) endpoint - Proves threads ARE freed.
    /// This frees the thread during the delay.
    /// </summary>
    /// <remarks>
    /// BEST PRACTICE DEMONSTRATION:
    /// Task.Delay() is truly async - the thread is freed during the wait.
    /// Under load, this will NOT exhaust the thread pool.
    /// 
    /// TEST:
    /// curl https://localhost:5001/diagnostics/slow-async?delayMs=5000
    /// 
    /// LOAD TEST (PowerShell):
    /// 1..50 | ForEach-Object -Parallel { 
    ///     Invoke-RestMethod "https://localhost:5001/diagnostics/slow-async?delayMs=5000" -SkipCertificateCheck
    /// }
    /// 
    /// Then check: GET /diagnostics/threadpool
    /// You'll see available threads remain high - threads are reused!
    /// </remarks>
    [HttpGet("slow-async")]
    public async Task<IActionResult> SlowAsynchronous([FromQuery] int delayMs = 1000)
    {
        var startThreadId = Environment.CurrentManagedThreadId;
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString() ?? "N/A";

        _logger.LogInformation(
            "ASYNC (NON-BLOCKING) request started. CorrelationId: {CorrelationId}, ThreadId: {ThreadId}, Delay: {DelayMs}ms",
            correlationId, startThreadId, delayMs);

        var sw = Stopwatch.StartNew();

        // ✅ GOOD: Task.Delay is truly async - thread is freed during wait
        // The thread can handle other requests during this time
        await Task.Delay(delayMs);

        sw.Stop();

        var endThreadId = Environment.CurrentManagedThreadId;

        _logger.LogInformation(
            "ASYNC (NON-BLOCKING) request completed. CorrelationId: {CorrelationId}, StartThreadId: {StartThreadId}, EndThreadId: {EndThreadId}, Duration: {Duration}ms",
            correlationId, startThreadId, endThreadId, sw.ElapsedMilliseconds);

        return Ok(new
        {
            Method = "Asynchronous (Task.Delay)",
            StartThreadId = startThreadId,
            EndThreadId = endThreadId,
            ThreadReused = startThreadId != endThreadId,
            RequestedDelayMs = delayMs,
            ActualDurationMs = sw.ElapsedMilliseconds,
            Benefit = "Thread was FREED and available for other requests during delay",
            CorrelationId = correlationId
        });
    }

    /// <summary>
    /// FAKE ASYNC (BLOCKING) endpoint - Proves that async doesn't guarantee non-blocking.
    /// This is async but still blocks internally - anti-pattern!
    /// </summary>
    /// <remarks>
    /// ANTI-PATTERN DEMONSTRATION:
    /// Task.Run(() => Thread.Sleep()) is still blocking a thread (from thread pool).
    /// This is "fake async" - it looks async but doesn't provide the benefits.
    /// 
    /// STAFF-LEVEL INSIGHT:
    /// "Some async APIs still block internally, so I confirm behavior under concurrency."
    /// This is why you must validate actual behavior, not just the async keyword.
    /// 
    /// EXAMPLES IN THE WILD:
    /// - Database drivers that use synchronous I/O internally
    /// - HTTP clients that block on sockets
    /// - File I/O that doesn't use async Windows APIs
    /// </remarks>
    [HttpGet("slow-fake-async")]
    public async Task<IActionResult> SlowFakeAsync([FromQuery] int delayMs = 1000)
    {
        var threadId = Environment.CurrentManagedThreadId;
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString() ?? "N/A";

        _logger.LogWarning(
            "FAKE ASYNC (STILL BLOCKING) request started. CorrelationId: {CorrelationId}, ThreadId: {ThreadId}, Delay: {DelayMs}ms",
            correlationId, threadId, delayMs);

        var sw = Stopwatch.StartNew();

        // ❌ ANTI-PATTERN: Task.Run with Thread.Sleep still blocks a thread pool thread
        // This is "fake async" - it's async but still consumes a thread
        await Task.Run(() => Thread.Sleep(delayMs));

        sw.Stop();

        _logger.LogWarning(
            "FAKE ASYNC (STILL BLOCKING) request completed. CorrelationId: {CorrelationId}, ThreadId: {ThreadId}, Duration: {Duration}ms",
            correlationId, threadId, sw.ElapsedMilliseconds);

        return Ok(new
        {
            Method = "Fake Async (Task.Run + Thread.Sleep)",
            ThreadId = threadId,
            RequestedDelayMs = delayMs,
            ActualDurationMs = sw.ElapsedMilliseconds,
            Warning = "This is async but STILL BLOCKS a thread pool thread - anti-pattern!",
            CorrelationId = correlationId
        });
    }

    /// <summary>
    /// CPU-bound work endpoint - Shows when synchronous is appropriate.
    /// </summary>
    /// <remarks>
    /// APPROPRIATE SYNCHRONOUS USE:
    /// CPU-bound work should NOT be async - there's no I/O to await.
    /// Making CPU work async just adds overhead without benefit.
    /// 
    /// RULE OF THUMB:
    /// - I/O-bound: Use async (file, network, database)
    /// - CPU-bound: Use synchronous or Task.Run if you need parallelism
    /// </remarks>
    [HttpGet("cpu-bound")]
    public IActionResult CpuBound([FromQuery] int iterations = 1000000)
    {
        var threadId = Environment.CurrentManagedThreadId;
        var sw = Stopwatch.StartNew();

        // CPU-bound work - appropriately synchronous
        long sum = 0;
        for (int i = 0; i < iterations; i++)
        {
            sum += i;
        }

        sw.Stop();

        return Ok(new
        {
            Method = "Synchronous CPU-bound work",
            ThreadId = threadId,
            Iterations = iterations,
            Result = sum,
            DurationMs = sw.ElapsedMilliseconds,
            Note = "CPU-bound work is appropriately synchronous - no I/O to await"
        });
    }
}
