using System.Diagnostics;
using Semaphore = ParallelProgrammingLab1.PetriNet.Semaphore;

namespace ParallelProgrammingLab1;

public class MyThread
{
    public MyThread(string name, int priority, int cpuBurst, int quantity, IEnumerable<Semaphore> semaphores)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            name = $"T{_threadCounter}";
            _threadCounter++;
        }

        if (Threads.Any(r => r.Name == name))
            throw new Exception($"Поток с именем {name} уже существует.");

        foreach (var semaphore in semaphores)
            semaphore.TryAddUser(this);

        Name = name;
        Priority = priority;
        CpuBurst = cpuBurst;
        _quantity = quantity;
        _semaphores = semaphores.ToArray();

        Threads.Add(this);
        
        vb = obj =>
        {
            var timeslice = ((int[])obj)[0];
            var timesliceNumber = ((int[])obj)[1];
            
            _semaphores[_currentSemaphoreIndex].Hold(this);

            State = ThreadState.Running;
            _stopwatch.Restart();
            while (_stopwatch.ElapsedMilliseconds < timeslice * timesliceNumber)
            {
            }

            _semaphores[_currentSemaphoreIndex].Release(this);

            _cpuBurstCompleted += timesliceNumber;

            if (_cpuBurstCompleted < CpuBurst)
            {
                //Console.WriteLine($"{Name} InQueue After Increment CpuBurstCompleted");
                State = ThreadState.InQueue;

                return;
            }

            _cpuBurstCompleted = 0;

            _currentSemaphoreIndex++;

            if (_currentSemaphoreIndex < _semaphores.Length)
            {
                //Console.WriteLine($"{Name} InQueue After Increment CurrentSemaphoreIndex");
                State = ThreadState.InQueue;

                return;
            }

            _currentSemaphoreIndex = 0;

            _quantity--;

            if (_quantity < 1)
            {
                //Console.WriteLine($"{Name} Completed");
                State = ThreadState.Completed;
            }
            else
            {
                //Console.WriteLine($"{Name} InQueue After Decrement Quantity");
                State = ThreadState.InQueue;
            }
        };
    }

    public override string ToString() => $"{Name} state = {State};";

    private ParameterizedThreadStart vb;
    
    public void Execute(int timeslice, int timesliceNumber)
    {
        State = ThreadState.Waiting;
        var thread = new Thread(vb);
        thread.Start(new[] { timeslice, timesliceNumber });
    }

    public int GetRestOfCpuBurst() => CpuBurst - _cpuBurstCompleted;

    public ThreadState State { get; private set; } = ThreadState.InQueue;

    public string Name { get; }

    public int Priority { get; }

    public int CpuBurst { get; }

    public static List<MyThread> Threads { get; } = new();

    private readonly Semaphore[] _semaphores;

    private readonly Stopwatch _stopwatch = new();

    private int _quantity;

    private int _cpuBurstCompleted;

    private int _currentSemaphoreIndex;

    private static int _threadCounter = 1;
}
