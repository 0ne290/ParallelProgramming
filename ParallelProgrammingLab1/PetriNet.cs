using System.Timers;

namespace ParallelProgrammingLab1;

public class PetriNet
{
    public PetriNet(Dictionary<MyThread, PetriSubnet> subnets, List<MyThread> threads, int timeSlice)
    {
        _subnets = subnets;
        _threads = threads;
        _timeSlice = timeSlice;
    }
    
    public void Execute()
    {
        var timer = new System.Timers.Timer(_timeSlice);
        timer.Elapsed += OnTimedEvent;
        timer.Start();
        
        while (_threads.Count > 0)
        {
            var thread = _threads[_indexThreads];
            var subnet = _subnets[thread];
            
            if (subnet.TransitionIsAvailable())
            {
                if (subnet.TransitionIsHolding())
                {
                    subnet.ExecuteTransition();
                    subnet.HoldResourse(thread.Name);
                    thread.IsRunning = true;
                    thread.Execute(thread.CpuBurst);
                }
                else if (!thread.IsRunning)
                {
                    subnet.RealeseResourse(thread.Name);
                    if (subnet.ExecuteTransition())
                    {
                        if (thread.Quantity == 1)
                            _threads.Remove(thread);
                        else
                            thread.Quantity--;
                    }
                }
            }

            if (_indexThreads >= _threads.Count - 1)
            {
                _indexThreads = 0;
            }
            else
                _indexThreads++;
        }
        
        timer.Stop();

        while (_sync) { }// Можно было бы заменить это на AutoResetEvent или добавить к спин-локу код, отдающий квант времени этого потока другому потоку (Thread.Curent.Join(100) или Thread.Sleep(0))
        
        timer.Elapsed -= OnTimedEvent;
        timer.Close();
        timer.Dispose();
    }
    
    private void OnTimedEvent(object? source, ElapsedEventArgs e)
    {
        _sync = true;

        var resourses = Resourse.Resourses.Aggregate("", (current, resourse) => current + resourse);

        var threads = MyThread.Threads.Aggregate("", (current, thread) => current + thread);

        Console.WriteLine($"{_quantumNumber} | {resourses} | {threads}");

        _quantumNumber++;

        _sync = false;
    }
    
    private bool _sync;
    
    private Dictionary<MyThread, PetriSubnet> _subnets;
    
    private List<MyThread> _threads;

    private int _quantumNumber;

    private int _timeSlice;

    private int _indexThreads;
}