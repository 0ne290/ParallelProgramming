using System.Diagnostics;
using ParallelProgramming.Enums;

namespace ParallelProgramming;

public class Workshop
{
    public Workshop(int timeSlice) => _timeSlice = timeSlice;
    
    public void StartProduction()
    {
        var details = new List<Detail>(Detail.Details);
        Detail.SortDetails(details);
        var stopwatch = new Stopwatch();
        var logTask = Task.CompletedTask;
        
        while (details.Any(d => d.State != ProcessingStates.Completed))
        {
            var tasks = new List<Task<Detail>>();
            foreach (var detail in details)
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

            Detail.SortDetails(details);
        }
        
        _writer.Dispose();
    }

    private async Task Log()
    {
        foreach (var machine in Machine.Machines)
            await _writer.WriteLineAsync(
                $"Станок {machine.Name}. Обрабатываемые детали: {string.Join(", ", machine.DetailNames)}");

        await _writer.WriteLineAsync();

        foreach (var detail in Detail.Details)
            await _writer.WriteLineAsync(
                $"Деталь {detail.Name}. Состояние: {detail.State}. Станок: {detail.TargetMachineName}");

        await _writer.WriteLineAsync();
    }

    private readonly int _timeSlice;

    private readonly StreamWriter _writer = new("Output.txt", false);
}
