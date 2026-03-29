using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeTrackerRepo.Data
{
    public class DbLoggerProvider : ILoggerProvider
    {
        private readonly ILogger _logger;
        private readonly IServiceScope _serviceScopeFactory;
        private TimeTrackerContext _context;

        public DbLoggerProvider(ILogger? logger,IServiceScope serviceScopeFactory)
        {
            if (logger != null)
            {
                _logger = logger;
            }
            _serviceScopeFactory = serviceScopeFactory;
            //            _serviceScopeFactory = serviceScopeFactory;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new DbLogger(_logger,_serviceScopeFactory);
        }

        public void Dispose()
        {
        }
    }
    public class DbLogger : ILogger
    {
        private readonly ILogger _logger;
        private readonly IServiceScope _serviceScopeFactory;

        public DbLogger(ILogger logger,IServiceScope serviceScopeFactory)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            using var scope = _serviceScopeFactory;
            var context = scope.ServiceProvider.GetService<TimeTrackerContext>();

            if (logLevel == LogLevel.Information)
            {
                context.Logs.Add(new Logs
                {
                    Message = formatter(state, exception),
                    LogLevel = logLevel.ToString(),
                    CreatedAt = DateTime.UtcNow
                });

                context.SaveChanges();
            }
            else
            {
                _logger.Log(logLevel, eventId, state, exception, formatter);
            }
        }
    }

}
