using System.Diagnostics;
using ParallelProgramming.Enums;

namespace ParallelProgramming;

public class Workshop
{
    public Workshop(int timeSlice) => _timeSlice = timeSlice;
    
    public void StartProduction()
    {
        Detail.SortDetails();
        var stopwatch = new Stopwatch();
        var logTask = Task.CompletedTask;
        
        while (Detail.Details.Any(d => d.State != ProcessingStates.Completed))
        {
            var tasks = new List<Task<Detail>>();
            foreach (var detail in Detail.Details)
            {
                if (!detail.IsAvailabe() || detail.State != ProcessingStates.Queued)
                    continue;
                
                detail.Preprocess();
                tasks.Add(detail.Process());
            }
            
            stopwatch.Restart();
            while (stopwatch.ElapsedMilliseconds < _timeSlice) { }
            
            logTask.Wait();
            logTask = Log();
            
            foreach (var task in tasks)
            {
                var detail = task.Result;
                detail.Postprocess();
            }

            Detail.SortDetails();
        }
    }

    private async Task Log() => await Task.Run(() =>
    {
        foreach (var machine in Machine.Machines)
            Console.WriteLine(
                $"Станок {machine.Name}. Обрабатываемые детали: {string.Join(", ", machine.DetailNames)}");

        Console.WriteLine();

        foreach (var detail in Detail.Details)
            Console.WriteLine($"Деталь {detail.Name}. Состояние: {detail.State}. Станок: {detail.TargetMachineName}");

        Console.WriteLine();
    });

    private readonly int _timeSlice;
}
