using System.Timers;
using ParallelProgrammingLab1.PetriNetSemaphore;

namespace ParallelProgrammingLab1;

public class ThreadScheduler
{
    public ThreadScheduler(int timeslice, IEnumerable<MyThread> threads)
    {
        _timeslice = timeslice;
        _threads = threads.ToList();
    }

    public void Execute()// SJF, premptive, absolute priority
    {
        Comparison<MyThread> comparator = (x, y) =>
        {
            var ret = y.Priority.CompareTo(x.Priority);
            return ret != 0 ? ret : x.GetRestOfCpuBurst().CompareTo(y.GetRestOfCpuBurst());
        };
        
        var timer = new System.Timers.Timer(_timeslice - 50);
        timer.Elapsed += OnTimedEvent;
        timer.Start();
        
        //jjj
        
        timer.Stop();

        while (_sync) { }// Можно было бы заменить это на AutoResetEvent или добавить к спин-локу код, отдающий квант времени этого потока другому потоку (Thread.Curent.Join(100) или Thread.Sleep(0))
        
        timer.Elapsed -= OnTimedEvent;
        timer.Close();
        timer.Dispose();
    }
    
    private void OnTimedEvent(object? source, ElapsedEventArgs e)
    {
        _sync = true;

        var resources = Resource.Resources.Aggregate("", (current, resourse) => current + resourse);

        var threads = MyThread.Threads.Aggregate("", (current, thread) => current + thread);

        Console.WriteLine($"{_quantumNumber} | {resources} | {threads}");

        _quantumNumber++;

        _sync = false;
    }
    
    private List<MyThread> _threads;

    private readonly int _timeslice;
    
    private bool _sync;
    
    private int _quantumNumber;
}