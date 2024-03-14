using System.Diagnostics;
using ParallelProgrammingLab1.PetriNetSemaphore;

namespace ParallelProgrammingLab1;

public class MyThread
{
    public MyThread(string name, int priority, int cpuBurst, int quantity, IEnumerable<Resource> resources)
    {
	if (string.IsNullOrWhiteSpace(name))
	{
	    Name = $"T{_threadCounter}";
	    _threadCounter++;
	}
        if (Threads.Any(r => r.Name == name))
            throw new Exception($"Поток с именем {name} уже существует.");
        
        Name = name;
        Priority = priority;
        CpuBurst = cpuBurst;
        _quantity = quantity;
        _resources = resources.ToArray();
        
        Threads.Add(this);
    }
    
    public override string ToString() => $"{Name} state = {State};";
    
    public void Execute(bool premptive)
    {
        var cpuBurst = premptive ? 1 : CpuBurst;
        var waitTimeInMs = Resource.Timeslice * cpuBurst;
        var currentResourceIndex = 0;
        var currentResource = _resources[currentResourceIndex];
        var stopwatch = new Stopwatch();
        
        while (true)
        {
            State = ThreadStates.InQueue;
            currentResource.Hold(this);
            Locker.WaitOne();
            State = ThreadStates.Running;
		
            stopwatch.Restart();
            while (stopwatch.ElapsedMilliseconds < waitTimeInMs) { }
		
            currentResource.Release(Name);
		
            _cpuBurstCompleted += cpuBurst;
            if (_cpuBurstCompleted < CpuBurst)
            {
                continue;
            }
        
            currentResourceIndex++;
            if (currentResourceIndex == _resources.Length)
            {
                if (_quantity < 2)
                {
                    State = ThreadStates.Completed;
                    Locker.Dispose();
                    return;
                }
                _quantity--;
                currentResourceIndex = 0;
            }
            currentResource = _resources[currentResourceIndex];
            _cpuBurstCompleted = 0;
        }
    }

    public int GetRestOfCpuBurst() => CpuBurst - _cpuBurstCompleted;
    
    public ThreadStates State { get; private set; } = ThreadStates.InQueue;
    
    public string Name { get; }
    
    public int Priority { get; }
    
    public int CpuBurst { get; }

    public AutoResetEvent Locker { get; } = new(false);

    public static List<MyThread> Threads { get; } = new();

    private readonly Resource[] _resources;

    private int _quantity;
    
    private int _cpuBurstCompleted;

    private static int _threadCounter = 1;
}
