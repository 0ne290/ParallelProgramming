using System.Diagnostics;

namespace ParallelProgramming;

public class Machine
{
    public Machine(string name, int capacity)
    {
        Name = name;
        _capacity = capacity;
    }
    
    public void Mill(int milliseconds, string detailName)
    {
        var stopWatch = new Stopwatch();
        
        lock (_nameBlocker)
        {
            _detailNames.Add(detailName);
        }
        
        stopWatch.Start();
        while (stopWatch.ElapsedMilliseconds < milliseconds) { }
        
        lock (_nameBlocker)
        {
            _detailNames.Remove(detailName);
        }
    }

    public bool IsAvailable() => _flow < _capacity;

    public void IncrementFlow() => _flow++;
    
    public void DecrementFlow() => Interlocked.Decrement(ref _flow);
    
    public string Name { get; }

    public List<string> DetailNames
    {
        get
        {
            lock (_nameBlocker)
            {
                return new List<string>(_detailNames);
            }
        }
    }
    
    public Queue<Task> TaskQueue { get; } = new Queue<Task>();
    
    private int _capacity;
    
    private int _flow;

    private List<string> _detailNames = new();
    
    private object _nameBlocker = new();
}