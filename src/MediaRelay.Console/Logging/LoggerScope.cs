using System.Collections;

namespace MediaRelay.Console.Logging;


internal sealed class LoggerScope(LoggerScope? parent = null) : IEnumerable<KeyValuePair<string, object>>, IDisposable
{
    private bool _disposed;
    private readonly Dictionary<string, object> _datas = [];
    private LoggerScope? _parent = parent;
    private readonly List<LoggerScope> _childs = [];


    public int Count => CompineCount(this);

    public void AddRange(IEnumerable<KeyValuePair<string, object>> datas)
    {
        foreach (var i in datas.ToArray()) _datas[i.Key] = i.Value;
    }
    public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
    {
        var datas = new Dictionary<string, object>();
        CompineDatas(this, datas);

        return datas.ToList().AsReadOnly().GetEnumerator();
    }
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }


    public LoggerScope CreateChildScope()
    {
        var scope = new LoggerScope(this);
        _childs.Add(scope);
        return scope;
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;

        foreach (var i in _childs.ToArray())
        {
            i.Dispose();
        }

        _childs.Clear();
        _datas.Clear();
    }



    private static void CompineDatas(LoggerScope? scope, Dictionary<string, object> datas)
    {
        if (scope is null) return;
        if (scope._parent is not null) CompineDatas(scope._parent, datas);

        foreach (var i in scope._datas) datas[i.Key] = i.Value;
    }

    private static int CompineCount(LoggerScope? scope)
    {
        if (scope is null) return 0;

        if (scope._parent is not null) return scope._datas.Count + CompineCount(scope._parent);
        else return scope._datas.Count;
    }
}