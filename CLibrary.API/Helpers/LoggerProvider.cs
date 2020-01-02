using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CLibrary.API.Helpers {
    public class LoggerProvider : ILoggerProvider {
        public ILogger CreateLogger(string categoryName) {
            return new MyLogger();
        }

        public void Dispose(){}

        private class MyLogger : ILogger {
            public IDisposable BeginScope<TState>(TState state) {
                return null;
            }

            public bool IsEnabled(LogLevel logLevel) {
                return true;
            }

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, 
                Exception exception, Func<TState, Exception, string> formatter) {
                Console.WriteLine(formatter(state, exception));
            }
        }
    }
}
