#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
namespace Microsoft.Extensions.Logging;
#pragma warning restore IDE0130 // 命名空间与文件夹结构不匹配


public static class LoggerScopeExtensions
{
    extension(ILogger logger)
    {
        public ScopeBuilder Scope()
        {
            return new ScopeBuilder(logger);
        }

        public ScopeBuilder Scope(string name, object value)
        {
            var builder =  new ScopeBuilder(logger);
            builder.Add(name, value);

            return builder;
        }

        public IDisposable? BeginScope(string name, object value)
        {
            var builder = new ScopeBuilder(logger);
            builder.Add(name, value);

            return builder.Begin();
        }
    }

    public sealed class ScopeBuilder(ILogger logger)
    {
        private readonly Dictionary<string, object> _values = [];

        public ScopeBuilder Add(string name, object value)
        {
            _values[name] = value;
            return this;
        }

        public IDisposable? Begin()
        {
            return logger.BeginScope(_values);
        }
    }
}