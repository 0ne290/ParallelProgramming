using System.Diagnostics;

namespace ParallelProgramming;

public class Machine
{
    public Machine(string name, int capacity)
    {
        Name = name;
        _semaphore = new Semaphore(capacity, capacity);
    }
    
    public void Mill(int milliseconds, string detailName)
    {
        var stopWatch = new Stopwatch();
        
        _semaphore.WaitOne();
        
        lock (_lockObj)
        {
            _detailNames.Add(detailName);
        }
        
        stopWatch.Start();
        while (stopWatch.ElapsedMilliseconds < milliseconds) { }
        
        lock (_lockObj)
        {
            _detailNames.Remove(detailName);
        }

        _semaphore.Release();
    }
    
    public string Name { get; }

    public List<string> DetailNames
    {
        get
        {
            lock (_lockObj)
            {
                return new List<string>(_detailNames);
            }
        }
    }

    private List<string> _detailNames = new();

    private Semaphore _semaphore;
    
    private object _lockObj = new();
}