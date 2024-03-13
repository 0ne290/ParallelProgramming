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
        
        Threads.Add(this);
    }
    
    public override string ToString() => $"{Name} state = {State};";
    
    public void Execute(int cpuBurst)
    {
        var waitTimeInMs = Resource.Timeslice * cpuBurst;
        var currentResourceIndex = 0;
        var currentResource = _resources[currentResourceIndex];
        var stopwatch = new Stopwatch();
        
        while (_quantity > 0)
        {
            State = ThreadStates.InQueue;
            Locker = true;
            currentResource.Hold(this);
            while (Locker) { }
            State = ThreadStates.Running;
		
            stopwatch.Restart();
            while (stopwatch.ElapsedMilliseconds < waitTimeInMs) { }
		
            currentResource.Release(Name);
		
            _cpuBurstCompleted += cpuBurst;
            if (_cpuBurstCompleted < _cpuBurst)
            {
                continue;
            }
        
            currentResourceIndex++;
            if (currentResourceIndex == _resources.Length)
            {
                if (_quantity < 2)
                {
                    State = ThreadStates.Completed;
                    return;
                }
                _quantity--;
                currentResourceIndex = 0;
            }
            currentResource = _resources[currentResourceIndex];
            _cpuBurstCompleted = 0;
        }
    }

    public int GetRestOfCpuBurst() => _cpuBurst - _cpuBurstCompleted;

    public ThreadStates State { get; set; } = ThreadStates.InQueue;
    
    public string Name { get; }
    
    public int Priority { get; }
    
    public bool Locker { get; set; }

    public static List<MyThread> Threads { get; } = new();

    private readonly Resource[] _resources;

    private readonly int _cpuBurst;

    private int _quantity;
    
    private int _cpuBurstCompleted;
}