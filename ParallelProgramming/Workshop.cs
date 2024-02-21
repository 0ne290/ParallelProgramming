using System.Timers;

namespace ParallelProgramming;

public class Workshop
{
    public Workshop(IEnumerable<Machine> machines, IEnumerable<Detail> details, int timeSlice)
    {
        _machines = machines.ToDictionary(machine => machine.Name);

        _details = details;

        _timeSlice = timeSlice;
    }

    public void StartProduction()
    {
        var tasks = new List<Task>();
        
        var timer = new System.Timers.Timer(_timeSlice);
        timer.Elapsed += OnTimedEvent;
        timer.Start();
        
        foreach (var detail in _details)
        {
            foreach (var machineName in detail.MachineNames)
            {
                for (var i = 0; i < detail.Quantity * detail.CpuBurst; i++)
                {
                    var task = new Task(() =>
                    {
                        _machines[machineName].Mill(_timeSlice, detail.Name);
                        _machines[machineName].DecrementFlow();
                    });
                    _machines[machineName].TaskQueue.Enqueue(task);
                    tasks.Add(task);
                }
            }
        }

        foreach (var machine in _machines.Values)
        {
            while (machine.TaskQueue.Count > 0)
            {
                if (machine.IsAvailable())
                {
                    machine.IncrementFlow();
                    machine.TaskQueue.Dequeue().Start();
                }
            }
        }
        
        Task.WaitAll(tasks.ToArray());
        
        timer.Stop();

        while (_sync) { }// Можно было бы заменить это на AutoResetEvent или добавить к спин-локу код, отдающий квант времени этого потока другому потоку (Thread.Curent.Join(100) или Thread.Sleep(0))
        
        timer.Elapsed -= OnTimedEvent;
        timer.Close();
        timer.Dispose();
    }
    
    private void OnTimedEvent(object? source, ElapsedEventArgs e)
    {
        _sync = true;

        foreach (var machine in _machines.Values)
            Console.WriteLine(
                $"Станок {machine.Name}. Обрабатываемые детали: {string.Join(", ", machine.DetailNames)}");

        _sync = false;
    }

    private IReadOnlyDictionary<string, Machine> _machines;

    private IEnumerable<Detail> _details;

    private int _timeSlice;

    private bool _sync;
}