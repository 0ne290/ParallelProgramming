using System.Timers;
using Semaphore = ParallelProgrammingLab1.PetriNet.Semaphore;

namespace ParallelProgrammingLab1;

public class ThreadScheduler : IDisposable, IAsyncDisposable
{
    public ThreadScheduler(int timeslice, bool premptive)
    {
        
    }
    
    public void Execute()
    {
        var timer = new System.Timers.Timer(_timeslice);
        timer.Elapsed += OnTimedEvent1;
        timer.Elapsed += OnTimedEvent;
        timer.Start();
        
        timer.Stop();

        while (_sync > 0) { }
        
        timer.Elapsed -= OnTimedEvent1;
        timer.Elapsed -= OnTimedEvent;
        timer.Dispose();
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

        foreach (var resource in Resources)
            resource.Distribute();

        Interlocked.Decrement(ref _sync);
    }
    
    public void Dispose() => _outputFile.Dispose();

    public async ValueTask DisposeAsync() => await _outputFile.DisposeAsync();

    private readonly int _timeslice;
    
    private readonly StreamWriter _outputFile = new("../../../Output.txt", false);
    
    private int _quantumNumber;
    
    private int _sync;
}