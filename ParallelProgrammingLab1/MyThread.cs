using System.Diagnostics;
using ThreadState = System.Diagnostics.ThreadState;

namespace ParallelProgrammingLab1;

public class MyThread
{
    public MyThread(string name, int quantity, int cpuBurst, int priority)
    {
        Name = name;
        Quantity = quantity;
        CpuBurst = cpuBurst;
        Priority = priority;
        
        Threads.Add(this);
    }
    
    public override string ToString() => $"{Name} state = {State};";
    
    public void Execute(int cpuBurst)
    {
        Task.Run(() =>
        {
            _stopwatch.Restart();

            while (_stopwatch.ElapsedMilliseconds < cpuBurst)
            {
            }

            State = ThreadStates.InQueue;
        });
    }
    
    public int GetRestOfCpuBurst() => CpuBurst - CpuBurstCompleted;

    public static List<MyThread> Threads { get; } = new();
    
    public string Name { get; set; }
    
    public int Quantity { get; set; }
    
    public int CpuBurst { get; set; }
    
    public int CpuBurstCompleted { get; set; }
    
    public int Priority { get; set; }

    public ThreadStates State { get; set; } = ThreadStates.InQueue;

    private readonly Stopwatch _stopwatch = new();
}