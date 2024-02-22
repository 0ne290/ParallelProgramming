using System.Diagnostics;

namespace ParallelProgramming;

public class Machine
{
    private Machine(string name, int capacity)
    {
        Name = name;
        
        Semaphore = new Semaphore(capacity, capacity);
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
    
    public static Machine[] GetMachinesByName(string[] machineNames) => Machines.Where(m => machineNames.Contains(m.Name)).ToArray();
    
    public static void CreateMachine(string name, int capacity)
    {
        var machine = new Machine(name, capacity);
        ((List<Machine>)Machines).Add(machine);
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
    
    public static IEnumerable<Machine> Machines { get; } = new List<Machine>();

    private readonly List<string> _detailNames = new();
    
    private readonly object _nameBlocker = new();
}