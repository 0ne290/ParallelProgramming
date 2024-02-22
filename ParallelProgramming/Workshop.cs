using System.Timers;
using ParallelProgramming.Enums;

namespace ParallelProgramming;

public class Workshop
{
    public Workshop(int timeSlice) => _timeSlice = timeSlice;

    public void StartProduction()
    {
        var timer = new System.Timers.Timer(_timeSlice);
        timer.Elapsed += OnTimedEvent;
        timer.Start();

        while (Detail.Details.Any(d => d.State != ProcessingStates.Completed))
        {
            if (Detail.DetailsInQueue.Count < 1)
                continue;
            var detail = Detail.DetailsInQueue.Dequeue();
            detail.Process();
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

        foreach (var machine in Machine.Machines)
            Console.WriteLine(
                $"Станок {machine.Name}. Обрабатываемые детали: {string.Join(", ", machine.DetailNames)}");
        
        foreach (var detail in Detail.Details)
            Console.WriteLine(
                $"Деталь {detail.Name}. Состояние: {detail.State}. Станок: {detail.TargetMachineName}");

        _sync = false;
    }

    private int _timeSlice;

    private bool _sync;
}