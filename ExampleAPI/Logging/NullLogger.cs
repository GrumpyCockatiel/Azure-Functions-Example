using System.Collections.Concurrent;
using System.Transactions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Raydreams.API.Example
{
    /// <summary>Null logger</summary>
    public class NullLogger : ILogger
    {
        public IDisposable BeginScope<TState>(TState state) => default!;

        public bool IsEnabled( LogLevel logLevel )
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            return;
        }
    }

    /// <summary>A Null Logger</summary>
    public sealed class NullLoggerProvider : ILoggerProvider
    {
        public NullLoggerProvider()
        {
        }

        public ILogger CreateLogger(string categoryName) => new NullLogger();

        public void Dispose()
        {
            return;
        }
    }
}
