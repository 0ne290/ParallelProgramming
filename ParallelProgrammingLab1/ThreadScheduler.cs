using System.Timers;
using Semaphore = ParallelProgrammingLab1.PetriNet.Semaphore;

namespace ParallelProgrammingLab1;

public class ThreadScheduler : IDisposable, IAsyncDisposable
{
    public ThreadScheduler(int timeslice, bool preemptive)
    {
        _timeslice = timeslice;
        _threads = new List<MyThread>(MyThread.Threads);
        _preemptive = preemptive;
    }
    
    public void Execute()
    {
        _comparator = _preemptive
            ? (x, y) =>
            {
                var ret = y.Priority.CompareTo(x.Priority);
                return ret != 0 ? ret : x.GetRestOfCpuBurst().CompareTo(y.GetRestOfCpuBurst());
            }
            : (x, y) => x.CpuBurst.CompareTo(y.CpuBurst);
        
        var timer = new System.Timers.Timer(_timeslice / 2);
        var timer1 = new System.Timers.Timer(_timeslice);
        timer.Elapsed += OnTimedEvent;
        timer1.Elapsed += OnTimedEvent1;
        timer.Start();
        timer1.Start();

        _locker.WaitOne();
        
        timer.Stop();
        timer1.Stop();

        while (_sync > 0) { }
        
        timer.Elapsed -= OnTimedEvent;
        timer1.Elapsed -= OnTimedEvent1;
        timer.Dispose();
        timer1.Dispose();
    }
    
    private void OnTimedEvent(object? source, ElapsedEventArgs e)
    {
        Interlocked.Increment(ref _sync);

        var semaphores = Semaphore.Semaphores.Aggregate("", (current, resourse) => current + resourse);

        var threads = MyThread.Threads.Aggregate("", (current, thread) => current + thread);

        _outputFile.WriteLine($"\t{_quantumNumber, -4} | {semaphores, -60} | {threads}");

        _quantumNumber++;

        Interlocked.Decrement(ref _sync);
    }
    
    private void OnTimedEvent1(object? source, ElapsedEventArgs e)
    {
        Interlocked.Increment(ref _sync);

        _threads.Sort();
        
        

        Interlocked.Decrement(ref _sync);
    }
    
    public void Dispose()
    {
        _outputFile.Dispose();
        _locker.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await _outputFile.DisposeAsync();
        _locker.Dispose();
    }

    private readonly int _timeslice;

    private readonly List<MyThread> _threads;
    
    private readonly StreamWriter _outputFile = new("../../../Output.txt", false);
    
    private readonly AutoResetEvent _locker = new(true);

    public bool _preemptive;
    
    private int _quantumNumber;
    
    private int _sync;

    private Comparison<MyThread> _comparator;
}