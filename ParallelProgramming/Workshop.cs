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
        
        var timer = new System.Timers.Timer(_timeSlice - 50);// Таймер иногда не успевает записать данные последнего потока, так что в качестве костыля (временно или нет - хз xD) я уменьшил ожидаемое время на 0.05 секунды
        timer.Elapsed += OnTimedEvent;
        timer.Start();
        
        foreach (var detail in _details)
            foreach (var machineName in detail.MachineNames)
                tasks.Add(Task.Run(() => _machines[machineName].Mill(_timeSlice * detail.CpuBurst, detail.Name)));
        
        Task.WaitAll(tasks.ToArray());
        
        timer.Stop();

        while (_sync) { }
        
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