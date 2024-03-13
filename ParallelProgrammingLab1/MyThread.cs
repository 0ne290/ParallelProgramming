using System.Diagnostics;
using ParallelProgrammingLab1.PetriNetSemaphore;

namespace ParallelProgrammingLab1;

public class MyThread
{
    public MyThread(string name, int priority, int cpuBurst, int quantity, IEnumerable<Resource> resources)
    {
        Name = name;
        Priority = priority;
        _cpuBurst = cpuBurst;
        _quantity = quantity;
        _resources = resources.ToArray();

        _currentResource = _resources[0];
        
        Threads.Add(this);
    }
    
    public override string ToString() => $"{Name} state = {State};";
    
    public void Execute(int timeslice)
    {
        _currentResource.Hold(this);
		
        _stopwatch.Restart();
        while (_stopwatch.ElapsedMilliseconds < timeslice) { }
		
        _currentResource.Release(this);
		
        _cpuBurstCompleted++;
        if (_cpuBurstCompleted != _cpuBurst)
            return;
        
        _currentResourceIndex++;
        if (_currentResourceIndex == _resources.Length)
        {
            if (_quantity == 1)
            {
                State = ThreadStates.Completed;
                return;
            }
            _quantity--;
            _currentResourceIndex = 0;
        }
        _currentResource = _resources[_currentResourceIndex];
        _cpuBurstCompleted = 0;
    }

    public int GetRestOfCpuBurst() => _cpuBurst - _cpuBurstCompleted;

    public ThreadStates State { get; set; } = ThreadStates.InQueue;
    
    public string Name { get; }
    
    public int Priority { get; }

    public static List<MyThread> Threads { get; } = new();

    private readonly Resource[] _resources;

    private readonly int _cpuBurst;

    private int _quantity;
    
    private int _cpuBurstCompleted; 

    private Resource _currentResource;
    
    private int _currentResourceIndex;

    private readonly Stopwatch _stopwatch = new();
}