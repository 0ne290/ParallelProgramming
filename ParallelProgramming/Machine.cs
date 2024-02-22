using System.Diagnostics;

namespace ParallelProgramming;

public class Machine
{
    public Machine(string name, int capacity)
    {
        Name = name;
        
        Semaphore = new Semaphore(capacity, capacity);
        
        ((List<Machine>)Machines).Add(this);
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
    
    public string Name { get; }
    
    public Semaphore Semaphore { get; }

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

    public static Machine[] GetMachinesByName(string[] machineNames) => Machines.Where(m => machineNames.Contains(m.Name)).ToArray();
    
    public static IEnumerable<Machine> Machines { get; } = new List<Machine>();

    private readonly List<string> _detailNames = new();
    
    private readonly object _nameBlocker = new();
}