using System.Diagnostics;

namespace ParallelProgrammingLab1;

public class MyThread
{
    public MyThread(string name, int quantity, int cpuBurst)
    {
        Name = name;
        Quantity = quantity;
        CpuBurst = cpuBurst;
        
        Threads.Add(this);
    }
    
    public override string ToString() => $"{Name} run = {IsRunning};";
    
    public void Execute(int cpuBurst)
    {
        Task.Run(() =>
        {
            _stopwatch.Restart();

            while (_stopwatch.ElapsedMilliseconds < cpuBurst)
            {
            }

            IsRunning = false;
        });
    }

    public static List<MyThread> Threads { get; } = new();
    
    public string Name { get; set; }
    
    public int Quantity { get; set; }
    
    public int CpuBurst { get; set; }
    
    public bool IsRunning { get; set; }

    private readonly Stopwatch _stopwatch = new();
}