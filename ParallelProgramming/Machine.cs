using System.Diagnostics;

namespace ParallelProgramming;

public class Machine
{
    private Machine(string name, int capacity)
    {
        Name = name;

        _capacity = capacity;
    }
    
    public async Task Mill(int milliseconds)
    {
        var stopWatch = new Stopwatch();

        await Task.Run(() =>
        {
            stopWatch.Start();
            while (stopWatch.ElapsedMilliseconds < milliseconds)
            {
            }
        });
    }

    public bool IsAvailable() => _flow < _capacity;

    public void Hold(string detailName)
    {
        _flow++;
        _detailNames.Add(detailName);
    }
    
    public void Release(string detailName)
    {
        _flow--;
        _detailNames.Remove(detailName);
    }
    
    public static Machine[] GetMachinesByName(string[] machineNames) => Machines.Where(m => machineNames.Contains(m.Name)).ToArray();
    
    public static void CreateMachine(string name, int capacity)
    {
        var machine = new Machine(name, capacity);
        ((List<Machine>)Machines).Add(machine);
    }
    
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
    
    public static IEnumerable<Machine> Machines { get; } = new List<Machine>();

    private readonly List<string> _detailNames = new();

    private readonly int _capacity;
    
    private int _flow;
    
    private readonly object _nameBlocker = new();
}