using System.Timers;
using Semaphore = ParallelProgrammingLab1.PetriNet.Semaphore;

namespace ParallelProgrammingLab1;

public class ThreadScheduler : IDisposable
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
        
        _timer = new System.Timers.Timer(_timeslice / 2);
        _timer1 = new System.Timers.Timer(_timeslice);
        _timer.Elapsed += OnTimedEvent;
        _timer1.Elapsed += OnTimedEvent1;
        _timer.Start();
        _timer1.Start();

        _locker.WaitOne();
    }
    
    private void OnTimedEvent(object? source, ElapsedEventArgs e)
    {
        Interlocked.Increment(ref _sync);

        var semaphores = Semaphore.Semaphores.Aggregate("", (current, resourse) => current + resourse);

        var threads = MyThread.Threads.Aggregate("", (current, thread) => current + thread);

        OutputFile.WriteLine($"\t{_quantumNumber, -4} | {semaphores, -60} | {threads}");

        _quantumNumber++;

        Interlocked.Decrement(ref _sync);
    }
    
    private void OnTimedEvent1(object? source, ElapsedEventArgs e)
    {
        Interlocked.Increment(ref _sync);

        _threads.Sort(_comparator);
        foreach (var thread in _threads)
        {
            if (thread.State == ThreadState.InQueue)
            {
                thread.Execute(_timeslice, _preemptive ? 1 : thread.CpuBurst);
            }
        }

        if (_threads.All(t => t.State == ThreadState.Completed))
        {
            _timer.Stop();
            _timer1.Stop();

            Interlocked.Decrement(ref _sync);

            while (_sync > 0) { }
        
            _timer.Elapsed -= OnTimedEvent;
            _timer1.Elapsed -= OnTimedEvent1;
            _timer.Dispose();
            _timer1.Dispose();
        
            _locker.Set();

            return;
        }

        Interlocked.Decrement(ref _sync);
    }
    
    public void Dispose()
    {
        _locker.Dispose();
    }
    
    public StreamWriter OutputFile { get; set; }

    private readonly int _timeslice;
    
    private readonly bool _preemptive;

    private readonly List<MyThread> _threads;
    
    private readonly AutoResetEvent _locker = new(false);
    
    private int _quantumNumber;
    
    private int _sync;

    private Comparison<MyThread> _comparator;

    private System.Timers.Timer _timer;
    
    private System.Timers.Timer _timer1;
}
